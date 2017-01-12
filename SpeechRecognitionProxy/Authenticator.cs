using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpeechRecognitionProxy
{
    public class Authenticator
    {
        public static readonly string AccessUri = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        private string apiKey;
        private string accessToken;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public async Task<string> Authenticate(string apiKey)
        {
            this.apiKey = apiKey;

            return await HttpPost(AccessUri, this.apiKey);
        }

        public string GetAccessToken()
        {
            return this.accessToken;
        }

        private async Task<string> HttpPost(string accessUri, string apiKey)
        {
            string retVal = string.Empty;
            // Prepare OAuth request 
            WebRequest request = WebRequest.Create(accessUri);
            request.Method = "POST";
            //webRequest.ContentLength = 0;
            request.Headers["Ocp-Apim-Subscription-Key"] = apiKey;

            using (WebResponse response = await request.GetResponseAsync())
            using (Stream resStream = response.GetResponseStream())
            using (StreamReader read = new StreamReader(resStream))
            {
                int count = (int)response.ContentLength;
                int offset = 0;
                Byte[] buf = new byte[count];
                do
                {
                    int n = resStream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                    retVal = Encoding.ASCII.GetString(buf, 0, buf.Length);
                } while (count > 0);

                read.Dispose();
            }

            return retVal;
        }
    }
}
