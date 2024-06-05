using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;


namespace ExampleClient
{
    public class HashSignHelper
    {
        /// <summary>
        /// A REST Client used to ease the interaction with a SigBox with Hash Signing
        /// </summary>

        private RestClient client;
        private string accountToken;
        private string language;
        private int numSignatures = 1;
        private string clientId;
        private string clientSecret;
        private string redirectUri;
        private string baseUrl;

        /// <summary>
        /// Initializes the SignaturboxClient
        /// </summary>
        /// <param name="baseUrl">The base URL of the SigBox Server</param>
        /// <param name="apiKey">Your API-Key used for Authentication on the SigBox Server</param>
        public HashSignHelper(string baseUrl, string redirectUri, string accountToken, string clientId, string clientSecret, int numSignatures, string language = "de")
        {
            client = new RestClient(baseUrl);
            this.baseUrl = baseUrl;
            this.clientId = clientId;
            this.accountToken = accountToken;
            this.language = language;
            this.numSignatures = numSignatures;
            this.clientSecret = clientSecret;
            this.redirectUri = redirectUri;
        }

        public CertificateInfo getCertificateInfo(string credentialId, string accessToken)
        {
            RestRequest request = GetBaseRequest("credentials/info", Method.Post);


            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody("{\"credentialID\":\"" + credentialId + "\",\"certificates\":\"chain\", \"certInfo\": true, \"authInfo\": true}");

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<CertificateInfo>(response.Content);
            }
            return null;
        }

        public string AuthorizeService()
        {
            Dictionary<string, string> handySigParams = new Dictionary<string, string>();
            handySigParams.Add("response_type", "code");
            handySigParams.Add("redirect_uri", redirectUri);
            handySigParams.Add("client_id", clientId);
            handySigParams.Add("scope", "service");
            handySigParams.Add("state", "RANDOMSTATE");
            handySigParams.Add("lang", language);
            handySigParams.Add("account_token", accountToken);

            return buildHandySigUrl("oauth2/authorize", handySigParams);
        }

        public string buildHandySigUrl(string url, Dictionary<string, string> parameters)
        {
            string retUrl = Combine(baseUrl, url);

            if (parameters.Count == 0)
            {
                return retUrl;
            }
            retUrl += "?";
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                retUrl += entry.Key + "=" + entry.Value + "&";
            }
            return retUrl.Remove(retUrl.Length - 1);
        }

        public static string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format("{0}/{1}", uri1, uri2);
        }

        public Tokens GetTokens(string code)
        {
            RestRequest request = GetBaseRequest("oauth2/token", Method.Post);
            request.AddParameter("redirect_uri", redirectUri);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("client_id", clientId);
            request.AddParameter("code", code);
            request.AddParameter("client_secret", clientSecret);

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Tokens>(response.Content);
            }
            return null;
        }

        public string DeleteAccessToken(string accessToken)
        {
            RestRequest request = GetBaseRequest("oauth2/revoke", Method.Post);
            request.AddParameter("token", accessToken);
            request.AddParameter("token_type_hint", "access_token");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return response.Content;
            }
            return null;
        }

        public string RefreshAccessToken(string refreshToken)
        {
            RestRequest request = GetBaseRequest("oauth2/token", Method.Post);
            request.AddParameter("token", refreshToken);
            request.AddParameter("token_type_hint", "redirectToken");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return response.Content;
            }
            return null;
        }

        public CredentialList GetCredentialsList(string accessToken)
        {
            RestRequest request = GetBaseRequest("credentials/list", Method.Post);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody("{}");

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<CredentialList>(response.Content);
            }
            return null;

        }

        public string GetCredentialsInfo(string credentialsId)
        {
            RestRequest request = GetBaseRequest("credentials/info", Method.Post);
            request.AddHeader("Authorization", "Bearer " + credentialsId);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return response.Content;
            }
            return null;
        }

        public Sad GetSad(string code)
        {
            RestRequest request = GetBaseRequest("oauth2/token", Method.Post);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("redirect_uri", redirectUri);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Sad>(response.Content);
            }
            return null;
        }

        public Signatures SignHash(string sad, string credentialId, string accessToken, string hashes)
        {
            RestRequest request = GetBaseRequest("signatures/signHash", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            request.AddBody("{\"credentialID\":\"" + credentialId + "\",\"SAD\":\"" + sad + "\",\"hash\":[" + hashes + "]}");

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<Signatures>(response.Content);
            }
            return null;
        }

        public string AuthorizeCredentials(string credentialId, string hash)
        {
            Dictionary<string, string> handySigParams = new Dictionary<string, string>();
            handySigParams.Add("response_type", "code");
            handySigParams.Add("redirect_uri", redirectUri);
            handySigParams.Add("client_id", clientId);
            handySigParams.Add("scope", "credential");
            handySigParams.Add("state", "VXAgdG8gMjU1IGJ5dGVzIG9mIGFyYml0cmFyeSBkYXRhIGZyb20gdGhlIHNpZ25hdHVyZSBhcHBsaWNhdGlvbiB0aGF0IHdpbGwgYmUgcGFzc2VkIGJhY2sgdG8gdGhlIHJlZGlyZWN0IFVSSS4gVGhlIHVzZSBpcyBSRUNPTU1FTkRFRCBmb3IgcHJldmVudGluZyBjcm9zcy1zaXRlIHJlcXVlc3QgZm9yZ2VyeS4");
            handySigParams.Add("lang", language);
            handySigParams.Add("numSignatures", numSignatures.ToString());
            handySigParams.Add("hash", hash);
            handySigParams.Add("credentialID", credentialId);
            string handySigUrl = buildHandySigUrl("oauth2/authorize", handySigParams);
            return handySigUrl;
        }

        /// <summary>
        /// Creates a base HTTP-Request and adds your API-Key to the requestheader
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private RestRequest GetBaseRequest(string resource, Method method)
        {
            RestRequest request = new RestRequest(resource, method);
            return request;
        }

        public string GetCodeFromUrlQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return "";
            }
            string[] parameters = query.Split('&');
            foreach (string param in parameters)
            {
                string[] paramSplit = param.Split('=');
                if (paramSplit[0].Equals("code"))
                {
                    return paramSplit[1];
                }
            }
            return "";
        }
    }

    public class Tokens
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class CredentialList
    {
        public string[] credentialIDs { get; set; }
    }

    public class Sad
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class CertificateInfo
    {
        public Key key { get; set; }
        public Cert cert { get; set; }
        public string description { get; set; }
        public string authMode { get; set; }
        public string SCAL { get; set; }
        public int multisign { get; set; }
        public string lang { get; set; }
    
        public class Key
        {
            public string[] algo { get; set; }
            public string status { get; set; }
            public int len { get; set; }
            public string curve { get; set; }
        }

        public class Cert
        {
            public string status { get; set; }
            public string[] certificates { get; set; }
            public string issuerDN { get; set; }
            public string serialNumber { get; set; }
            public string subjectDN { get; set; }
            public string validFrom { get; set; }
            public string validTo { get; set; }
        }
    }

    public class Signatures
    {
        public string[] signatures{ get; set; }

    }


}

