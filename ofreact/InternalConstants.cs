using System.Diagnostics;
using System.Linq.Expressions;

namespace ofreact
{
    static class InternalConstants
    {
        public static bool ValidateHooks => Debugger.IsAttached;
        public static bool ValidateNodeBind => Debugger.IsAttached;

        public static bool IsEmitAvailable { get; }

        static InternalConstants()
        {
            try
            {
                Expression.Lambda(Expression.Constant(true)).Compile();

                IsEmitAvailable = true;
            }
            catch
            {
                IsEmitAvailable = false;
            }
        }
    }
}