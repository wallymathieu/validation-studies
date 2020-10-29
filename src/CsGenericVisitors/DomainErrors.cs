using System;

namespace CsGenericVisitors
{
    /// <summary>
    /// Errors understood to be part of the domain.
    /// </summary>
    [Flags]
    public enum DomainErrors
    {
        None=0,
        NameBetween1And50 =1<<0,
        EmailMustContainAtChar=1<<1,
        AgeBetween0and120=1<<2,
        DescriptionBetween1And50=1<<3,
    }
}