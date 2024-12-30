
namespace BidirectionalIterator
{
    /// <summary>
    /// Can freely iterate forwards and backwards over the contents of a <see cref="System.Collections.Generic.IList{T}"/>.
    /// </summary>
    /// <remarks>
    /// Be aware that no versioning checks can be implemented for this object, since internals of an IList are not visible to this assembly.
    /// </remarks>
    public struct ListIterator<T>(IList<T> list) : IBidirectionalIterator<T>
    {
        private IList<T> _list = list;
        private int _index;
        private T? _current;
        private bool _lastForward = true;

        object? System.Collections.IEnumerator.Current => Current;

        /// <inheritdoc/>
        public T Current => _current!;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public bool MoveNext()
        {
            AdjustIndexForward();

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
            AdjustIndexBackward();

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
                AdjustIndexForward();

                //new index must not be out of bounds
                var offsetIndex = _index + offset - 1;
                if ((uint)offsetIndex < (uint)_list.Count)
                {
                    _current = _list[offsetIndex];
                    _index = offsetIndex + 1;
                    return true;
                }

                _current = default;
                _index = _list.Count;
                return false;
            }
            else
            {
                if (offset is 0)
                {
                    return (!_lastForward || _index is not 0) //enumeration has already started
                        && (uint)_index < (uint)_list.Count; //not out of bounds
                }

                AdjustIndexBackward();

                //new index must not be out of bounds
                var offsetIndex = _index + offset + 1;
                if ((uint)offsetIndex < (uint)_list.Count)
                {
                    _current = _list[offsetIndex];
                    _index = offsetIndex - 1;
                    return true;
                }

                _current = default;
                _index = -1;
                return false;
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

        private void AdjustIndexBackward()
        {
            if (_lastForward)
            {
                _index -= 2;
                _lastForward = false;
            }
        }

        private void AdjustIndexForward()
        {
            if (!_lastForward)
            {
                _index += 2;
                _lastForward = true;
            }
        }
    }
}
