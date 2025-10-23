using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Interface for an object that represents an action through multiple parameters.
/// </summary>
internal interface IAction
{
    /// <summary>
    /// Combines the <see cref="ActionName"/> and <see cref="TypeName"/>.
    /// </summary>
    public string Id => $"{ActionName}_@_{TypeName}";

    /// <summary>
    /// The first name of this <see cref="IAction"/>.
    /// </summary>
    string ActionName { get; }

    /// <summary>
    /// The second name of this <see cref="IAction"/>.
    /// </summary>
    string TypeName { get; }

    /// <summary>
    /// The backing value of this <see cref="IAction"/>.
    /// </summary>
    IReadOnlyList<IParameter> Parameters { get; }

    /// <summary>
    /// The inverse relationship to its preceding parameter.
    /// </summary>
    /// <remarks>
    /// Can be <see langword="null"/> if this <see cref="IAction"/> is the first item in its hierarchical structure.
    /// </remarks>
    IParameter? ParentParameter { get; }

    /// <summary>
    /// Finds the <see cref="IParameter"/> with the specified <paramref name="parameterName"/>.
    /// </summary>
    /// <returns>
    /// The parameter if found; otherwise <see langword="null"/>.
    /// </returns>
    public IParameter? this[string parameterName] { get; }

    /// <summary>
    /// Finds the <see cref="IParameter"/> with the specified <paramref name="parameterName"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to find.</param>
    /// <param name="parameter">The parameter if it was found or <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the parameter can be found; otherwise <see langword="false"/>.
    /// </returns>
    bool TryGetParameter(string parameterName, [NotNullWhen(true)] out IParameter? parameter);

    /// <summary>
    /// Reads the parameter with the specified <paramref name="parameterName"/> and the correct storage type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to read a value from.</param>
    /// <param name="value">The value of the parameter if it is compatible with the target type, or <see langword="default"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the parameter can be found and is compatible; otherwise <see langword="false"/>.
    /// </returns>
    bool TryGetParameterValue(string parameterName, out int value);

    /// <inheritdoc cref="TryGetParameterValue"/>
    bool TryGetParameterValue(string parameterName, out double value);

    /// <inheritdoc cref="TryGetParameterValue"/>
    bool TryGetParameterValue(string parameterName, out bool value);

    /// <inheritdoc cref="TryGetParameterValue"/>
    bool TryGetParameterValue<TEnum>(string parameterName, out TEnum value) where TEnum : struct, Enum;

    /// <param name="value">The value of the parameter if it is compatible with the target type, or an empty string ("").</param>
    /// <remarks>
    /// The value is guaranteed to be non-null.
    /// </remarks>
    /// <inheritdoc cref="TryGetParameterValue"/>
    bool TryGetParameterValue(string parameterName, out string value);

    /// <inheritdoc cref="TryGetParameterValue"/>
    bool TryGetParameterValue(string parameterName, out IList<double> value);

    /// <inheritdoc cref="TryGetParameterValue"/>
    bool TryGetParameterValue(string parameterName, out IList<string> value);
}
