using HashSignExampleClient.classes;
using System;
using System.Windows.Forms;

namespace HashSignExampleClient
{
    public partial class PdfViewer : Form
    {
        private DocumentMeta[] documents = null;
        private int selection = 0;

        public PdfViewer(DocumentMeta[] documents)
        {
            InitializeComponent();
            this.documents = documents;
            DocViewer.Source = new Uri(this.documents[0].path);
            for (int i = 0; i < documents.Length; i++)
            {
                documentList.Items.Add(documents[i].filename);
            }
            documentList.SetSelected(0, true);
        }

        private void documentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selection = documentList.SelectedIndex;
            DocViewer.Source = new Uri((string)documents[documentList.SelectedIndex].path);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }       

        private void checkOnlyDigits(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) ||
        (e.KeyChar == '.'))
            {
                e.Handled = true;
            }
        }
    }
}
