namespace CsGenericValidations
{
    /// <summary>
    /// Errors understood to be part of the domain.
    /// </summary>
    public enum DomainErrors
    {
        None=0,
        NameBetween1And50,
        EmailMustContainAtChar,
        AgeBetween0and120,
        DescriptionBetween1And50,
    }
}