# ofreact

Declarative programming in osu!framework, inspired by React.

### Why?

It's an experiment. Declarative programming is nice.

### Features

List of things implemented:

- Component lifecycle management
- State management
- Partial rendering based on prop/state change
- Fragments
- Base component class for custom components
- Hooks:
  - UseRef
  - UseState
  - UseContext
  - UseEffect
  - Helper methods to define custom hooks

### Performance

ofreact will most definitely perform worse than writing custom Drawables by hand. But the performance difference is negligible if you are writing simple user interfaces.

### It's really not JSX

Because writing an "embedded XML to C#" transformer for the C# compiler is impractical (i.e. downright impossible), ofreact is designed to *look* like JSX even though it really isn't.

Props are passed via element constructor using named argument syntax, and children are added using collection initializer syntax.

```csharp
new ofContainer(key: "element key", style: new ContainerStyle
{
    Size = new Vector2(100)
})
{
    new ofDrawable<SpriteText>(key: "my sprite text")
}
```

If you squint hard enough you might see this:

```jsx
<Container key='element key' style={{
  position: 'relative',
  width: 100,
  height: 100
}}>
  <SpriteText key='my sprite text' />
</Container>
```

### What you can do

Right now, you can do things like this:

```csharp
using System;
using ofreact;
using osu.Framework.Declarative;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace MyGame
{
    public class MyComponent : ofComponent
    {
        protected override ofElement Render()
        {
            var (name, setName) = UseState("David");

            var buttonRef = UseRef<BasicButton>();

            UseEffect(() =>
            {
                Console.WriteLine("Component mounted!");

                return () => Console.WriteLine("Component unmounted...");
            }, null);

            return new ofFragment
            {
                // text
                new ofDrawable<SpriteText>(
                    key: "text display",
                    style: t => t.Text = name),

                // button
                new ofContainer(
                    key: "wrapper container",
                    style: new ContainerStyle
                    {
                        Alpha        = 0.5f,
                        Colour       = Color4.Aqua,
                        AutoSizeAxes = Axes.Both,
                        Margin = new MarginPadding
                        {
                            Top = 20
                        }
                    })
                {
                    // this button will change the name from "John" to "Smith"
                    new ofDrawable<BasicButton>(
                        key: "name button",
                        style: b =>
                        {
                            b.Text   = "Change name";
                            b.Action = () => setName("Smith");
                            b.Size   = new Vector2(100);
                        },
                        @ref: buttonRef)
                }
            };
        }
    }
}
```

## TODO

- **YAML-based scene graph parsing** (attempt at [this issue](https://github.com/ppy/osu-framework/issues/3056))
- More wrappers for osu!framework classes
- More thorough testing
- More hooks from react
