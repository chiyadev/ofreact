using System;
using NUnit.Framework;

namespace ofreact.Tests
{
    public class UseRefTests
    {
        class Element1 : ofComponent
        {
            public static bool Alt;

            static object _value;

            protected override ofElement Render()
            {
                if (Alt)
                    _value = UseRef<int>().Current;

                else
                    _value = UseRef<double>().Current;

                return null;
            }
        }

        [Test]
        public void MismatchingType()
        {
            var node = new ofRootNode();

            node.RenderElement(new Element1());

            Element1.Alt = true;

            Assert.That(() => node.RenderElement(new Element1()), Throws.InstanceOf<InvalidCastException>());
        }
    }
}