using System.Reflection;

namespace ofreact
{
    public interface IElementMethodInvoker
    {
        /// <summary>
        /// Indicates that element binder should not throw if it couldn't find an <see cref="IElementMethodParameterProvider"/> for a parameter of the method in question, and use null instead.
        /// </summary>
        bool AllowUnknownParameters { get; }

        void Initialize(MethodInfo method, IElementMethodParameterProvider[] parameterProviders);
        void Invoke(ofElement element);
    }
}