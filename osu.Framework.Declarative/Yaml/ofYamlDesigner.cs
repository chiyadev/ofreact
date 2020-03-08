using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ofreact;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using osuTK;
using osuTK.Graphics;
using YamlDotNet.RepresentationModel;
using static ofreact.Hooks;
using static osu.Framework.Declarative.Hooks;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Renders a YAML component and debug information.
    /// This is used in visual tests.
    /// </summary>
    public class ofYamlDesigner : ofComponent
    {
        [Prop] readonly string _document;

        /// <summary>
        /// Creates a new <see cref="ofYamlDesigner"/> from a document string.
        /// </summary>
        public ofYamlDesigner(string document, ElementKey key = default) : base(key)
        {
            _document = document;
        }

        protected override ofElement Render()
        {
            // build yaml
            var (element, code, exception) = UseMemo(() =>
            {
                var stream = new YamlStream();

                stream.Load(new StringReader(_document));

                var builder = new YamlComponentBuilder(stream.Documents[0])
                {
                    FullAnalysis = true
                };

                try
                {
                    return (builder.Build(), builder.GenerateSource(), null as Exception);
                }
                catch (Exception e)
                {
                    return (null, null, e);
                }
            }, _document);

            return new ofContainer(style: new ContainerStyle
            {
                RelativeSizeAxes = Axes.Both,
                Masking          = true
            })
            {
                element,

                new ErrorDisplay(exception),
                new CodeDisplay(code)
            };
        }

        sealed class CodeDisplay : ofComponent
        {
            [Prop] readonly string _code;

            public CodeDisplay(string code) : base(code)
            {
                _code = code?.Replace("\r", "");
            }

            protected override ofElement Render()
            {
                var (open, setOpen) = UseState(false);

                var host = UseDependency<GameHost>();

                if (_code == null)
                    return null;

                if (!open)
                    return new ofButton(onClick: () => setOpen(true), style: new ButtonStyle
                    {
                        Text             = "Show generated code",
                        RelativeSizeAxes = Axes.X,
                        Size             = new Vector2(0.2f, 30),
                        Anchor           = Anchor.BottomLeft,
                        Origin           = Anchor.BottomLeft
                    });

                return new ofContainer(style: new ContainerStyle
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes     = Axes.Y,
                    Anchor           = Anchor.BottomLeft,
                    Origin           = Anchor.BottomLeft
                })
                {
                    new ofBox("background", style: new DrawableStyle
                    {
                        Colour           = Color4.Black,
                        RelativeSizeAxes = Axes.Both,
                        Alpha            = 0.8f
                    }),

                    new ofFillFlow(style: new FillFlowStyle
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes     = Axes.Y,
                        Direction        = FillDirection.Vertical
                    })
                    {
                        new ofContainer(style: new ContainerStyle
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes     = Axes.Y
                        })
                        {
                            new ofButton("show button", onClick: () => setOpen(false), style: new ButtonStyle
                            {
                                Text             = "Hide generated code",
                                RelativeSizeAxes = Axes.X,
                                Size             = new Vector2(0.8f, 30)
                            }),

                            new ofButton("copy button", onClick: () => host.GetClipboard().SetText(_code), style: new ButtonStyle
                            {
                                Text             = "Copy to clipboard",
                                RelativeSizeAxes = Axes.X,
                                Size             = new Vector2(0.2f, 30),
                                Anchor           = Anchor.TopRight,
                                Origin           = Anchor.TopRight
                            })
                        },

                        new ofScroll("text scroll", style: new ScrollStyle
                        {
                            RelativeSizeAxes = Axes.X,
                            Size             = new Vector2(1, 400)
                        })
                        {
                            new ofTextFlow(style: new TextFlowStyle
                            {
                                Text             = _code,
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes     = Axes.Y
                            })
                        }
                    }
                };
            }
        }

        sealed class ErrorDisplay : ofComponent
        {
            [Prop] readonly Exception _ex;

            public ErrorDisplay(Exception ex)
            {
                _ex = ex;
            }

            static IEnumerable<YamlComponentException> EnumerateErrors(Exception ex)
            {
                switch (ex)
                {
                    case null:
                        break;

                    case YamlComponentException yaml:
                        yield return yaml;

                        break;

                    case AggregateException aggregate:
                        foreach (var inner in aggregate.InnerExceptions)
                        foreach (var error in EnumerateErrors(inner))
                            yield return error;

                        break;

                    default:
                        foreach (var error in EnumerateErrors(ex.InnerException))
                            yield return error;

                        break;
                }
            }

            protected override ofElement Render()
            {
                var errors = UseMemo(() => EnumerateErrors(_ex).ToArray(), _ex);

                if (errors.Length == 0)
                    return null;

                return new ofScroll(style: new ScrollStyle
                {
                    RelativeSizeAxes = Axes.Both
                })
                {
                    new ofFillFlow(style: new FillFlowStyle
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes     = Axes.Y,
                        Direction        = FillDirection.Vertical
                    })
                    {
                        new ofText(style: new TextStyle
                        {
                            Text   = $"{errors.Length} error(s) occurred.",
                            Colour = Color4.Red
                        }),

                        errors.Select((e, i) => new Item(e, shouldOpen: i == 0, key: e.GetHashCode())).ToArray()
                    }
                };
            }

            sealed class Item : ofComponent
            {
                readonly Exception _ex;
                readonly bool _shouldOpen;

                public Item(Exception ex, ElementKey key = default, bool shouldOpen = default) : base(key)
                {
                    _ex         = ex;
                    _shouldOpen = shouldOpen;
                }

                protected override ofElement Render()
                {
                    var (open, setOpen) = UseState(_shouldOpen);

                    var button = new ofButton(onClick: () => setOpen(!open), style: new ButtonStyle
                    {
                        Text             = _ex.Message,
                        RelativeSizeAxes = Axes.X,
                        Size             = new Vector2(1, 20)
                    });

                    if (!open)
                        return button;

                    return new ofFillFlow(style: new FillFlowStyle
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes     = Axes.Y,
                        Direction        = FillDirection.Vertical
                    })
                    {
                        button,

                        new ofScroll("text scroll", style: new ScrollStyle
                        {
                            RelativeSizeAxes = Axes.X,
                            Size             = new Vector2(1, 400)
                        })
                        {
                            new ofTextFlow(style: new TextFlowStyle
                            {
                                Text             = $"{_ex.Message}\n\n{_ex.StackTrace}",
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes     = Axes.Y
                            })
                        }
                    };
                }
            }
        }
    }
}