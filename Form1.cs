using System;
using System.Windows.Forms;

namespace MessageCrypter;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
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
}