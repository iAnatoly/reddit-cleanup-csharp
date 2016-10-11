namespace reddit_cleanup
{
    using System;
    using System.Configuration;

    class Configuration
    {
        public static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(30);

        public static readonly string AuthorizationServerEndpointAddress = "https://www.reddit.com/api/v1/access_token";

        public static readonly string ClientID = ConfigurationManager.AppSettings["ClientID"];
        public static readonly string ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];

        public static readonly string ContactLogin = ConfigurationManager.AppSettings["ContactLogin"];
        public static readonly string ContactPassword = ConfigurationManager.AppSettings["ContactPassword"];

        public static readonly TimeSpan KeepCommentsFor = TimeSpan.FromDays(14);
        public static readonly TimeSpan KeepPostsFor = TimeSpan.FromDays(14);

    }
}
