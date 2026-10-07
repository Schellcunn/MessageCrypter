using System;
using System.Windows.Forms;
using System.Linq;


namespace MessageCrypter;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        LoadKeys();
        UpdateButtonState();
    }

    //Enable/disable buttons based on selection state
    private void UpdateButtonState()
    {
        bool hasMyKey        = lstMyKeys.SelectedItem != null;
        bool hasRecipientKey = lstRecipientKeys.SelectedItem != null;
        bool hasInput        = !string.IsNullOrWhiteSpace(txtInput.Text);

        btnEncrypt.Enabled = hasMyKey && hasRecipientKey && hasInput;
        btnDecrypt.Enabled = hasMyKey && hasInput;
        btnCopy.Enabled    = !string.IsNullOrWhiteSpace(txtOutput.Text);
    }

    private void lstMyKeys_SelectedIndexChanged(object sender, EventArgs e)
        => UpdateButtonState();

    private void lstRecipientKeys_SelectedIndexChanged(object sender, EventArgs e)
        => UpdateButtonState();

    private void txtInput_TextChanged(object sender, EventArgs e)
        => UpdateButtonState();

    private void txtOutput_TextChanged(object sender, EventArgs e)
        => UpdateButtonState();

    //Button handlers
    private void btnEncrypt_Click(object sender, EventArgs e)
    {
        
        
        txtOutput.Text = "-----BEGIN PGP MESSAGE-----\n...placeholder...\n-----END PGP MESSAGE-----";
    }

    private void btnDecrypt_Click(object sender, EventArgs e)
    {
        
        txtOutput.Text = "placeholder";
    }

    private void btnCopy_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtOutput.Text))
            return;

        Clipboard.SetText(txtOutput.Text);

       
        var original = btnCopy.Text;
        btnCopy.Text = "Copied!";
        var timer = new System.Windows.Forms.Timer { Interval = 900 };
        timer.Tick += (s, _) =>
        {
            btnCopy.Text = original;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }
    private void LoadKeys()
    {
        try
        {
            var keys = FetchKeys.GetPublicKeys();

            lstMyKeys.Items.Clear();
            lstRecipientKeys.Items.Clear();

            foreach (var key in keys)
            {
                // Show all keys in both lists, the user picks which to use
                lstMyKeys.Items.Add(key);
                lstRecipientKeys.Items.Add(key);
            }

            if (keys.Count == 0)
            {
                MessageBox.Show(
                    "No PGP keys found in your GnuPG keyring.\n\n" +
                    "Generate or import a key pair in Kleopatra first.",
                    "No keys found",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (FileNotFoundException ex)
        {
            MessageBox.Show(ex.Message, "GnuPG not found",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load keys: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}