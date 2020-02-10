using System.Diagnostics;

namespace ofreact
{
    static class InternalConstants
    {
        public static bool ValidateHooks => Debugger.IsAttached;
        public static bool ValidateNodeBind => Debugger.IsAttached;
    }
}