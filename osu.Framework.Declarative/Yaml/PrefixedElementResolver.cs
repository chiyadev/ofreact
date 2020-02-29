using System;
using ofreact;

namespace osu.Framework.Declarative.Yaml
{
    /// <summary>
    /// Resolves an element type by prepending or removing a name prefix.
    /// </summary>
    public class PrefixedElementResolver : IElementTypeResolver
    {
        readonly string _prefix;
        readonly IElementTypeResolver _resolver;
        readonly StringComparison _comparison;

        public PrefixedElementResolver(string prefix, IElementTypeResolver resolver, StringComparison comparison = StringComparison.Ordinal)
        {
            _prefix     = prefix;
            _resolver   = resolver;
            _comparison = comparison;
        }

        public Type Resolve(ComponentBuilderContext context, string name) => _resolver?.Resolve(context, AddPrefix(name)) ?? _resolver?.Resolve(context, RemovePrefix(name));

        string AddPrefix(string name)
        {
            if (name.StartsWith(_prefix, _comparison))
                return name;

            return _prefix + name;
        }

        string RemovePrefix(string name)
        {
            if (name.StartsWith(_prefix, _comparison))
                return name.Substring(_prefix.Length);

            return name;
        }
    }
}