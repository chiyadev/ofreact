using System.Reflection;

namespace ofreact
{
    public interface IElementMethodParameterProvider
    {
        ParameterInfo Parameter { get; }

        void Initialize(ParameterInfo parameter);
        object GetValue(ofElement element);
    }
}