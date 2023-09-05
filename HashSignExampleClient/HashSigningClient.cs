using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Microsoft.Web.WebView2.Core;
using HashSignExampleClient.classes;
using System.IO;

namespace HashSignExampleClient
{
    public partial class HashSigningClient : Form
    {
        private GUIHashSigner hashSigner;
        private bool startedSigningProcess = false;

        private CancellationTokenSource cts = null;
        private CancellationToken hashSigningCancelToken;

        private DocumentMeta[] documents;

        public HashSigningClient()
        {
            InitializeComponent();
            loggingBox.ReadOnly = true;
            hashSigner = new GUIHashSigner(webView, loggingBox, loadingSpinner);
            webView.NavigationStarting += this.webbrowser_Navigated;
            loggingBox.AppendText("Welcome to A-Trust's Hash Signing Client. This application shows a sample implementation of CSC hash signing by A-Trust. An already prepared PDF file and its metadata are ready for the signature process.");
            loggingBox.AppendText("\n\n Start the signing process by pressing the \"Start\"-Button.");

            documents = new DocumentMeta[1];
            documents[0] = new DocumentMeta();
            documents[0].filename = "signTest1.pdf";
            string executionDirectory = System.IO.Directory.GetCurrentDirectory();
            string projectDirectory = Directory.GetParent(executionDirectory).Parent.FullName;
            documents[0].path = projectDirectory + "/documents/signTest1.pdf";
            documents[0].startZeroBytes = 1178;
            documents[0].endZeroBytes = 20124;
        }

        /***
         * embedded webbrowser has navigated method
         * 
         * searches for parameter code in the query and passes it to the hashSigner if found
         */
        public void webbrowser_Navigated(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var queryParameters = HttpUtility.ParseQueryString(new Uri(e.Uri).Query);
            if (queryParameters.AllKeys.Contains("code"))
            {
                string code = queryParameters["code"];
                loggingBox.AppendText("code found: " + code + "\n");
                hashSigner.continueExcecution(code);
            }
        }

        /***
         * startButton onClick method - starts the hash signing process as new Thread
         */
        private async void startButton_Click(object sender, EventArgs e)
        {
            if (startedSigningProcess)
            {
                return;
            }
            startedSigningProcess = true;
            loggingBox.Clear();
            cts = new CancellationTokenSource();
            hashSigningCancelToken = cts.Token;
            atrustLogo.Visible = false;
            webView.Enabled = true;
            startButton.Enabled = false;
            stopButton.Enabled = true;

            // hash signing process
            await Task.Run(() => hashSigner.Execute(hashSigningCancelToken, documents));
            startedSigningProcess = false;
            displayPdfButton.Enabled = true;
            startButton.Enabled = true;
            stopButton.Enabled = false;
            webView.Visible = false;
            atrustLogo.Visible = true;
            loadingSpinner.Visible = false;
            startedSigningProcess = false;
        }

        /// <summary>
        /// Stops the execution of the signing process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
            hashSigner.continueExcecution("");
            startButton.Enabled = true;
            stopButton.Enabled = false;
            webView.Source = new Uri("about:blank");
            startedSigningProcess = false;
            loggingBox.AppendText("\nStopped by the user.\nClick the start button to begin the process with the same documents or click the select documents button to load different ones.");
        }

        /// <summary>
        /// Clears the logs inside the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearLogsButton_Click(object sender, EventArgs e)
        {
            loggingBox.Text = string.Empty;
            webView.Visible = false;
            loadingSpinner.Visible = true;
        }

        /// <summary>
        /// Executes event when the webbrowser finishes navigating to other sites.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!startedSigningProcess)
            {
                return;
            }
            if (e.HttpStatusCode != 200 && e.HttpStatusCode != 302)
            {
                webView.Visible = false;
                loadingSpinner.Visible = true;
            }
            else
            {
                webView.Visible = true;
                loadingSpinner.Visible = false;
            }
        }

        /// <summary>
        /// Executes event when the display PDF button gets clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void displayPdfButton_Click(object sender, EventArgs e)
        {
            if (documents != null)
            {
                PdfViewer pdfViewer = new PdfViewer(documents);
                pdfViewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("Error getting PDF-Url.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executes event when the load PDF button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPdfButton_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "PDF files (*.pdf)|*.pdf";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                if(ofd.FileNames == null || ofd.FileNames.Length == 0)
                {
                    loggingBox.AppendText("\nError loading pdf document(s)");

                    return;
                }
                documents = new DocumentMeta[ofd.FileNames.Length];

                for (int i = 0; i < ofd.FileNames.Length; i++)
                {
                    documents[i] = new DocumentMeta();
                    documents[i].path = ofd.FileNames[i];
                    documents[i].filename = ofd.SafeFileNames[i];
                }
                displayPdfButton.Enabled = true;
                startButton.Enabled = true;
            }
            loggingBox.AppendText("\nPdf document(s) successfully loaded.\nPress the start button to begin the signing process.");
        }
    }
}
