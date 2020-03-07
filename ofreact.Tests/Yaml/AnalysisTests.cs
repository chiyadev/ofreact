using System;
using System.IO;
using NUnit.Framework;
using osu.Framework.Declarative.Yaml;
using YamlDotNet.RepresentationModel;

namespace ofreact.Tests.Yaml
{
    public class AnalysisElement : ofElement
    {
        public AnalysisElement(ElementKey key = default, int integer = default, bool boolean = default) : base(key) { }
    }

    public class AnalysisTests
    {
        [Test]
        public void QuickAnalysis()
        {
            var stream = new YamlStream();

            stream.Load(new StringReader(@"
render:
  AnalysisElement:
    integer: test
    boolean: test"));

            var builder = new YamlComponentBuilder(stream.Documents[0]);

            var e = Assert.Throws<YamlComponentException>(() => builder.Build());

            Assert.That(e.Node.Start.Line, Is.EqualTo(4));
            Assert.That(e.Node.Start.Column, Is.EqualTo(14));
        }

        [Test]
        public void FullAnalysis()
        {
            var stream = new YamlStream();

            stream.Load(new StringReader(@"
render:
  AnalysisElement:
    integer: test
    boolean: test"));

            var builder = new YamlComponentBuilder(stream.Documents[0])
            {
                FullAnalysis = true
            };

            var e = Assert.Throws<AggregateException>(() => builder.Build());

            Assert.That(e.InnerExceptions, Has.Exactly(2).Items);

            if (e.InnerExceptions[0] is YamlComponentException e1)
            {
                Assert.That(e1.Node.Start.Line, Is.EqualTo(4));
                Assert.That(e1.Node.Start.Column, Is.EqualTo(14));
            }
            else
            {
                Assert.Fail();
            }

            if (e.InnerExceptions[1] is YamlComponentException e2)
            {
                Assert.That(e2.Node.Start.Line, Is.EqualTo(5));
                Assert.That(e2.Node.Start.Column, Is.EqualTo(14));
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void NestedFullAnalysis()
        {
            var stream = new YamlStream();

            stream.Load(new StringReader(@"
render:
  - - - - AnalysisElement:
        - AnalysisElement:
            integer: 1
        - AnalysisElement:
            integer: one

  - AnalysisElement:
      boolean: test
  - AnalysisElement:
      boolean: true"));

            var builder = new YamlComponentBuilder(stream.Documents[0])
            {
                FullAnalysis = true
            };

            var e = Assert.Throws<AggregateException>(() => builder.Build());

            Assert.That(e.InnerExceptions, Has.Exactly(2).Items);

            if (e.InnerExceptions[0] is YamlComponentException e1)
            {
                Assert.That(e1.Node.Start.Line, Is.EqualTo(7));
                Assert.That(e1.Node.Start.Column, Is.EqualTo(22));
            }
            else
            {
                Assert.Fail();
            }

            if (e.InnerExceptions[1] is YamlComponentException e2)
            {
                Assert.That(e2.Node.Start.Line, Is.EqualTo(10));
                Assert.That(e2.Node.Start.Column, Is.EqualTo(16));
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}