using System.Reflection;

namespace ofreact
{
    public interface IElementMethodArgumentProvider
    {
        ParameterInfo Parameter { get; }

        void Initialize(ParameterInfo parameter);
        object GetValue(ofElement element);
    }
}