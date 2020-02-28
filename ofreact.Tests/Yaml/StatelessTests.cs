using System.IO;
using NUnit.Framework;
using ofreact.Yaml;
using YamlDotNet.RepresentationModel;

namespace ofreact.Tests.Yaml
{
    public class StatelessTestElement : ofElement { }

    public class StatelessTests
    {
        [Test]
        public void BuilderIsStateless()
        {
            var stream = new YamlStream();

            stream.Load(new StringReader(@"
render:
  StatelessTestElement:
    key: test")); // key prop declares variable

            var builder = new YamlComponentBuilder(stream.Documents[0]);

            builder.Build();
            builder.Build();
            builder.Build();
        }
    }
}