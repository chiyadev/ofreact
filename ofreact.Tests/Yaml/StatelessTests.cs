using System.IO;
using NUnit.Framework;
using osu.Framework.Declarative.Yaml;
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
            builder.Build(); // variable should not be stored statefully in builder object
            builder.Build();
        }
    }
}