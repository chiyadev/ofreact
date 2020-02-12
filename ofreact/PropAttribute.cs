using System;

namespace ofreact
{
    /// <summary>
    /// Marks a field as a prop of an element.
    /// </summary>
    /// <remarks>
    /// Prop fields must be public.
    /// If prop fields do not change across renders, ofreact can optimize rendering of elements.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class PropAttribute : Attribute { }
}