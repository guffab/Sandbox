
namespace FlexibleIterator
{
    /// <summary>
    /// Can freely iterate forwards and backwards over the contents of a <see cref="System.Collections.Generic.IList{T}"/>.
    /// </summary>
    /// <remarks>
    /// Be aware that no versioning checks can be implemented for this object, since internals of an IList are not visible to this assembly.
    /// </remarks>
    public struct ListIterator<T>(IList<T> list) : IFlexibleIterator<T>
    {
        private IList<T> _list = list;
        private int _index;
        private T? _current;
        private bool lastForward = true;

        object? System.Collections.IEnumerator.Current => Current;

        /// <inheritdoc/>
        public T Current => _current!;

        /// <summary>
        /// Enables the use of foreach on this object.
        /// </summary>
        public ListIterator<T> GetEnumerator() => this; //do not rename (duck typing)

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (!lastForward)
            {
                _index += 2;
                lastForward = true;
            }

            if (((uint)_index < (uint)_list.Count))
            {
                _current = _list[_index];
                _index++;
                return true;
            }

            _index += 2;
            _current = default;
            return false;
        }

        /// <inheritdoc/>
        public bool MovePrevious()
        {
            if (lastForward)
            {
                _index -= 2;
                lastForward = false;
            }

            if (((uint)_index < (uint)_list.Count))
            {
                _current = _list[_index];
                _index--;
                return true;
            }

            _index -= 2;
            _current = default;
            return false;
        }

        /// <inheritdoc/>
        public bool Move(int offset)
        {
            //input of 0 is implicitly supported
            if (offset > 0)
            {
                for (int i = 0; i < offset; i++)
                {
                    if (!MoveNext())
                        return false;
                }
                return true;
            }
            else
            {
                for (int i = 0; i > offset; i--)
                {
                    if (!MovePrevious())
                        return false;
                }
                return true;
            }
        }

        public void Reset()
        {
            _index = 0;
            _current = default;
        }

        public void Dispose()
        {
        }
    }
}
