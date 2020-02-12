using System.Diagnostics;
using System.Linq.Expressions;

namespace ofreact
{
    static class InternalConstants
    {
        public static bool ValidateHooks => Debugger.IsAttached;
        public static bool ValidateNodeBind => Debugger.IsAttached;

        public static bool IsEmitAvailable
        {
            get
            {
                try
                {
                    Expression.Lambda(Expression.Constant(true)).Compile();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}