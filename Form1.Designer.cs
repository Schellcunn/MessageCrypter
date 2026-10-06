namespace MessageCrypter;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.TableLayoutPanel rootLayout;
    private System.Windows.Forms.Label lblMyKeys;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.Label lblRecipient;
    private System.Windows.Forms.ListBox lstMyKeys;
    private System.Windows.Forms.ListBox lstRecipientKeys;
    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.Panel bottomPanel;
    private System.Windows.Forms.FlowLayoutPanel buttonPanel;
    private System.Windows.Forms.Button btnEncrypt;
    private System.Windows.Forms.Button btnDecrypt;
    private System.Windows.Forms.Button btnCopy;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
        this.lblMyKeys = new System.Windows.Forms.Label();
        this.lblMessage = new System.Windows.Forms.Label();
        this.lblRecipient = new System.Windows.Forms.Label();
        this.lstMyKeys = new System.Windows.Forms.ListBox();
        this.lstRecipientKeys = new System.Windows.Forms.ListBox();
        this.txtInput = new System.Windows.Forms.TextBox();
        this.txtOutput = new System.Windows.Forms.TextBox();
        this.bottomPanel = new System.Windows.Forms.Panel();
        this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.btnEncrypt = new System.Windows.Forms.Button();
        this.btnDecrypt = new System.Windows.Forms.Button();
        this.btnCopy = new System.Windows.Forms.Button();

        this.rootLayout.SuspendLayout();
        this.bottomPanel.SuspendLayout();
        this.buttonPanel.SuspendLayout();
        this.SuspendLayout();

        // Layout
        this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.rootLayout.ColumnCount = 3;
        this.rootLayout.RowCount = 3;
        this.rootLayout.Padding = new System.Windows.Forms.Padding(8);
        this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
        this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
        this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
        this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.rootLayout.Controls.Add(this.lblMyKeys, 0, 0);
        this.rootLayout.Controls.Add(this.lblMessage, 1, 0);
        this.rootLayout.Controls.Add(this.lblRecipient, 2, 0);
        this.rootLayout.Controls.Add(this.lstMyKeys, 0, 1);
        this.rootLayout.Controls.Add(this.lstRecipientKeys, 2, 1);
        this.rootLayout.Controls.Add(this.txtInput, 1, 1);
        this.rootLayout.Controls.Add(this.bottomPanel, 1, 2);
        this.rootLayout.SetRowSpan(this.lstMyKeys, 2);
        this.rootLayout.SetRowSpan(this.lstRecipientKeys, 2);

        //headers
        this.lblMyKeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblMyKeys.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblMyKeys.Text = "My Keys";
        this.lblMyKeys.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

        this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblMessage.Text = "Message";
        this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

        this.lblRecipient.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblRecipient.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblRecipient.Text = "Recipient";
        this.lblRecipient.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

        //lstMyKeys
        this.lstMyKeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lstMyKeys.IntegralHeight = false;
        this.lstMyKeys.SelectedIndexChanged += new System.EventHandler(this.lstMyKeys_SelectedIndexChanged);

        //lstRecipientKeys
        this.lstRecipientKeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lstRecipientKeys.IntegralHeight = false;
        this.lstRecipientKeys.SelectedIndexChanged += new System.EventHandler(this.lstRecipientKeys_SelectedIndexChanged);

        //txtInput
        this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtInput.Multiline = true;
        this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtInput.AcceptsReturn = true;
        this.txtInput.Font = new System.Drawing.Font("Consolas", 10F);
        this.txtInput.TextChanged += new System.EventHandler(this.txtInput_TextChanged);

        //bottomPanel (output + buttons)
        this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.bottomPanel.Controls.Add(this.txtOutput);   
        this.bottomPanel.Controls.Add(this.buttonPanel); 

        //txtOutput
        this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtOutput.Multiline = true;
        this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtOutput.ReadOnly = true;
        this.txtOutput.Font = new System.Drawing.Font("Consolas", 10F);
        this.txtOutput.BackColor = System.Drawing.SystemColors.Window;
        this.txtOutput.TextChanged += new System.EventHandler(this.txtOutput_TextChanged);

        //buttonPanel
        this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.buttonPanel.Height = 40;
        this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
        this.buttonPanel.Controls.Add(this.btnEncrypt);
        this.buttonPanel.Controls.Add(this.btnDecrypt);
        this.buttonPanel.Controls.Add(this.btnCopy);

        //buttons
        this.btnEncrypt.Text = "Encrypt";
        this.btnEncrypt.Width = 90;
        this.btnEncrypt.Enabled = false;
        this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);

        this.btnDecrypt.Text = "Decrypt";
        this.btnDecrypt.Width = 90;
        this.btnDecrypt.Enabled = false;
        this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);

        this.btnCopy.Text = "Copy";
        this.btnCopy.Width = 90;
        this.btnCopy.Enabled = false;
        this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);

        //Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1000, 650);
        this.MinimumSize = new System.Drawing.Size(700, 450);
        this.Text = "MessageCrypter";
        this.Controls.Add(this.rootLayout);

        this.rootLayout.ResumeLayout(false);
        this.bottomPanel.ResumeLayout(false);
        this.buttonPanel.ResumeLayout(false);
        this.ResumeLayout(false);
    }
}