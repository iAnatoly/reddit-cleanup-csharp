namespace reddit_cleanup
{
    using NLog;

    /// <summary>
    /// Logging abstraction.
    /// </summary>
    public static class Log
    {
        private static Logger Logger = LogManager.GetLogger("reddit_cleanup");
        public static void Write(string message)
        {
            Logger.Info(message);
        }
    }
}
