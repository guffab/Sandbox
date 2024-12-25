
namespace FlexibleIterator
{
    public interface IFlexibleIterator<T> : IEnumerator<T>
    {
        /// <summary>
        /// Advances the enumerator to the previous element of the collection.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to the previous element;
        /// <see langword="false"/> if the enumerator has passed the start of the collection.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        bool MovePrevious();

        /// <summary>
        /// Advances the enumerator to the element by an <paramref name="offset"/> to its current position.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the enumerator was successfully advanced to a another (or the same) element;
        /// <see langword="false"/> if the enumerator moved out of bounds.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        bool Move(int offset);
    }
}
