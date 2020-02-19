using NUnit.Framework;
using ofreact.Yaml;
using YamlDotNet.RepresentationModel;

namespace ofreact.Tests.Yaml
{
    public class NameTest
    {
        [Test]
        public void AssignName()
        {
            var builder = new YamlComponentBuilder(new YamlDocument(new YamlMappingNode
            {
                Children =
                {
                    ["name"] = "Test name"
                }
            }));

            builder.Build();

            Assert.That(builder.Name, Is.EqualTo("Test name"));
        }
    }
}