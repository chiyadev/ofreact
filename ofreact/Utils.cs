using System.Runtime.CompilerServices;

namespace ofreact
{
    static class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ObjectsEqual(object[] a, object[] b)
        {
            if (a == b)
                return true;

            if (a == null || b == null || a.Length != b.Length)
                return false;

            for (var i = 0; i < a.Length; i++)
            {
                var x = a[i];
                var y = b[i];

                if (x != y && (x == null || !x.Equals(y)))
                    return false;
            }

            return true;
        }
    }
}