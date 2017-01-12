using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SpeechRecognitionProxy
{
    public class SpeechToText
    {
        string _authorizationToken;
        /// <summary>
        /// Authorization Token.
        /// </summary>
        public string AuthorizationToken { get { return _authorizationToken; } set { _authorizationToken = "Bearer " + value; } }

        public async Task<RootObject> Recognize(CancellationToken cancellationToken, byte[] fileBytes)
        {
            RootObject retVal = null;
            HttpClient client = null;
            HttpRequestMessage request = null;
            HttpClientHandler handler = null;
            CookieContainer cookieContainer = null;
            HttpResponseMessage response = null;
            
            string uri = "https://speech.platform.bing.com/recognize?scenarios=catsearch&appid=f84e364c-ec34-4773-a783-73707bd9a585&locale=en-US&device.os=wp7&version=3.0&format=json&requestid=1d4b6030-8099-11e0-91e4-0800200c9a66&instanceid=2d4b6030-9099-11e0-91e4-0800200c9a66";

            try
            {
                cookieContainer = new CookieContainer();
                handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                client = new HttpClient(handler);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "audio / wav; samplerate = 16000");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", this.AuthorizationToken);
                request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = new ByteArrayContent(fileBytes);
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var rawJson = await response.Content.ReadAsStringAsync();
                    retVal = JsonConvert.DeserializeObject<RootObject>(rawJson);
                }
                else
                {
                    //this.Error(new GenericEventArgs<Exception>(new Exception(String.Format("Service returned {0}", responseMessage.Result.StatusCode))));
                }
            }
            catch (Exception e)
            {
                //this.Error(new GenericEventArgs<Exception>(e.GetBaseException()));
            }
            finally
            {
                client?.Dispose();
                request?.Dispose();
                response?.Dispose();
                handler?.Dispose();
            }

            return retVal;

            //var httpTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            //var saveTask = httpTask.ContinueWith(
            //    async (responseMessage, token) =>
            //    {
            //        try
            //        {
            //            if (responseMessage.IsCompleted && responseMessage.Result != null && responseMessage.Result.IsSuccessStatusCode)
            //            {
            //                var b = await responseMessage.Result.Content.ReadAsStringAsync().ConfigureAwait(false);
            //                var retVal = JsonConvert.DeserializeObject<RootObject>(b);
            //            }
            //            else
            //            {
            //                //this.Error(new GenericEventArgs<Exception>(new Exception(String.Format("Service returned {0}", responseMessage.Result.StatusCode))));
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            //this.Error(new GenericEventArgs<Exception>(e.GetBaseException()));
            //        }
            //        finally
            //        {
            //            //responseMessage.Dispose();
            //            request.Dispose();
            //            client.Dispose();
            //            handler.Dispose();
            //        }
            //    },
            //    TaskContinuationOptions.AttachedToParent,
            //    cancellationToken);
        }
    }
}
