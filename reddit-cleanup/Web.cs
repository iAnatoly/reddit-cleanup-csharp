namespace reddit_cleanup
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    class Web
    {
        public static HttpRequestMessage CreateHttpClientRequest(string uri, HttpMethod method, AuthenticationHeaderValue auth, Dictionary<string, string> content = null)
        {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Authorization = auth;

            if (content != null) request.Content = new FormUrlEncodedContent(content);

            return request;
        }

        public static async Task<string> CreateTokenUsingHttpClientAsync()
        {
            var requestParameters = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "username", Configuration.ContactLogin },
                { "password", Configuration.ContactPassword }
            };

            var auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration.ClientID}:{Configuration.ClientSecret}")));

            var token = await InvokeWebAPIRequest(HttpMethod.Post, auth, "https://www.reddit.com/api/v1/access_token", requestParameters);
            var access_token = (string)token.SelectToken("access_token");

            return access_token;
        }

        public static async Task<JObject> InvokeWebAPIRequest(HttpMethod method, AuthenticationHeaderValue auth, string url, Dictionary<string, string> requestParameters = null)
        {
            using (var client = new HttpClient { Timeout = Configuration.HttpTimeout })
            {
                client.DefaultRequestHeaders.Add("USer-Agent", "Whatever");

                var request = CreateHttpClientRequest(url, method, auth, requestParameters);

                var response = await client.SendAsync(request);
                if (response.Content != null)
                {
                    await response.Content.LoadIntoBufferAsync();
                    var text = await response.Content.ReadAsStringAsync();

                    var token = JObject.Parse(text);
                    return token;
                }

                throw new Exception();
            }
        }

    }
}
