using System.Windows.Forms;

namespace HashSignExampleClient
{
    partial class HashSigningClient
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashSigningClient));
            this.startButton = new System.Windows.Forms.Button();
            this.loggingBox = new System.Windows.Forms.RichTextBox();
            this.logLabel = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.clearButton = new System.Windows.Forms.Button();
            this.displayPdfButton = new System.Windows.Forms.Button();
            this.PdfControlsGroupBox = new System.Windows.Forms.GroupBox();
            this.ProgramControlsGroupBox = new System.Windows.Forms.GroupBox();
            this.atrustLogo = new System.Windows.Forms.PictureBox();
            this.loadingSpinner = new Spinner();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.PdfControlsGroupBox.SuspendLayout();
            this.ProgramControlsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.atrustLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.startButton.Location = new System.Drawing.Point(47, 35);
            this.startButton.Margin = new System.Windows.Forms.Padding(2);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(97, 25);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // loggingBox
            // 
            this.loggingBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggingBox.Location = new System.Drawing.Point(11, 30);
            this.loggingBox.Margin = new System.Windows.Forms.Padding(2);
            this.loggingBox.Name = "loggingBox";
            this.loggingBox.Size = new System.Drawing.Size(576, 415);
            this.loggingBox.TabIndex = 2;
            this.loggingBox.Text = "";
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(14, 15);
            this.logLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(30, 13);
            this.logLabel.TabIndex = 3;
            this.logLabel.Text = "Logs";
            // 
            // stopButton
            // 
            this.stopButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(47, 64);
            this.stopButton.Margin = new System.Windows.Forms.Padding(2);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(97, 25);
            this.stopButton.TabIndex = 4;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Enabled = false;
            this.webView.Location = new System.Drawing.Point(593, 0);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(407, 646);
            this.webView.Source = new System.Uri("about:blank", System.UriKind.Absolute);
            this.webView.TabIndex = 5;
            this.webView.Visible = false;
            this.webView.ZoomFactor = 1D;
            this.webView.NavigationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs>(this.webView_NavigationCompleted);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.clearButton.Location = new System.Drawing.Point(47, 95);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(97, 25);
            this.clearButton.TabIndex = 6;
            this.clearButton.Text = "Clear Logs";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearLogsButton_Click);
            // 
            // displayPdfButton
            // 
            this.displayPdfButton.Location = new System.Drawing.Point(51, 61);
            this.displayPdfButton.Name = "displayPdfButton";
            this.displayPdfButton.Size = new System.Drawing.Size(97, 25);
            this.displayPdfButton.TabIndex = 8;
            this.displayPdfButton.Text = "Display PDF";
            this.displayPdfButton.UseVisualStyleBackColor = true;
            this.displayPdfButton.Click += new System.EventHandler(this.displayPdfButton_Click);
            // 
            // PdfControlsGroupBox
            // 
            this.PdfControlsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.PdfControlsGroupBox.Controls.Add(this.displayPdfButton);
            this.PdfControlsGroupBox.Location = new System.Drawing.Point(52, 475);
            this.PdfControlsGroupBox.Name = "PdfControlsGroupBox";
            this.PdfControlsGroupBox.Size = new System.Drawing.Size(200, 144);
            this.PdfControlsGroupBox.TabIndex = 9;
            this.PdfControlsGroupBox.TabStop = false;
            this.PdfControlsGroupBox.Text = "PDF Controls";
            // 
            // ProgramControlsGroupBox
            // 
            this.ProgramControlsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ProgramControlsGroupBox.Controls.Add(this.startButton);
            this.ProgramControlsGroupBox.Controls.Add(this.clearButton);
            this.ProgramControlsGroupBox.Controls.Add(this.stopButton);
            this.ProgramControlsGroupBox.Location = new System.Drawing.Point(345, 472);
            this.ProgramControlsGroupBox.Name = "ProgramControlsGroupBox";
            this.ProgramControlsGroupBox.Size = new System.Drawing.Size(189, 147);
            this.ProgramControlsGroupBox.TabIndex = 10;
            this.ProgramControlsGroupBox.TabStop = false;
            this.ProgramControlsGroupBox.Text = "Program Controls";
            // 
            // atrustLogo
            // 
            this.atrustLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.atrustLogo.BackColor = System.Drawing.Color.Transparent;
            this.atrustLogo.Image = ((System.Drawing.Image)(resources.GetObject("atrustLogo.Image")));
            this.atrustLogo.Location = new System.Drawing.Point(665, 262);
            this.atrustLogo.Name = "atrustLogo";
            this.atrustLogo.Size = new System.Drawing.Size(276, 122);
            this.atrustLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.atrustLogo.TabIndex = 11;
            this.atrustLogo.TabStop = false;
            // 
            // loadingSpinner
            // 
            this.loadingSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadingSpinner.BackColor = System.Drawing.Color.Transparent;
            this.loadingSpinner.Location = new System.Drawing.Point(743, 72);
            this.loadingSpinner.Name = "loadingSpinner";
            this.loadingSpinner.NodeBorderColor = System.Drawing.Color.White;
            this.loadingSpinner.NodeBorderSize = 2;
            this.loadingSpinner.NodeCount = 8;
            this.loadingSpinner.NodeFillColor = System.Drawing.Color.Black;
            this.loadingSpinner.NodeRadius = 4;
            this.loadingSpinner.NodeResizeRatio = 1F;
            this.loadingSpinner.Size = new System.Drawing.Size(100, 503);
            this.loadingSpinner.SpinnerRadius = 100;
            this.loadingSpinner.TabIndex = 7;
            this.loadingSpinner.Text = "spinner1";
            this.loadingSpinner.Visible = false;
            // 
            // HashSigningClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 646);
            this.Controls.Add(this.atrustLogo);
            this.Controls.Add(this.ProgramControlsGroupBox);
            this.Controls.Add(this.PdfControlsGroupBox);
            this.Controls.Add(this.loadingSpinner);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.loggingBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(1016, 685);
            this.Name = "HashSigningClient";
            this.Text = "HashSigningClient";
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.PdfControlsGroupBox.ResumeLayout(false);
            this.ProgramControlsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.atrustLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.RichTextBox loggingBox;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.Button stopButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private Button clearButton;
        private Spinner loadingSpinner;
        private Button displayPdfButton;
        private GroupBox PdfControlsGroupBox;
        private GroupBox ProgramControlsGroupBox;
        private PictureBox atrustLogo;
    }
}

