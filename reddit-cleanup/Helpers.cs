namespace reddit_cleanup
{
    using System;

    /// <summary>
    /// Static helpers.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// The unix time stamp to date-time translator.
        /// </summary>
        /// <param name="unixTimeStamp">
        /// The unix time stamp.
        /// </param>
        /// <returns>
        /// The translated date-time.
        /// </returns>
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }


    }
}
