using System;

namespace ofreact
{
    /// <summary>
    /// Marks a field as a prop of an element.
    /// </summary>
    /// <remarks>
    /// When a prop field does not change between renders, ofreact can optimize the rendering of the element.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class PropAttribute : Attribute { }
}