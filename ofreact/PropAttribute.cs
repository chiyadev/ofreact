using System;

namespace ofreact
{
    /// <summary>
    /// Marks a field as a property of the element.
    /// Prop fields must be public.
    /// </summary>
    /// <remarks>
    /// Fields annotated with this attribute will cause a rerender when the field's value changes.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class PropAttribute : Attribute { }
}