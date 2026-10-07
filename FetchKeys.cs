using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MessageCrypter;

/// <summary>
/// class to represent pgp key
/// </summary>
public class GpgKey
{
    public string KeyId { get; set; } = string.Empty;
    public string Fingerprint { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool HasSecretKey { get; set; }
    public bool IsExpired { get; set; }

    /// <summary>
    /// displays text in mykeys, shows name and email
    /// </summary>
    public override string ToString()
    {
        var markers = new List<string>();
        if (HasSecretKey) markers.Add("🔑");
        if (IsExpired)    markers.Add("⏰");

        var prefix = markers.Count > 0 ? string.Join(" ", markers) + " " : "";
        return $"{prefix}{UserId}";
    }
}

/// <summary>
/// gets keys from path
/// </summary>
public static class FetchKeys
{
    /// <summary>
    /// Uses gpgconf if available, otherwise falls back to %APPDATA%\gnupg to  get the keys
    public static string GetGnuPgHome()
    {
        // runs pgpconf to test if is available
        var fromGpgConf = TryRunGpgConf();
        if (!string.IsNullOrWhiteSpace(fromGpgConf) && Directory.Exists(fromGpgConf))
            return fromGpgConf;

        // if that fails try windows directory
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "gnupg");
    }

    /// <summary>
    /// Locates gpg.exe. Checks PATH first, then common Gpg4win install locations.
    /// </summary>
    public static string? FindGpgExecutable()
    {
        //PATH
        foreach (var name in new[] { "gpg.exe", "gpg" })
        {
            var onPath = TryResolveOnPath(name);
            if (onPath != null) return onPath;
        }

        //Common Gpg4win install locations in windows
        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),   "GnuPG", "bin", "gpg.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),"GnuPG", "bin", "gpg.exe"),
            @"C:\Program Files (x86)\GnuPG\bin\gpg.exe",
            @"C:\Program Files\GnuPG\bin\gpg.exe",
        };

        return candidates.FirstOrDefault(File.Exists);
    }

    /// <summary>
    /// Lists all public keys in the user's keyring. Each entry also indicates
    /// whether a matching secret key is present.
    /// </summary>
    public static List<GpgKey> GetPublicKeys()
    {
        var gpg = FindGpgExecutable()
            ?? throw new FileNotFoundException(
                "Could not find gpg.exe. Make sure Gpg4win is installed and gpg.exe is on PATH.");

        var home = GetGnuPgHome();
        if (!Directory.Exists(home))
            throw new DirectoryNotFoundException(
                $"GnuPG home directory not found at: {home}");

        // --with-colons gives a stable, machine-readable format.
        // --fixed-list-mode + --with-fingerprint gives us fingerprints on 'fpr' lines.
        // --list-keys lists public keys. Secret subkeys are flagged by 'sec' records.
        var args = new[]
        {
            "--homedir", home,
            "--batch",
            "--no-tty",
            "--with-colons",
            "--fixed-list-mode",
            "--with-fingerprint",
            "--list-keys"
        };

        var raw = RunProcess(gpg, args, out var stderr);

        if (string.IsNullOrWhiteSpace(raw))
        {
            // No keys, or gpg failed silently.
            return new List<GpgKey>();
        }

        var keys = ParseColonListing(raw);

        // Determine which keys have a secret counterpart.
        var secretFingerprints = GetSecretKeyFingerprints(gpg, home);
        foreach (var k in keys)
            k.HasSecretKey = secretFingerprints.Contains(k.Fingerprint);

        return keys
            .OrderBy(k => k.UserId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }



    private static HashSet<string> GetSecretKeyFingerprints(string gpg, string home)
    {
        var args = new[]
        {
            "--homedir", home,
            "--batch",
            "--no-tty",
            "--with-colons",
            "--fixed-list-mode",
            "--with-fingerprint",
            "--list-secret-keys"
        };

        var raw = RunProcess(gpg, args, out _);
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(raw)) return result;

        string? pendingFpr = null;
        foreach (var line in raw.Split('\n'))
        {
            var fields = line.Split(':');
            if (fields.Length < 10) continue;

            switch (fields[0])
            {
                case "sec": // secret key record
                    pendingFpr = null;
                    break;
                case "fpr":
                    if (pendingFpr == null && fields.Length > 9)
                        pendingFpr = fields[9];
                    break;
                case "ssb": // secret subkey — reset pending
                    pendingFpr = null;
                    break;
            }

            if (!string.IsNullOrEmpty(pendingFpr))
            {
                result.Add(pendingFpr);
                pendingFpr = null;
            }
        }
        return result;
    }

    private static List<GpgKey> ParseColonListing(string output)
    {
        var keys = new List<GpgKey>();
        GpgKey? current = null;
        bool expectingFpr = false;

        foreach (var rawLine in output.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            if (string.IsNullOrEmpty(line)) continue;

            var f = line.Split(':');
            if (f.Length < 10) continue;

            var recordType = f[0];

            switch (recordType)
            {
                case "pub":
                    // Flush previous key
                    if (current != null) keys.Add(current);

                    current = new GpgKey
                    {
                        KeyId    = f[4],
                        IsExpired = f[1] == "e" || f[1] == "r"
                    };
                    expectingFpr = true;
                    break;

                case "fpr":
                    if (current != null && expectingFpr)
                    {
                        current.Fingerprint = f[9];
                        expectingFpr = false;
                    }
                    break;

                case "uid":
                    if (current != null && string.IsNullOrEmpty(current.UserId))
                    {
                        var uid = DecodeUid(f[9]);
                        current.UserId = uid;
                        current.Email  = ExtractEmail(uid);
                    }
                    break;
            }
        }

        if (current != null) keys.Add(current);
        return keys;
    }

    /// <summary>
    /// gpg --with-colons escapes certain characters with \xNN. Decode them back.
    /// </summary>
    private static string DecodeUid(string escaped)
    {
        var sb = new StringBuilder(escaped.Length);
        for (int i = 0; i < escaped.Length; i++)
        {
            if (escaped[i] == '\\' && i + 3 < escaped.Length && escaped[i + 1] == 'x')
            {
                if (byte.TryParse(escaped.Substring(i + 2, 2),
                                  System.Globalization.NumberStyles.HexNumber,
                                  null, out var b))
                {
                    sb.Append((char)b);
                    i += 3;
                    continue;
                }
            }
            sb.Append(escaped[i]);
        }
        return sb.ToString();
    }

    private static string ExtractEmail(string uid)
    {
        var start = uid.IndexOf('<');
        var end   = uid.IndexOf('>', start + 1);
        if (start >= 0 && end > start)
            return uid.Substring(start + 1, end - start - 1).Trim();
        return string.Empty;
    }

    private static string? TryRunGpgConf()
    {
        try
        {
            var gpgconf = FindOnPathOrInstall("gpgconf.exe")
                ?? FindOnPathOrInstall("gpgconf");
            if (gpgconf == null) return null;

            var output = RunProcess(gpgconf, new[] { "--list-dirs", "homedir" }, out _);
            var line = output.Split('\n').FirstOrDefault();
            return line?.Trim().TrimEnd('\r');
        }
        catch
        {
            return null;
        }
    }

    private static string? FindOnPathOrInstall(string exeName)
    {
        var onPath = TryResolveOnPath(exeName);
        if (onPath != null) return onPath;

        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),    "GnuPG", "bin", exeName),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "GnuPG", "bin", exeName),
        };
        return candidates.FirstOrDefault(File.Exists);
    }

    private static string? TryResolveOnPath(string exeName)
    {
        var pathVar = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(pathVar)) return null;

        foreach (var dir in pathVar.Split(Path.PathSeparator))
        {
            try
            {
                var full = Path.Combine(dir.Trim(), exeName);
                if (File.Exists(full)) return full;
            }
            catch { /* skip bad PATH entries */ }
        }
        return null;
    }

    private static string RunProcess(string fileName, string[] args, out string stderr)
    {
        var psi = new ProcessStartInfo
        {
            FileName               = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);

        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start {fileName}");

        var stdout = proc.StandardOutput.ReadToEnd();
        stderr     = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        return stdout;
    }
}