using System.Reflection;

namespace ofreact
{
    public interface IElementFieldBinder
    {
        FieldInfo Field { get; }

        void Initialize(FieldInfo field);
        object GetValue(ofElement element);
    }
}