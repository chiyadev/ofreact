using ofreact;

namespace osu.Framework.Declarative.VisualTests
{
    public class TestGame : ofComponent
    {
        protected override ofElement Render()
        {
            return new ofDrawSizePreservingFillContainer
            {
                new ofTestBrowser()
            };
        }
    }
}