namespace MK.Data
{
    using System;
    using MK.Log;

    internal static class Extensions
    {
        public static void ThrowIfNotAssignable(this ILogger logger, Type type, Type baseType)
        {
            logger.Fatal(new Exception($"{type.FullName} is not assignable to {baseType.FullName}"));
        }
    }
}