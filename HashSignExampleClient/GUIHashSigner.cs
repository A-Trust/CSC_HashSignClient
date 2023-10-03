using ExampleClient;
using HashSignExampleClient.classes;
using Microsoft.Web.WebView2.WinForms;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace HashSignExampleClient
{
    internal class GUIHashSigner
    {
        // Test-Credentials for Handy-Signature:
        // Phone number: 1030121749133712
        // Password: 123456789
        // TAN: 123456

        // Test-Credentials used for the CSC HashSigning Service
        // If you have further questions, please contact the A-Trust Sales Team (sales@a-trust.at)
        #region Hash-Sign Options
        private string boxurl = @"https://testbox.a-trust.at/csc/v1/";
        private string redirect_url = "test1.a-trust.at";
        private string clientId = "github_client_id";
        private string clientSecret = "github_client_secret";
        private string subscriberId = "github_subscriber_id";
        #endregion

        #region PreparePDFData
        private DocumentMeta[] documents;
        #endregion

        private ManualResetEvent manualResetEvent;
        private RichTextBox loggingBox = null;
        private WebView2 browser = null;
        private Spinner spinner = null;

        private string code = "";
        private string language = "de";

        public GUIHashSigner(WebView2 browser, RichTextBox loggingBox, Spinner spinner)
        {
            this.browser = browser;
            this.loggingBox = loggingBox;
            this.spinner = spinner;
        }

        /// <summary>
        /// The main method for the signing process. All API Calls are executed and processes right here.
        /// </summary>
        /// <param name="ct">CancelationToken for running Execute in its own Thread</param>
        /// <param name="documents">The metadata for all the documents</param>
        /// <returns>Bool</returns>
        public async Task<bool> Execute(CancellationToken ct, DocumentMeta[] documents)
        {
            #region Program specific

            manualResetEvent = new ManualResetEvent(false);
            this.documents = documents;
            appendLog("Started");
            #endregion
            
            // remove the empty signature bytes from the prepared PDF
            GetDocsWithoutZeroes();

            if (documents == null)
            {
                appendLog("error could not remove 0s from PDF successfully");
                return false;
            }

            #region Generate AccountToken & instanciate HashSignClient (Helper-Class)

            // One account token is needed for every API-Call-Cycle
            string account_token = TokenGenerator.GenerateToken(subscriberId, clientId, clientSecret);

            HashSignHelper boxClient = new HashSignHelper(boxurl, redirect_url, account_token, clientId, clientSecret, this.documents.Length, language);
            #endregion

            #region AuthorizeService - oauth2/authorize (API Call via Browser)

            // First in-browser authorization via HandySignatur.AT
            string handySigUrl = boxClient.AuthorizeService();

            if (handySigUrl == null)
            {
                appendLog("error did not return handySigUrl");
                return false;
            }

            browserNavigateTo(handySigUrl);
            appendLog("\nPlease login with your Handy-Signatur or ID Austria\n");
            appendLog("Enter your ID Austria/HandySignatur credentials.\n");

            manualResetEvent.WaitOne();
            if (ct.IsCancellationRequested)
            {
                return false;
            }
            if (code == null)
            {
                appendLog("error on AuthorizeService");
                return false;
            }

            #endregion

            #region GetToken - oauth2/token (API Call)

            // Get Access- & RefreshTokens
            var tokenResponse = boxClient.GetTokens(code);

            if (ct.IsCancellationRequested)
            {
                return false;
            }
            if (tokenResponse == null)
            {
                appendLog("error on getTokens");
                return false;
            }

            string access_token = tokenResponse.access_token;
            string refresh_token = tokenResponse.refresh_token;

            appendLog("\nRefreshToken: " + refresh_token);
            appendLog("AccessToken: " + access_token);
            appendLog("Expires in: " + tokenResponse.expires_in + "");
            appendLog("Token Type: " + tokenResponse.token_type);

            #endregion

            #region credentials/list (API Call)
            // Get the credentialIDs for your selected documents
            var credentialList = boxClient.GetCredentialsList(access_token);

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            if (credentialList == null)
            {
                appendLog("error on GetCredentialList");
                return false;
            }

            string credentialId = credentialList.credentialIDs[0];
            string[] certificates = boxClient.getCertificateInfo(credentialId, access_token).cert.certificates;
            appendLog("\nCredentialId: " + credentialId);

            #endregion

            #region Create SignedAttributes from document & create Hashes from it

            // Create SignedAttributes
            createSignedAttributes(certificates);

            // Create hash from signedAttribute
            string hashes = getHashes(false);
            #endregion

            #region AuthorizeCredentials - oauth2/authorize (API Call via Browser)

            string handySigUrlCredentials = boxClient.AuthorizeCredentials(credentialId, hashes);

            browserNavigateTo(handySigUrlCredentials);
            appendLog("\nPlease enter your 2nd factor\n");

            manualResetEvent.WaitOne();
            if (ct.IsCancellationRequested)
            {
                return false;
            }
            #endregion

            #region GetSAD - oauth2/token (API Call)

            var sad = boxClient.GetSad(code);
            if (sad == null)
            {
                appendLog("error on getSad");
                return false;
            }

            string sad_code = sad.access_token;
            appendLog("\nSad Code: " + sad_code);
            #endregion

            // Get Hashes (for JSON Request)
            hashes = getHashes(true);

            #region SignHash - signatures/signHash (API Call)

            var signedHashObj = boxClient.SignHash(sad_code, credentialId, access_token, hashes);
            if (ct.IsCancellationRequested)
            {
                return false;
            }

            if (signedHashObj == null)
            {
                appendLog("error on SignHash");
                return false;
            }

            foreach (string signature in signedHashObj.signatures)
            {
                appendLog("\nSignature: " + signature);
            }

            #endregion

            #region Insert signature into the documents
            byte[][] finishedDocuments = new byte[documents.Length][];
            for (int i = 0; i < documents.Length; i++)
            {
                finishedDocuments[i] = addSignatureToPDF(credentialId, Convert.FromBase64String(signedHashObj.signatures[0]), documents[i].signedAttributesBytes,
                    Convert.FromBase64String(certificates[0]), Convert.FromBase64String(certificates[1]), Convert.FromBase64String(certificates[2]), documents[i].preparedPdf, documents[i].startZeroBytes + 1);
            }

            #endregion

            // Save the PDF to disk
            savePDFs(finishedDocuments);

            appendLog("\nFinished!\nThe document(s) were saved in the same folder with the prefix \"signed_\".");
            return true;
        }

        /// <summary>
        /// Creates the signedAttributes object needed for clientData in oauth2/authorize
        /// </summary>
        /// <param name="certificates"></param>
        private void createSignedAttributes(string[] certificates) 
        {
            CmsSignedAttributes signedAttributes = new CmsSignedAttributes();

            for (int i = 0; i<documents.Length; i++)
            {
                byte[] signedAttributesBytes = signedAttributes.GetPAdES(documents[i].preparedPdfNo0s, certificates[0], false);
                string signedAttributesB64 = Convert.ToBase64String(signedAttributesBytes);
                byte[] signedAttributesHashBytes = SHA256.Create().ComputeHash(signedAttributesBytes);
                string signedAttributesHashB64 = Convert.ToBase64String(signedAttributesHashBytes);
                documents[i].signedAttributesBytes = signedAttributesBytes;
                documents[i].hash = signedAttributesHashB64;
            }
        }

    /// <summary>
    /// Stores the signed documents in the finishedDocuments array into their containing folder.
    /// </summary>
    /// <param name="finishedDocuments"></param>
    private void savePDFs(byte[][] finishedDocuments)
        {
            for (int i = 0; i < finishedDocuments.Length; i++)
            {
                string path = getPath(i);
                File.WriteAllBytes(path, finishedDocuments[i]);
            }
        }

        /// <summary>
        /// Returns the path to the folder the document at index (i) is stored in.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string getPath(int index)
        {
            return Path.GetDirectoryName(documents[index].path) + "\\signed_" + documents[index].filename;
        }

        /// <summary>
        /// Stores the prepared pdf data once with and without zeroes into the documents dictionary.
        /// </summary>
        private void GetDocsWithoutZeroes()
        {
            for (int i = 0; i < documents.Length; i++)
            {
                byte[] preparedPdf = File.ReadAllBytes(documents[i].path);
                documents[i].preparedPdf = preparedPdf;
                byte[] preparedPdfNo0s = preparedPdf;
                long a = 0;
                long b = (long)documents[i].startZeroBytes;
                long c = (long)documents[i].endZeroBytes;
                long d = preparedPdfNo0s.Length - c;
                byte[] signData = new byte[b + d];
                Array.Copy(preparedPdfNo0s, a, signData, 0, b);
                Array.Copy(preparedPdfNo0s, c, signData, b, d);
                
                File.WriteAllBytes("signdata.pdf", signData);
                
                if (signData == null)
                {
                    documents = null;
                    return;
                }

                documents[i].preparedPdfNo0s = signData;
            }
        }

        /// <summary>
        /// Returns the hashed signedAttributes of each document in the documents dictionary array.
        /// </summary>
        /// <param name="arrayForm"></param>
        /// <returns></returns>
        private string getHashes(bool arrayForm)
        {
            string hashes = "";
            for (int i = 0; i < documents.Length; i++)
            {
                if (arrayForm)
                {
                    hashes += $"\"{HttpUtility.UrlEncode(documents[i].hash)}\"";
                }
                else
                {
                    hashes += $"{HttpUtility.UrlEncode(documents[i].hash)}";
                }
                if (i < documents.Length - 1)
                {
                    hashes += ",";
                }
            }
            return hashes;
        }

        /// <summary>
        /// Continues the execution of the Thread working on Execute(). Callable from outside the method.
        /// </summary>
        /// <param name="code">The code extracted from the uri after the ID Austria/HandySignatur:AT signing process</param>
        public void continueExcecution(string code)
        {
            this.code = code;
            try
            {
                manualResetEvent.Set();
                manualResetEvent.Reset();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Navigates the Windows Forms Application webbrowser from outside the file to a link.
        /// </summary>
        /// <param name="url"></param>
        private void browserNavigateTo(string url)
        {
            browser.Invoke(new Action(() =>
            {
                browser.Source = new Uri(url);
            }));
        }

        /// <summary>
        /// Appends a log to the Windows Forms Application Log-Window.
        /// </summary>
        /// <param name="msg"></param>
        private void appendLog(string msg)
        {
            loggingBox.Invoke(new Action(() =>
            {
                loggingBox.AppendText(msg + "\n");
                loggingBox.SelectionStart = loggingBox.Text.Length;
                loggingBox.ScrollToCaret();
            }));
        }

        /// <summary>
        /// Adds the signature to the prepared PDF.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="signatureBytes"></param>
        /// <param name="signedAttributes"></param>
        /// <param name="sigCertBytes"></param>
        /// <param name="intermidiate"></param>
        /// <param name="root"></param>
        /// <param name="preparedPdfDataBase64"></param>
        /// <param name="pkcs7Offset"></param>
        /// <returns>the PDF data with the signature as byte[]</returns>
        private byte[] addSignatureToPDF(string sessionId, byte[] signatureBytes, byte[] signedAttributes, byte[] sigCertBytes, byte[] intermidiate, byte[] root, byte[] preparedPdfDataBase64, long pkcs7Offset)
        {
            byte[] finalSig = CreateCmsSignature(sessionId, signatureBytes, signedAttributes, sigCertBytes, intermidiate, root);
            byte[] hexBytes = ToHexArray(finalSig);

            Array.Copy(hexBytes, 0, preparedPdfDataBase64, (int)pkcs7Offset, hexBytes.Length);
            return preparedPdfDataBase64;
        }

        /// <summary>
        /// Creates the CMS Signature
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="signature"></param>
        /// <param name="signedAttributes"></param>
        /// <param name="sigCertBytes"></param>
        /// <param name="intermediate"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private byte[] CreateCmsSignature(string ticket, byte[] signature, byte[] signedAttributes, byte[] sigCertBytes, byte[] intermediate, byte[] root)
        {
            byte[] ContentBlobNull = null;

            var cms = new CmsSigBuilder(ticket, ContentBlobNull, true);

            signature = DecodeASN1Signature(signature);

            bool signer = cms.AddSigner(sigCertBytes, Hash.HASH_ALG.SHA256, signedAttributes, signature, null);

            cms.AddCertificate(intermediate);

            cms.AddCertificate(root);

            byte[] result = cms.GetCms();

            return result;

        }

        /// <summary>
        /// Parses an array toHex and returns it.
        /// </summary>
        /// <param name="finalSig"></param>
        /// <returns>Hex Array</returns>
        private static byte[] ToHexArray(byte[] finalSig)
        {
            string t = string.Concat(finalSig.Select(b => b.ToString("X2")).ToArray());

            byte[] xxxxx = Encoding.ASCII.GetBytes(t);
            return xxxxx;
        }

        /// <summary>
        /// Decodes the ASN1Signature.
        /// </summary>
        /// <param name="ASN1Signature"></param>
        /// <returns></returns>
        public static byte[] DecodeASN1Signature(byte[] ASN1Signature)
        {

            //byte[] der = Convert.FromBase64String(ASN1Signature);
            var xx = Asn1Sequence.FromByteArray(ASN1Signature);
            DerSequence ds = (DerSequence)xx;
            DerInteger r = (DerInteger)ds[0];
            DerInteger s = (DerInteger)ds[1];
            BigInteger rb = r.Value;
            BigInteger sb = s.Value;
            byte[] rbytes = rb.ToByteArray();
            byte[] sbytes = sb.ToByteArray();

            if (rbytes.Length > 32)
            {
                rbytes = rbytes.Skip(1).ToArray();
            }

            if (sbytes.Length > 32)
            {
                sbytes = sbytes.Skip(1).ToArray();
            }

            byte[] rsBytes64 = rbytes.Concat(sbytes).ToArray();

            return rsBytes64;
        }

    }

}
