using System.Diagnostics;
using System.Net.Http;
using System.Text;

namespace InventarWorkerServiceIntegrationTests;

internal static class LocalServiceHostManager
{
    private static readonly object SyncLock = new();
    private static Process? _process;
    private static readonly StringBuilder OutputBuffer = new();

    internal static async Task StartAsync(string baseUrl, int startupTimeoutSeconds = 60)
    {
        lock (SyncLock)
        {
            if (_process is { HasExited: false })
            {
                return;
            }
        }

        var repoRoot = FindRepositoryRoot();
        var projectPath = Path.Combine(repoRoot.FullName, "InventarWorkerService", "InventarWorkerService.csproj");
        if (!File.Exists(projectPath))
        {
            throw new InvalidOperationException($"InventarWorkerService.csproj wurde nicht gefunden: {projectPath}");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{projectPath}\" --no-build --no-launch-profile",
            WorkingDirectory = repoRoot.FullName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.Environment["ASPNETCORE_URLS"] = baseUrl;
        startInfo.Environment["Kestrel__Endpoints__Http__Url"] = baseUrl;
        startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

        var process = new Process { StartInfo = startInfo };
        process.OutputDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                lock (OutputBuffer)
                {
                    OutputBuffer.AppendLine(args.Data);
                }
            }
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.Data))
            {
                lock (OutputBuffer)
                {
                    OutputBuffer.AppendLine(args.Data);
                }
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException("Lokaler Test-Host konnte nicht gestartet werden.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        lock (SyncLock)
        {
            _process = process;
        }

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var statusEndpoint = $"{baseUrl.TrimEnd('/')}/api/inventar/status";
        var deadline = DateTime.UtcNow.AddSeconds(startupTimeoutSeconds);

        while (DateTime.UtcNow < deadline)
        {
            if (process.HasExited)
            {
                throw new InvalidOperationException(
                    $"Lokaler Test-Host wurde vorzeitig beendet (ExitCode {process.ExitCode}).\n{GetBufferedOutput()}");
            }

            try
            {
                using var response = await client.GetAsync(statusEndpoint);
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    return;
                }
            }
            catch
            {
                // Service ist wahrscheinlich noch im Startvorgang.
            }

            await Task.Delay(500);
        }

        TryStopProcess(process);
        lock (SyncLock)
        {
            _process = null;
        }

        throw new TimeoutException(
            $"Lokaler Test-Host wurde nicht rechtzeitig bereit unter {statusEndpoint}.\n{GetBufferedOutput()}");
    }

    internal static Task StopAsync()
    {
        Process? process;
        lock (SyncLock)
        {
            process = _process;
            _process = null;
        }

        if (process is null)
        {
            return Task.CompletedTask;
        }

        try
        {
            TryStopProcess(process);
        }
        catch
        {
            // Cleanup soll den Testabschluss nicht blockieren.
        }
        finally
        {
            process.Dispose();
        }

        return Task.CompletedTask;
    }

    private static void TryStopProcess(Process process)
    {
        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit(5000);
        }
    }

    private static DirectoryInfo FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var solutionPath = Path.Combine(current.FullName, "InventarWorkerService.sln");
            if (File.Exists(solutionPath))
            {
                return current;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Repository-Root mit InventarWorkerService.sln konnte nicht ermittelt werden.");
    }

    private static string GetBufferedOutput()
    {
        lock (OutputBuffer)
        {
            return OutputBuffer.ToString();
        }
    }
}
