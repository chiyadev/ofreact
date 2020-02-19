using NUnit.Framework;
using ofreact.Yaml;
using static ofreact.Hooks;

namespace ofreact.Tests.Yaml
{
    public class NullablePropElement : ofComponent
    {
        public class Context
        {
            public int Value { get; set; }
        }

        [Prop] readonly int? _nullableInt;

        public NullablePropElement(int? nullableInt)
        {
            _nullableInt = nullableInt;
        }

        protected override ofElement Render()
        {
            var context = UseContext<Context>();

            Assert.That(_nullableInt, Is.Not.Null);

            context.Value = _nullableInt.Value;

            return null;
        }
    }

    public class NullableEnumPropElement : ofComponent
    {
        public class Context
        {
            public Enum Value { get; set; }
        }

        public enum Enum
        {
            Item1 = 1
        }

        [Prop] readonly Enum? _nullableEnum;

        public NullableEnumPropElement(Enum? nullableEnum)
        {
            _nullableEnum = nullableEnum;
        }

        protected override ofElement Render()
        {
            var context = UseContext<Context>();

            Assert.That(_nullableEnum, Is.Not.Null);

            context.Value = _nullableEnum.Value;

            return null;
        }
    }

    public class NullablePropTests
    {
        [Test]
        public void NullableInt()
        {
            using var node = new ofRootNode();

            var context = new NullablePropElement.Context();

            node.RenderElement(new ofContext<NullablePropElement.Context>(value: context)
            {
                new ofYamlComponent(@"
render:
  NullablePropElement:
    nullableInt: 10")
            });

            Assert.That(context.Value, Is.EqualTo(10));
        }

        [Test]
        public void NullableEnum()
        {
            using var node = new ofRootNode();

            var context = new NullableEnumPropElement.Context();

            node.RenderElement(new ofContext<NullableEnumPropElement.Context>(value: context)
            {
                new ofYamlComponent(@"
render:
  NullableEnumPropElement:
    nullableEnum: Item1")
            });

            Assert.That(context.Value, Is.EqualTo(NullableEnumPropElement.Enum.Item1));
        }
    }
}