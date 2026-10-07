using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MessageCrypter;

/// <summary>
/// Encrypts plaintext to one or more recipients using the user's GnuPG keyring.
/// Uses gpg.exe as a subprocess so no private key material ever enters this process.
/// </summary>
public static class InputEncrypt
{
    /// <summary>
    /// Encrypts <paramref name="plaintext"/> for the given recipient key fingerprints.
    /// Returns ASCII-armored PGP ciphertext.
    /// </summary>
    /// <param name="plaintext">The message to encrypt.</param>
    /// <param name="recipientFingerprints">One or more full fingerprints of recipient public keys.</param>
    /// <param name="signWithFingerprint">
    /// Optional. If provided, the message is also signed with this key (must be a secret key).
    /// </param>
    public static string Encrypt(
        string plaintext,
        IEnumerable<string> recipientFingerprints,
        string? signWithFingerprint = null)
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new ArgumentException("Plaintext is empty.", nameof(plaintext));

        var recipients = recipientFingerprints?.Where(f => !string.IsNullOrWhiteSpace(f)).ToList()
                         ?? new List<string>();
        if (recipients.Count == 0)
            throw new ArgumentException("At least one recipient is required.", nameof(recipientFingerprints));

        var gpg  = FetchKeys.FindGpgExecutable()
            ?? throw new FileNotFoundException(
                "Could not find gpg.exe. Make sure Gpg4win is installed and gpg.exe is on PATH.");

        var home = FetchKeys.GetGnuPgHome();

        // Build the gpg argument list.
        var args = new List<string>
        {
            "--homedir", home,
            "--batch",
            "--no-tty",
            "--yes",                       // overwrite if asked 
            "--armor",                     // ASCII-armored output
            "--trust-model", "always",     // skip trust prompt
            "--encrypt",
            "--output", "-",               // write ciphertext to stdout
        };

        // Add each recipient by fingerprint
        foreach (var fpr in recipients)
        {
            args.Add("--recipient");
            args.Add(fpr);
        }

        // Optionally sign with the user's own key.
        if (!string.IsNullOrWhiteSpace(signWithFingerprint))
        {
            args.Add("--sign");
            args.Add("--local-user");
            args.Add(signWithFingerprint);
        }

        // Read plaintext from stdin, write ciphertext to stdout.
        var psi = new ProcessStartInfo
        {
            FileName               = gpg,
            RedirectStandardInput  = true,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding  = Encoding.UTF8,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);

        using var proc = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start gpg.exe");

        // Feed plaintext to stdin. Use UTF-8 without BOM.
        var stdin = new StreamWriter(proc.StandardInput.BaseStream, new UTF8Encoding(false));
        stdin.Write(plaintext);
        stdin.Flush();
        stdin.Close();   // close stdin so gpg knows we're done

        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();

        if (proc.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"gpg failed (exit {proc.ExitCode}):\n{stderr.Trim()}");
        }

        return stdout;
    }
}