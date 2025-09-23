namespace ActionContainers;

/// <summary>
/// Interface for an object that represents a piece of data.
/// </summary>
internal interface IParameter
{
    /// <summary>
    /// The name of this <see cref="IParameter"/>.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The string value of this <see cref="IParameter"/>. <br/>
    /// Use methods like <see cref="TryGetValue(out bool)"/> for conversions.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// The inverse relationship to its action.
    /// </summary>
    IAction? ParentAction { get; }

    /// <summary>
    /// Represents the first/only <see cref="IAction"/> that is linked directly to this <see cref="IParameter"/>.
    /// </summary>
    public IAction? SubAction { get; }

    /// <summary>
    /// Represents every <see cref="IAction"/> that is linked directly to this <see cref="IParameter"/>.
    /// </summary>
    public List<IAction> SubActions { get; }

    /// <summary>
    /// Reads the value of this <see cref="IParameter"/> if its internal storage is compatible with the target type.
    /// </summary>
    /// <param name="value">The value of the parameter if it is compatible with the target type, or <see langword="default"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the storage type is compatible; otherwise <see langword="false"/>.
    /// </returns>
    bool TryGetValue(out int value);

    /// <inheritdoc cref="TryGetValue(out int)"/>
    bool TryGetValue(out bool value);

    /// <inheritdoc cref="TryGetValue(out int)"/>
    bool TryGetValue(out double value);

    /// <param name="value">The value of the parameter if it is compatible with the target type, or an empty string ("").</param>
    /// <inheritdoc cref="TryGetValue(out int)"/>
    bool TryGetValue(out string value);

    /// <param name="value">The value of the parameter if it is compatible with the target type, or an empty array.</param>
    /// <inheritdoc cref="TryGetValue(out int)"/>
    bool TryGetValue(out IList<double> value);

    /// <inheritdoc cref="TryGetValue(out IList{double})"/>
    bool TryGetValue(out IList<string> value);

    /// <inheritdoc cref="TryGetValue(out int)"/>
    bool TryGetValue<TEnum>(out TEnum value) where TEnum : struct, Enum;
}
