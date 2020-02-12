using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ofreact
{
    static class InternalConstants
    {
        public static bool ValidateHooks => Debugger.IsAttached;

        public static bool IsEmitAvailable { get; }

        static InternalConstants()
        {
            try
            {
                IsEmitAvailable = Expression.Lambda<Func<bool>>(Expression.Constant(true)).Compile()();
            }
            catch
            {
                IsEmitAvailable = false;
            }
        }
    }
}