using NUnit.Framework;
using ofreact.Yaml;

namespace ofreact.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ofElement.IsHookValidated = true;

            ofRootNode.IsDiagnosticsEnabled = true;

            // make elements in test assembly available
            YamlComponentBuilder.DefaultTypeResolver = new CompositeElementResolver(
                YamlComponentBuilder.DefaultTypeResolver,
                new PrefixedElementResolver("of", new AssemblyElementResolver(GetType().Assembly)));
        }
    }
}