using NUnit.Framework;
using osu.Framework.Declarative.Yaml;

namespace ofreact.Tests.Yaml
{
    public class StringArrayPropElement : ofComponent
    {
        [Prop] readonly string[] _values;

        public StringArrayPropElement(ElementKey key = default, string[] values = default) : base(key)
        {
            _values = values;
        }

        protected override ofElement Render()
        {
            Assert.That(_values, Is.EqualTo(new[]
            {
                "value 1",
                "value 2",
                ""
            }));

            return null;
        }
    }

    public class StringJaggedArrayPropElement : ofComponent
    {
        [Prop] readonly string[][] _values;

        public StringJaggedArrayPropElement(ElementKey key = default, string[][] values = default) : base(key)
        {
            _values = values;
        }

        protected override ofElement Render()
        {
            Assert.That(_values, Is.EqualTo(new[]
            {
                new[] { "value 1 1", "value 1 2" },
                new[] { "value 2 1", "value 2 2", "value 2 3" },
                new[] { "" }
            }));

            return null;
        }
    }

    public class ComplexNullableJaggedArrayPropElement : ofComponent
    {
        [Prop] readonly int?[][][] _values;

        public ComplexNullableJaggedArrayPropElement(ElementKey key = default, int?[][][] values = default) : base(key)
        {
            _values = values;
        }

        protected override ofElement Render()
        {
            Assert.That(_values, Is.EqualTo(new[]
            {
                new[]
                {
                    new[] { 1, 2, 3 },
                    new[] { 4, 5 }
                },
                new[] { new[] { 6 } }
            }));

            return null;
        }
    }

    public class ArrayPropTests
    {
        [Test]
        public void StringArrayProp()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  StringArrayPropElement:
    values:
      - value 1
      - value 2
      -"));
        }

        [Test]
        public void StringJaggedArrayProp()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  StringJaggedArrayPropElement:
    values:
      - - value 1 1
        - value 1 2
      - - value 2 1
        - value 2 2
        - value 2 3
      -"));
        }

        [Test]
        public void ComplexNullableJaggedArrayProp()
        {
            using var node = new ofRootNode();

            node.RenderElement(new ofYamlComponent(@"
render:
  ComplexNullableJaggedArrayPropElement:
    values:
      - - - 1
          - 2
          - 3
        - - 4
          - 5
      - 6"));
        }
    }
}