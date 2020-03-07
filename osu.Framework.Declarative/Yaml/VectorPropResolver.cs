using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using ofreact;
using osuTK;
using YamlDotNet.RepresentationModel;

namespace osu.Framework.Declarative.Yaml
{
    public class VectorPropResolver : IPropResolver
    {
        public IPropProvider Resolve(ComponentBuilderContext context, PropTypeInfo prop, ElementBuilder element, YamlNode node)
        {
            if (prop.Type == typeof(Vector2))
                return new Provider2(ParseVector(node, 2));

            if (prop.Type == typeof(Vector3))
                return new Provider3(ParseVector(node, 3));

            if (prop.Type == typeof(Vector4))
                return new Provider4(ParseVector(node, 4));

            return null;
        }

        static float[] ParseVector(YamlNode s, int count)
        {
            switch (s)
            {
                case YamlScalarNode scalar:
                    var parts = scalar.Value.Split(',');

                    if (parts.Length == 1) // scalar shorthand
                    {
                        var a = new float[count];
                        var n = ParseNumber(s, parts[0]);

                        for (var i = 0; i < count; i++)
                            a[i] = n;

                        return a;
                    }

                    if (parts.Length == count)
                    {
                        var a = new float[count];

                        for (var i = 0; i < count; i++)
                            a[i] = ParseNumber(s, parts[i]);

                        return a;
                    }

                    break;

                case YamlSequenceNode sequence when sequence.Children.Count == count:
                {
                    var a = new float[count];

                    for (var i = 0; i < count; i++)
                        a[i] = ParseNumber(sequence[i]);

                    return a;
                }
            }

            throw new YamlComponentException($"Must be a scalar or sequence containing {count} components.", s);
        }

        static float ParseNumber(YamlNode n)
        {
            if (n is YamlScalarNode scalar)
                return ParseNumber(scalar, scalar.Value);

            throw new YamlComponentException("Must be a scalar.", n);
        }

        static float ParseNumber(YamlNode n, string s)
        {
            if (float.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var value))
                return value;

            throw new YamlComponentException($"Cannot convert '{s}' to number.", n);
        }

        sealed class Provider2 : IPropProvider
        {
            readonly float[] _v;

            public Provider2(float[] v)
            {
                _v = v;
            }

            static readonly ConstructorInfo _ctor = typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) });

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.New(_ctor,
                    Expression.Constant(_v[0]),
                    Expression.Constant(_v[1]));
        }

        sealed class Provider3 : IPropProvider
        {
            readonly float[] _v;

            public Provider3(float[] v)
            {
                _v = v;
            }

            static readonly ConstructorInfo _ctor = typeof(Vector3).GetConstructor(new[] { typeof(float), typeof(float), typeof(float) });

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.New(_ctor,
                    Expression.Constant(_v[0]),
                    Expression.Constant(_v[1]),
                    Expression.Constant(_v[2]));
        }

        sealed class Provider4 : IPropProvider
        {
            readonly float[] _v;

            public Provider4(float[] v)
            {
                _v = v;
            }

            static readonly ConstructorInfo _ctor = typeof(Vector4).GetConstructor(new[] { typeof(float), typeof(float), typeof(float), typeof(float) });

            public Expression GetValue(ComponentBuilderContext context)
                => Expression.New(_ctor,
                    Expression.Constant(_v[0]),
                    Expression.Constant(_v[1]),
                    Expression.Constant(_v[2]),
                    Expression.Constant(_v[3]));
        }
    }
}