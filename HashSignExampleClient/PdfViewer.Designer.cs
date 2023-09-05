namespace HashSignExampleClient
{
    partial class PdfViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfViewer));
            this.DocViewer = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.documentList = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.DocViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // DocViewer
            // 
            this.DocViewer.AllowExternalDrop = true;
            this.DocViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DocViewer.CreationProperties = null;
            this.DocViewer.DefaultBackgroundColor = System.Drawing.Color.White;
            this.DocViewer.Location = new System.Drawing.Point(2, 0);
            this.DocViewer.Name = "DocViewer";
            this.DocViewer.Size = new System.Drawing.Size(769, 782);
            this.DocViewer.TabIndex = 0;
            this.DocViewer.ZoomFactor = 1D;
            // 
            // documentList
            // 
            this.documentList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.documentList.FormattingEnabled = true;
            this.documentList.Location = new System.Drawing.Point(777, 0);
            this.documentList.Name = "documentList";
            this.documentList.Size = new System.Drawing.Size(185, 797);
            this.documentList.TabIndex = 1;
            this.documentList.SelectedIndexChanged += new System.EventHandler(this.documentList_SelectedIndexChanged);
            // 
            // PdfViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 794);
            this.Controls.Add(this.documentList);
            this.Controls.Add(this.DocViewer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PdfViewer";
            this.Text = "PdfViewer";
            ((System.ComponentModel.ISupportInitialize)(this.DocViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 DocViewer;
        private System.Windows.Forms.ListBox documentList;
    }
}