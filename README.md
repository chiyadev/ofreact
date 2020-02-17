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
- Attribute-bound:
  - Ref fields
  - State fields
  - Effect methods
  - Extensibility in the binding pipeline

### Performance

ofreact will most definitely perform worse than writing custom Drawables if you are designing fast-changing scenes for a game.

However in the best-case scenario where there are only a few state changes, e.g. graphical user interfaces, ofreact will perform very well and the overhead should be negligible.

There are many optimizations in the rendering logic of components. Reflection calls are avoided unless absolutely necessary, and expression trees are heavily utilized and cached when JIT is available.

In addition, due to the lightweight design of the ofreact component tree, it is possible to optimize osu!framework's scene graph. If you are designing a modular scene using lots of intermediary containers, you can reduce the level of nesting by using ofreact components instead.

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
  width: 100,
  height: 100
}}>
  <SpriteText key='my sprite text' />
</Container>
```

### Example using hooks

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

### Writing a component using YAML (OML)

Not implemented yet.

```yaml
# the name by which this element can be referred from other elements
name: Cursor

# state variables cause a rerender when changed
states:
  # variable "hovered" of boolean type initialized to "false" is available within this element
  hovered: false

  # variables can explicitly specify a type
  number:
    value: 1.5
    type: float

# o!f injected dependencies
deps:
  # defines a dependency named "hoverSample" loaded from sample store with the name "test-channel"
  hoverSample:
    type: SampleChannel
    name: test-channel

# defines the rendering function
render:
  # renders a Transform element that applies transformations to all child Drawables
  Transform:
    # setting ref to an element will make it available in callbacks and side effects (imperative code)
    ref: hoverTransform

    # dictionary of actions that can be invoked
    actions:
      myAction:
        # action body is a list of transforms that are played sequentially
        # fade to 50% alpha in 150ms
        - fade:
            to: 0.5
            for: 150
        # scale to 2x in 100ms (concurrently with fade)
        - scale:
            to: 2
            from: 1
            for: 100ms # duration can specify units
            with: fade # this key allows the transform to play concurrently with the last "fade" transform in the list
        # delay for 100ms (after both fade and scale completes)
        - delay:
            for: 0.1s
        # fade colour to blue in 50ms
        - fadeColour:
            to: blue
            from: red
            for: 50
        # myAction takes 300ms in total to complete

        # it is possible to invoke another action in the same transform element
        - doNothing:
      doNothing:

    # defines the children to apply transform on
    children:
      # renders a Conditional element that is essentially a ternary operator in the rendering function
      Conditional:
        # dictionary of variables to test; variables are &&'d
        test:
          # tests the variable "hovered" for equality with true
          hovered: true

        # if the test passes, render the following element
        true:
          # renders a Container element which manages a Container<Drawable> in the o!f scene graph
          # *the Container<Drawable> will be transformed by the nearest ancestor Transform element
          Container:
            ref: rotateableContent

            # style prop can be used to set properties of the Container<Drawable>
            style:
              position: 50, 50
              relativeSizeAxes: both
              colour: yellow

        # otherwise, render a different element
        false:
          Container:
            ref: rotateableContent
            style:
              position: 50, 50
              autoSizeAxes: both

            # adds child elements which will be rendered inside the Container<Drawable>
            children:
              # renders a Box element that manages a Box drawable in o!f scene graph
              # this Box drawable will NOT be transformed by the ancestor Transform element because transforms are already applied on the parent Container<Drawable>
              # (if this Box was not contained in that Container<Drawable>, it would be transformed)
              Box:
                style:
                  size: 100 # vector2 shorthand = 100, 100

            # defines a function that handles Container's "onHover" callback prop
            onHover:
              # function body is a list of statements that are executed sequentially

              # act upon the variable "hoverSample"
              - hoverSample:
                  do: play # invoke "play" defined in SampleChannel

              # act upon the ref element "hoverTransform"
              - hoverTransform:
                  do: myAction # invoke "myAction" defined in Transform.actions prop

              # assign a value to the state variable "hovered"
              - hovered:
                  # assigning a value will cause a rerender of this element
                  # since "hovered" is now "true", the Conditional element test will pass, changing the styling of the rendered container and removing inner box drawable
                  set: true
```

### Using ofreact not for osu!framework

ofreact is designed for maximum extensibility, so you can use all of ofreact features without osu!framework-specific features.

- Project `ofreact` implements the core functionality including rendering, state management, hooks, etc. This project has no dependency except .NET Standard 2.1.
- Project `osu.Framework.Declarative` provides Drawable-wrappers and other elements that bootstrap ofreact in the osu!framework scene graph.

In fact, `osu.Framework.Declarative` is simply an extension of the core ofreact project. It is entirely possible to use another GUI library and use ofreact to write declarative wrapper components.

## TODO

- **YAML-based components** (attempt at [this issue](https://github.com/ppy/osu-framework/issues/3056))
- More wrappers for osu!framework classes
- More thorough testing
- More hooks from react
