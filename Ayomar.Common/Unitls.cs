using System;

namespace Ayomar.Common
{
    public static class Unitls
    {
        /// <summary>
        /// DateTime类型转DateTimeOffset
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeOffset DateTimeToDateTimeOffset(DateTime dateTime)
        {
            return dateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(dateTime);
        }
    }
}
