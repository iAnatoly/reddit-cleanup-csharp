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
                var task = MainAsync();
                Task.WaitAll(task);
                Log.Write("Done.");
            }
            catch (Exception ex)
            {
                Log.Write(ex.ToString());
            }
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

            var me = await Web.InvokeWebAPIRequest(HttpMethod.Get, auth, "https://oauth.reddit.com/api/v1/me");
            var name = (string)me.SelectToken("$.name");
            Log.Write($"Logged in as {name}. Loading comments and posts...");

            // See https://www.reddit.com/dev/api/
            var comments = await Web.InvokeWebAPIRequest(HttpMethod.Get, auth, $"https://oauth.reddit.com/user/{Configuration.ContactLogin}/comments");
            var commentsData = comments.SelectTokens("$..data.children[*].data");
            Log.Write($"Found {commentsData.Count()} comments; looking for comments created {Configuration.KeepCommentsFor} ago");
            await DeleteChildren(commentsData, auth, "t1_", DateTime.Now - Configuration.KeepCommentsFor);

            var posts = await Web.InvokeWebAPIRequest(HttpMethod.Get, auth, $"https://oauth.reddit.com/user/{Configuration.ContactLogin}/submitted");
            var postsData = posts.SelectTokens("$..data.children[*].data");
            Log.Write($"Found {postsData.Count()} posts;  looking for posts created {Configuration.KeepPostsFor} ago");
            await DeleteChildren(postsData, auth, "t3_", DateTime.Now - Configuration.KeepPostsFor);
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
                    Log.Write($"Removed entity: {id}");
                }
                else
                {
                    Log.Write($"Skipping entity because it was created on {created}");
                }
            });
        }
    }
}