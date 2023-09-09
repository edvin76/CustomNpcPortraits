using Kingmaker.Blueprints;
using System;

namespace DialogTest.Utilities
{
    internal static class Extensions
    {
        public static BlueprintGuid ToGUID(this string guid)
        {
            return new BlueprintGuid(Guid.Parse(guid));
        }
    }
}
