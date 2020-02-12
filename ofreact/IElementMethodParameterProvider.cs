using System.Reflection;

namespace ofreact
{
    public interface IElementMethodParameterProvider
    {
        void Initialize(ParameterInfo parameter);
        object GetValue(ofElement element);
    }
}