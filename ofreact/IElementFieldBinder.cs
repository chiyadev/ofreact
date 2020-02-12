using System.Reflection;

namespace ofreact
{
    public interface IElementFieldBinder
    {
        void Initialize(FieldInfo field);
        object GetValue(ofElement element);
    }
}