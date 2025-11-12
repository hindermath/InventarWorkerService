namespace InventarWorkerCommon.Helpers.Exceptions;

/// <summary>
/// Provides exception types related to network information errors, such as missing or unresolved information.
/// </summary>
public class NetworkInformation
{
    private const string UnknownMachineName = @"""Unknown Machine Name""";

    /// <summary>
    /// Represents an exception that occurs when network information
    /// such as IPv4, IPv6, or FQDN for a machine is missing or unavailable.
    /// </summary>
    public class NetworkInformationMissingException : Exception
    {
        /// <summary>
        /// Gets the name of the machine associated with the exception.
        /// Represents the name of the machine for which network information
        /// (such as IPv4, IPv6, or FQDN) is missing or unavailable.
        /// </summary>
        public string? MachineName { get; }

        /// <summary>
        /// Represents an exception that is thrown when network information
        /// such as IPv4 address, IPv6 address, or Fully Qualified Domain Name (FQDN)
        /// for a particular machine is missing or cannot be resolved.
        /// </summary>
        /// <param name="machineName">The name of the machine for which network information is missing or unavailable.</param>
        public NetworkInformationMissingException(string? machineName)
            : base($"Machine {machineName ?? UnknownMachineName} has no IPv4, IPv6 or FQDN information!") =>
            MachineName = machineName ?? UnknownMachineName;

        /// <summary>
        /// Represents an exception that occurs when network information,
        /// such as IPv4, IPv6, or Fully Qualified Domain Name (FQDN),
        /// for a specific machine is missing or cannot be determined.
        /// </summary>
        /// <param name="machineName">The name of the machine for which network information is missing or unavailable.</param>
        /// <param name="innerException">The inner exception that caused the current exception.</param>
        public NetworkInformationMissingException(string? machineName, Exception innerException)
            : base($"Machine {machineName ?? UnknownMachineName} has no IPv4, IPv6 or FQDN information!", innerException) =>
            MachineName = machineName  ?? UnknownMachineName;
    }

    /// <summary>
    /// Represents an exception that is thrown when a host's network information cannot be resolved.
    /// </summary>
    public class HostNetworkInformationCannotResolveException : Exception
    {
        /// <summary>
        /// Gets the name of the machine where the exception occurred or which is relevant to the exception context.
        /// Typically used to identify the machine for which network information could not be resolved or is unavailable.
        /// </summary>
        public string? MachineName { get; }

        /// <summary>
        /// Represents an exception that is thrown when a host's network information cannot be resolved.
        /// </summary>
        /// <param name="machineName">The name of the machine where the exception occurred or which is relevant to the exception context.</param>
        public HostNetworkInformationCannotResolveException(string? machineName)
            : base($"Could not resolve host's network information for {machineName ?? UnknownMachineName}!") =>
            MachineName = machineName ?? UnknownMachineName;

        /// <summary>
        /// Represents an exception that is thrown when a host's network information cannot be resolved.
        /// </summary>
        /// <param name="machineName">The name of the machine where the exception occurred or which is relevant to the exception context.</param>
        /// <param name="innerException">The inner exception that caused the current exception.</param>
        public HostNetworkInformationCannotResolveException(string? machineName, Exception innerException)
            : base($"Could not resolve host's network information for {machineName ?? UnknownMachineName}!", innerException) =>
            MachineName = machineName ?? UnknownMachineName;
    }
}