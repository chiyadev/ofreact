using System.Reflection;

namespace ofreact
{
    public interface IElementFieldBinder
    {
        void Initialize(FieldInfo field);
        void Bind(ofElement element);
    }
}