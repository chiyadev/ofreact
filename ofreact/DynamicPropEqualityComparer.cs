using System;
using System.Linq;

namespace ofreact
{
    /// <summary>
    /// Prop comparer that uses reflection.
    /// </summary>
    public class DynamicPropEqualityComparer : IPropEqualityComparer
    {
        public Func<ofElement, ofElement, bool> Equals { get; }

        public DynamicPropEqualityComparer(Type type)
        {
            var fields = type.GetAllFields().Where(f => f.IsDefined(typeof(PropAttribute), true)).ToArray();

            switch (fields.Length)
            {
                case 0:
                {
                    Equals = (x, y) => true;
                    break;
                }

                case 1:
                {
                    var field = fields[0];

                    Equals = (x, y) =>
                    {
                        var a = field.GetValue(x);
                        var b = field.GetValue(y);

                        return a != b && (a == null || !a.Equals(b));
                    };
                    break;
                }

                default:
                {
                    Equals = (x, y) =>
                    {
                        for (var i = 0; i < fields.Length; i++)
                        {
                            var field = fields[i];

                            var a = field.GetValue(x);
                            var b = field.GetValue(y);

                            if (a != b && (a == null || !a.Equals(b)))
                                return false;
                        }

                        return true;
                    };
                    break;
                }
            }
        }
    }
}