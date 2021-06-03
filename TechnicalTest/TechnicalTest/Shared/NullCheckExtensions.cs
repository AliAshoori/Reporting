using System;

namespace TechnicalTest.Shared
{
    public static class NullCheckExtensions
    {
        public static T NotNull<T>(this T item, string message = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException(message ?? nameof(item));
            }

            return item;
        }
    }
}
