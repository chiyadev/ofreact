using System;
using NUnit.Framework;
using static ofreact.Hooks;

namespace ofreact.Tests
{
    [TestFixture]
    public class UseContextTests
    {
        public class MultiLevelNestedContext : ofComponent
        {
            class Context
            {
                public string Value;
            }

            public static bool ContextFound;

            protected override ofElement Render() => new ofContext<Context>(value: new Context { Value = "test" })
            {
                new Nested1()
            };

            class Nested1 : ofComponent
            {
                protected override ofElement Render() => new Nested2();

                class Nested2 : ofComponent
                {
                    protected override ofElement Render() => new Nested3();

                    class Nested3 : ofComponent
                    {
                        protected override ofElement Render()
                        {
                            var context = UseContext<Context>();

                            Assert.That(context, Is.Not.Null);
                            Assert.That(context.Value, Is.EqualTo("test"));

                            ContextFound = true;

                            return null;
                        }
                    }
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(ContextFound, Is.True);
            }
        }

        public class NearestContext : ofComponent
        {
            class Context
            {
                public string Value;
            }

            public static bool ContextFound;

            protected override ofElement Render() =>
                new ofContext<Context>(value: new Context { Value = "test1" })
                {
                    new Nested1()
                };

            class Nested1 : ofComponent
            {
                protected override ofElement Render() =>
                    new ofContext<Context>(value: new Context { Value = "test2" })
                    {
                        new Nested2()
                    };

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);
                        Assert.That(context.Value, Is.EqualTo("test2"));

                        ContextFound = true;

                        return null;
                    }
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(ContextFound, Is.True);
            }
        }

        public class NonexistentContext : ofComponent
        {
            public class Context
            {
                public string Value;
            }

            public bool ContextNotFound;

            protected override ofElement Render()
            {
                var context = UseContext<Context>();

                Assert.That(context, Is.Null);

                ContextNotFound = true;

                return null;
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(ContextNotFound, Is.True);
            }
        }

        public class DisposeContext : ofComponent
        {
            public static bool Disposed;

            public class Context : IDisposable
            {
                public void Dispose() => Disposed = true;
            }

            protected override ofElement Render() => new ofContext<Context>(value: new Context())
            {
                new Nested()
            };

            class Nested : ofComponent
            {
                protected override ofElement Render()
                {
                    var context = UseContext<Context>();

                    Assert.That(context, Is.Not.Null);

                    return null;
                }
            }

            [Test]
            public void Test()
            {
                var node = new ofRootNode();

                Assert.That(Disposed, Is.False);

                node.RenderElement(this);

                Assert.That(Disposed, Is.False);

                node.Dispose();

                Assert.That(Disposed, Is.True);
            }
        }

        public class PartialUpdateContextAvailable : ofComponent
        {
            public static int Rendered;
            public static StateObject<int> State;

            protected override ofElement Render() => new Nested1();

            class Context
            {
                [Prop] public string Value;
            }

            class Nested1 : ofComponent
            {
                protected override ofElement Render() => new ofContext<Context>(value: new Context { Value = "test" })
                {
                    new Nested2()
                };

                class Nested2 : ofComponent
                {
                    [State] readonly StateObject<int> _state;

                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);
                        Assert.That(context.Value, Is.EqualTo("test"));

                        ++Rendered;
                        State = _state;

                        return null;
                    }
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                Assert.That(Rendered, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(1));

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(1));

                State.Current++;

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(2));

                node.RenderElement(this);

                Assert.That(Rendered, Is.EqualTo(2));
            }
        }

        public class ContextChangeRerender : ofComponent
        {
            public static int Nested1Rendered;
            public static int Nested2Rendered;

            public bool RemoveNested2;

            protected override ofElement Render() => new ofContext<Context>(value: new Context
            {
                Count = Nested2Rendered
            })
            {
                new Nested1(RemoveNested2)
            };

            class Context
            {
                [Prop] public int Count;
            }

            class Nested1 : ofComponent
            {
                [Prop] readonly bool _value;

                public Nested1(bool value)
                {
                    _value = value;
                }

                protected override ofElement Render()
                {
                    ++Nested1Rendered;

                    if (_value)
                        return null;

                    return new Nested2();
                }

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);
                        Assert.That(context.Count, Is.EqualTo(Nested2Rendered));

                        ++Nested2Rendered;

                        return null;
                    }
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(1));
                Assert.That(Nested2Rendered, Is.EqualTo(1));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(1));
                Assert.That(Nested2Rendered, Is.EqualTo(2));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(1));
                Assert.That(Nested2Rendered, Is.EqualTo(3));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(1));
                Assert.That(Nested2Rendered, Is.EqualTo(4));

                RemoveNested2 = true;

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(2));
                Assert.That(Nested2Rendered, Is.EqualTo(4));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(2));
                Assert.That(Nested2Rendered, Is.EqualTo(4));

                RemoveNested2 = false;

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(3));
                Assert.That(Nested2Rendered, Is.EqualTo(5));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(3));
                Assert.That(Nested2Rendered, Is.EqualTo(6));
            }
        }

        public class EmptyContext : ofComponent
        {
            public static int Nested1Rendered;
            public static int Nested2Rendered;

            protected override ofElement Render() => new ofContext<Context>(value: new Context
            {
                IgnoreMe = $"test {Nested1Rendered}"
            })
            {
                new Nested1()
            };

            class Context
            {
                // [Prop]
                public string IgnoreMe;
            }

            class Nested1 : ofComponent
            {
                protected override ofElement Render()
                {
                    ++Nested1Rendered;

                    return new Nested2();
                }

                class Nested2 : ofComponent
                {
                    protected override ofElement Render()
                    {
                        var context = UseContext<Context>();

                        Assert.That(context, Is.Not.Null);

                        ++Nested2Rendered;

                        return null;
                    }
                }
            }

            [Test]
            public void Test()
            {
                using var node = new ofRootNode();

                Assert.That(Nested1Rendered, Is.EqualTo(0));
                Assert.That(Nested2Rendered, Is.EqualTo(0));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(1));
                Assert.That(Nested2Rendered, Is.EqualTo(1));

                node.RenderElement(this);

                Assert.That(Nested1Rendered, Is.EqualTo(1));
                Assert.That(Nested2Rendered, Is.EqualTo(1));
            }
        }
    }
}