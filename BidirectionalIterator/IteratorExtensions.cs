
namespace BidirectionalIterator
{
    /// <summary>
    /// Extends the concept of a reversible iterator on known collections.
    /// </summary>
    public static class IteratorExtensions
    {
        /// <summary>
        /// Returns an iterator that can freely move forwards and backwards during a single enumeration.
        /// </summary>
        public static ListIterator<T> GetBidirectionalIterator<T>(this IList<T> list)
        {
            return new ListIterator<T>(list);
        }
    }
}
