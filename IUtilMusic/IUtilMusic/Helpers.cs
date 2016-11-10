using System;


namespace IUtilMusic
{
    /// <summary>
    /// Helper class for various things
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Get the current time millis in a similar way than System.currentTimeMillis() in Java
        /// </summary>
        /// <returns>Current time millis</returns>
        public static long CurrentTimeMillis()
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

    }
}
