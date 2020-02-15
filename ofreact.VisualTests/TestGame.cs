using osu.Framework.Declarative;

namespace ofreact.VisualTests
{
    public class TestGame : ofComponent
    {
        protected override ofElement Render()
        {
            return new ofGame(
                name: "ofreact",
                host: new GameHostOptions
                {
                    PortableInstallation = true
                })
            {
                new ofDrawSizePreservingFillContainer
                {
                    new ofTestBrowser()
                }
            };
        }
    }
}