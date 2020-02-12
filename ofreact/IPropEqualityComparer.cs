using System;

namespace ofreact
{
    public interface IPropEqualityComparer
    {
        Func<ofElement, ofElement, bool> Equals { get; }
    }
}