namespace CsMediatR.Infrastructure;
/// <summary>
/// From https://github.com/autofac/Autofac/blob/f52dc4d877aaa7428b93ccf799a0e4a265a88c85/src/Autofac/Util/Traverse.cs
/// Provides a method to support traversing structures.
/// </summary>
internal static class Traverse
{
    /// <summary>
    /// Traverse across a set, taking the first item in the set, and a function to determine the next item.
    /// </summary>
    /// <typeparam name="T">The set type.</typeparam>
    /// <param name="first">The first item in the set.</param>
    /// <param name="next">A callback that will take the current item in the set, and output the next one.</param>
    /// <returns>An enumerable of the set.</returns>
    public static IEnumerable<T> Across<T>(T first, Func<T, T> next)
        where T : class
    {
        var item = first;
        while (item != null)
        {
            yield return item;
            item = next(item);
        }
    }
}
    /// <summary>
    /// Provides helper methods for manipulating and inspecting types.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks whether this type is a closed type of a given generic type.
        /// </summary>
        /// <param name="this">The type we are checking.</param>
        /// <param name="openGeneric">The open generic type to validate against.</param>
        /// <returns>True if <paramref name="this"/> is a closed type of <paramref name="openGeneric"/>. False otherwise.</returns>
        public static bool IsClosedTypeOf(this Type @this, Type openGeneric)
        {
            return TypesAssignableFrom(@this).Any(t => t.IsGenericType && !@this.ContainsGenericParameters && t.GetGenericTypeDefinition() == openGeneric);
        }
        private static IEnumerable<Type> TypesAssignableFrom(Type candidateType)
        {
            return candidateType.GetInterfaces().Concat(
                Traverse.Across(candidateType, t => t.BaseType!));
        }
    }