using NUnit.Framework;
using osu.Framework.Declarative.Yaml;
using static ofreact.Hooks;

namespace ofreact.Tests.Yaml
{
    /// <summary>
    /// Element with unambiguous ctors.
    /// </summary>
    public class UnambiguousCtorElement : ofComponent
    {
        public class Context
        {
            public Type? TheType { get; set; }
        }

        public enum Type
        {
            String,
            Integer,
            Boolean
        }

        [Prop] readonly Type _type;

        public UnambiguousCtorElement(string str) : base(str)
        {
            Assert.That(str, Is.EqualTo("my value"));

            _type = Type.String;
        }

        public UnambiguousCtorElement(int integer) : base(integer)
        {
            Assert.That(integer, Is.EqualTo(765));

            _type = Type.Integer;
        }

        public UnambiguousCtorElement(bool boolean) : base(boolean)
        {
            Assert.That(boolean, Is.EqualTo(true));

            _type = Type.Boolean;
        }

        protected override ofElement Render()
        {
            var context = UseContext<Context>();

            context.TheType = _type;

            return null;
        }
    }

    /// <summary>
    /// Element with ambiguous ctors depending on the value.
    /// </summary>
    public class PartlyAmbiguousCtorElement : ofComponent
    {
        public class Context
        {
            public Type? TheType { get; set; }
        }

        public enum Type
        {
            String,
            Integer
        }

        [Prop] readonly Type _type;

        public PartlyAmbiguousCtorElement(string value) : base(value)
        {
            Assert.That(value, Is.EqualTo("my value"));

            _type = Type.String;
        }

        public PartlyAmbiguousCtorElement(int value) : base(value)
        {
            Assert.Fail($"{value} should be ambiguous.");
        }

        protected override ofElement Render()
        {
            var context = UseContext<Context>();

            context.TheType = _type;

            return null;
        }
    }

    /// <summary>
    /// Element with a private ctor.
    /// </summary>
    public class PrivateCtorElement : ofElement
    {
        PrivateCtorElement() { }
    }

    /// <summary>
    /// Element with multiple possible ctors that are preferred depending on the number of matches.
    /// </summary>
    public class MultiCtorElement : ofComponent
    {
        public class Context
        {
            public int Params { get; set; }
        }

        [Prop] readonly int _params;

        public MultiCtorElement(string value1) : base(value1)
        {
            _params = 1;
        }

        public MultiCtorElement(string value1, string value2 = default, string value3 = default) : base(value1)
        {
            _params = 3;
        }

        public MultiCtorElement(string value1, string value2 = default, string value3 = default, string value4 = default, string value5 = default) : base(value1)
        {
            _params = 5;
        }

        protected override ofElement Render()
        {
            var context = UseContext<Context>();

            context.Params = _params;

            return null;
        }
    }

    public class ConstructorMatchingTests
    {
        [Test]
        public void UnambiguousCtor()
        {
            using var node = new ofRootNode();

            var context = new UnambiguousCtorElement.Context();

            node.RenderElement(new ofContext<UnambiguousCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 1, document: @"
render:
  UnambiguousCtorElement:
    str: my value")
            });

            Assert.That(context.TheType, Is.EqualTo(UnambiguousCtorElement.Type.String));

            node.RenderElement(new ofContext<UnambiguousCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 2, document: @"
render:
  UnambiguousCtorElement:
    integer: 765")
            });

            Assert.That(context.TheType, Is.EqualTo(UnambiguousCtorElement.Type.Integer));

            node.RenderElement(new ofContext<UnambiguousCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 3, document: @"
render:
  UnambiguousCtorElement:
    boolean: true")
            });

            Assert.That(context.TheType, Is.EqualTo(UnambiguousCtorElement.Type.Boolean));
        }

        [Test]
        public void PartlyAmbiguousCtor()
        {
            var node = new ofRootNode();

            var context = new PartlyAmbiguousCtorElement.Context();

            // "my value" is unambiguously a string
            node.RenderElement(new ofContext<PartlyAmbiguousCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 1, document: @"
render:
  PartlyAmbiguousCtorElement:
    value: my value")
            });

            Assert.That(context.TheType, Is.EqualTo(PartlyAmbiguousCtorElement.Type.String));

            // "765" can be parsed as string OR integer, so it is ambiguous
            Assert.That(() => node.RenderElement(new ofContext<PartlyAmbiguousCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 2, document: @"
render:
  PartlyAmbiguousCtorElement:
    value: 765")
            }), Throws.InstanceOf<YamlComponentException>());
        }

        [Test]
        public void PrivateCtor()
        {
            var node = new ofRootNode();

            Assert.That(() => node.RenderElement(new ofYamlComponent(@"
render:
  PrivateCtorElement:")), Throws.InstanceOf<YamlComponentException>());
        }

        [Test]
        public void MultiCtor()
        {
            var node = new ofRootNode();

            var context = new MultiCtorElement.Context();

            node.RenderElement(new ofContext<MultiCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 1, document: @"
render:
  MultiCtorElement:
    value1: 1")
            });

            // exact match - 1 param
            Assert.That(context.Params, Is.EqualTo(1));

            node.RenderElement(new ofContext<MultiCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 2, document: @"
render:
  MultiCtorElement:
    value1: 1
    value2: 2")
            });

            // closest match - 2 param with 1 optional (due to param 2 excess)
            Assert.That(context.Params, Is.EqualTo(3));

            node.RenderElement(new ofContext<MultiCtorElement.Context>(value: context)
            {
                new ofYamlComponent(key: 3, document: @"
render:
  MultiCtorElement:
    value1: 1
    value2: 2
    value3: 3
    value4: 4")
            });

            // closest match - 4 param with 1 optional (due to param 4 excess)
            Assert.That(context.Params, Is.EqualTo(5));
        }
    }
}