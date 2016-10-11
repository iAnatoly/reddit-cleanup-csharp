namespace reddit_cleanup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The entry point class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Executable entry point.
        /// </summary>
        static void Main()
        {
            try
            {
                MainAsync().Wait();
                Write("Done.");
            }
            catch (Exception ex)
            {
                Write(ex.ToString());
            }

            Write("Press any key to quit.");
            Console.ReadKey();
        }

        /// <summary>
        /// The main workflow implemnted as async process.
        /// </summary>
        /// <returns>
        /// The async task.
        /// </returns>
        private static async Task MainAsync()
        {
            var accessToken = await Web.CreateTokenUsingHttpClientAsync();
            var auth = new AuthenticationHeaderValue("bearer", accessToken);

            // See https://www.reddit.com/dev/api/
            var comments = await Web.InvokeWebAPIRequest(HttpMethod.Get, auth, $"https://oauth.reddit.com/user/{Configuration.ContactLogin}/comments");
            await DeleteChildren(comments.SelectTokens("$..data.children[*].data"), auth, "t1_", DateTime.Now - Configuration.KeepCommentsFor);

            var posts = await Web.InvokeWebAPIRequest(HttpMethod.Get, auth, $"https://oauth.reddit.com/user/{Configuration.ContactLogin}/submitted");
            await DeleteChildren(posts.SelectTokens("$..data.children[*].data"), auth, "t3_", DateTime.Now - Configuration.KeepPostsFor);


        }

        private static async Task DeleteChildren(IEnumerable<JToken> children, AuthenticationHeaderValue auth, string prefix, DateTime cutOffDate)
        {
            children.ToList().AsParallel().ForAll(async child => 
            {
                var created = Helpers.UnixTimeStampToDateTime((double)child.SelectToken("$.created_utc"));
                var id = (string)child.SelectToken("$.id");
                if (created < cutOffDate)
                {
                    await Web.InvokeWebAPIRequest(HttpMethod.Post, auth, "https://oauth.reddit.com/api/del", new Dictionary<string, string> { { "id", prefix + id } });
                }
            });
        }

        /// <summary>
        /// Simple debug output to be re-implemented later.
        /// </summary>
        /// <param name="message">
        /// The message to log.
        /// </param>
        private static void Write(string message)
        {
            Console.WriteLine(message);
        }

    }
}