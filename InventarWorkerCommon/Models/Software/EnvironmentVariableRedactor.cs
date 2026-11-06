namespace InventarWorkerCommon.Models.Software;

/// <summary>
/// DE: Entfernt Umgebungsvariablenwerte, bevor Inventardaten eine Prozess- oder Persistenzgrenze überschreiten.
/// EN: Removes environment-variable values before inventory data crosses a process or persistence boundary.
/// </summary>
public static class EnvironmentVariableRedactor
{
    private const string RedactedValue = "[REDACTED]";

    /// <summary>
    /// DE: Gibt einen Umgebungsvariableneintrag zurück, der nur seinen Namen enthält.
    /// EN: Returns an environment-variable entry that retains only its name.
    /// </summary>
    /// <param name="environmentVariable">DE: Ursprünglicher Variableneintrag. EN: Source environment-variable entry.</param>
    /// <returns>DE: Redigierter Eintrag oder <see langword="null"/> ohne Variablennamen. EN: A redacted entry, or <see langword="null"/> when no variable name is present.</returns>
    public static string? Redact(string? environmentVariable)
    {
        if (string.IsNullOrWhiteSpace(environmentVariable))
        {
            return null;
        }

        var separatorIndex = environmentVariable.IndexOf('=');
        if (separatorIndex <= 0)
        {
            return null;
        }

        return $"{environmentVariable[..separatorIndex]}={RedactedValue}";
    }

    /// <summary>
    /// DE: Gibt redigierte Umgebungsvariableneinträge zurück und lässt ungültige Einträge weg.
    /// EN: Returns redacted environment-variable entries and omits malformed entries.
    /// </summary>
    /// <param name="environmentVariables">DE: Ursprüngliche Variableneinträge. EN: Source environment-variable entries.</param>
    /// <returns>DE: Einträge mit Variablennamen, aber niemals ihren ursprünglichen Werten. EN: Entries that contain variable names but never their original values.</returns>
    public static List<string> RedactAll(IEnumerable<string>? environmentVariables)
    {
        if (environmentVariables is null)
        {
            return [];
        }

        return environmentVariables
            .Select(Redact)
            .OfType<string>()
            .ToList();
    }
}
