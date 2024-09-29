using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SyntaxParser;

/// <summary>
/// Represents a stack-based implementation of <see cref="Stack{T}"/> specifically for <see cref="SyntaxPair"/>.
/// </summary>
// inspired by: https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/Text/ValueStringBuilder.cs
internal ref struct SyntaxStack
{
    private Span<SyntaxPair> _span;
    private int _size;

    public SyntaxStack(Span<SyntaxPair> initialBuffer)
    {
        _span = initialBuffer;
        _size = 0;
    }

    public SyntaxStack(int initialCapacity)
    {
        _span = new SyntaxPair[initialCapacity];
        _size = 0;
    }

    public int Length => _size;

    public int Capacity => _span.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(SyntaxPair c)
    {
        int pos = _size;
        Span<SyntaxPair> chars = _span;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = c;
            _size = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
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
    }

    public SyntaxPair Pop()
    {
        var size = _size - 1;
        var array = _span;

        // if (_size == 0) is equivalent to if (size == -1), and this case is covered with (uint)size
        if ((uint)size >= (uint)array.Length)
            throw new InvalidOperationException("Stack is already empty!");

        _size = size;
        return array[size];
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(SyntaxPair c)
    {
        Grow(1);
        Push(c);
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

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        int newCapacity = (int)Math.Max(
            (uint)(_size + additionalCapacityBeyondPos),
            Math.Min((uint)_span.Length * 2, ArrayMaxLength));

        var newArray = new SyntaxPair[newCapacity];

        _span.Slice(0, _size).CopyTo(newArray);
        _span = newArray;
    }
}
