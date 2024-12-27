
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
        private bool _lastForward = true;

        object? System.Collections.IEnumerator.Current => Current;

        /// <inheritdoc/>
        public T Current => _current!;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (!_lastForward)
            {
                _index += 2;
                _lastForward = true;
            }

            if (((uint)_index < (uint)_list.Count))
            {
                _current = _list[_index];
                _index++;
                return true;
            }

            _current = default;
            return false;
        }

        /// <inheritdoc/>
        public bool MovePrevious()
        {
            if (_lastForward)
            {
                _index -= 2;
                _lastForward = false;
            }

            if (((uint)_index < (uint)_list.Count))
            {
                _current = _list[_index];
                _index--;
                return true;
            }

            _current = default;
            return false;
        }

        /// <inheritdoc/>
        public bool Move(int offset)
        {
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
                if (offset is 0)
                {
                    return (!_lastForward || _index is not 0) //enumeration has already started
                        && (uint)_index < (uint)_list.Count; //not out of bounds
                }

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
