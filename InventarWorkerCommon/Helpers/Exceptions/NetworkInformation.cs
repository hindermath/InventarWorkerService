namespace InventarWorkerCommon.Helpers.Exceptions;

/// <summary>
/// Provides exception types related to network information errors, such as missing or unresolved information.
/// </summary>
public class NetworkInformation
{
    /// <summary>
    /// Represents an exception that occurs when network information
    /// such as IPv4, IPv6, or FQDN for a machine is missing or unavailable.
    /// </summary>
    public class NetworkInformationMissingException : Exception
    {
        public string? MachineName { get; }

        public NetworkInformationMissingException(string machineName)
            : base($"Machine '{machineName}' has no IPv4, IPv6 or FQDN information")
        {
            MachineName = machineName;
        }
    }

    /// <summary>
    /// Represents an exception that is thrown when a host's network information cannot be resolved.
    /// </summary>
    public class HostResolutionException : Exception
    {
        public string? HostIdentifier { get; }

        public HostResolutionException(string hostIdentifier)
            : base($"Could not resolve host information for '{hostIdentifier}'")
        {
            HostIdentifier = hostIdentifier;
        }

        public HostResolutionException(string hostIdentifier, Exception innerException)
            : base($"Could not resolve host information for '{hostIdentifier}'", innerException)
        {
            HostIdentifier = hostIdentifier;
        }
    }
}