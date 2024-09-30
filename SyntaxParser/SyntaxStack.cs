using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SyntaxParser;

// inspired by: https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
//          and: https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/Generic/Stack.cs

/// <summary>
/// Represents a stack-based implementation of <see cref="Stack{T}"/> specifically for <see cref="SyntaxPair"/>.
/// </summary>
/// <param name="initialBuffer">A buffer that will be used as internal storage for this stack.</param>
/// <remarks>
/// Once the stack is required to go outside the bounds of the <paramref name="initialBuffer"/> it will allocated a new array on the heap!
/// </remarks>
[DebuggerDisplay("Length = {Length}")]
internal ref struct SyntaxStack(Span<SyntaxPair> initialBuffer)
{
    private Span<SyntaxPair> _span = initialBuffer; // Storage for stack elements
    private int _size= 0; // Number of items in the stack
    private int _version; // Used to keep enumerator in sync w/ collection

    public SyntaxStack(int initialCapacity) : this(new SyntaxPair[initialCapacity])
    {
    }

    public int Length => _size;

    public int Capacity => _span.Length;

    public Enumerator GetEnumerator() => new Enumerator(this); //do not rename (duck typing)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(SyntaxPair c)
    {
        //cast to unsigned accounts for overflow
        if ((uint)_size < (uint)_span.Length)
        {
            _span[_size] = c;
            _size++;
            _version++;
        }
        else
        {
            PushWithResize(c);
        }
    }

    public void Push(scoped ReadOnlySpan<SyntaxPair> value)
    {
        int pos = _size;
        if (pos > _span.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_span.Slice(_size));
        _size += value.Length;
        _version++;
    }

    public SyntaxPair Pop()
    {
        var size = _size - 1;
        var array = _span;

        // if (_size == 0) is equivalent to if (size == -1), and this case is covered with (uint)size
        if ((uint)size >= (uint)array.Length)
            throw new InvalidOperationException("Stack is already empty!");

        _size = size;
        _version++;
        return array[size];
    }

    // Non-inline from Stack.Push to improve its code quality as uncommon path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void PushWithResize(SyntaxPair c)
    {
        Debug.Assert(_size == _span.Length);
        Grow(1);

        _span[_size] = c;
        _size++;
        _version++;
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_size"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_size > _span.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const int DefaultCapacity = 4;
        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        int newCapacity = DefaultCapacity;

        if (_span.Length is not 0)
        {
            // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
            // to double the size if possible, bounding the doubling to not go beyond the max array length.
            newCapacity = (int)Math.Max(
                (uint)(_size + additionalCapacityBeyondPos),
                Math.Min((uint)_span.Length * 2, ArrayMaxLength));
        }

        var newArray = new SyntaxPair[newCapacity];

        _span.Slice(0, _size).CopyTo(newArray);
        _span = newArray;
    }

    public ref struct Enumerator(scoped in SyntaxStack syntaxStack)
    {
        private readonly SyntaxStack _stack = syntaxStack;
        private readonly int _version = syntaxStack._version;
        private int _index = syntaxStack._size;

        public SyntaxPair Current => _stack._span[_index]; //do not rename (duck typing)

        public bool MoveNext() //do not rename (duck typing)
        {
            if (_version != _stack._version)
                throw new InvalidOperationException("Collection was modified after the enumerator was instantiated");

            if (_index == -1) // End of enumeration.
                return false;

            return --_index >= 0;
        }
    }
}
