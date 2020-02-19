using NUnit.Framework;

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
        }
    }
}