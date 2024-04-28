using NuGet.Versioning;
using RoslynPad.Build;
using RoslynPad.UI;
using RoslynPad.UI.SDK;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace RoslynPad;

[Export(typeof(IPlatformsFactory))]
internal class PlatformsFactory : IPlatformsFactory
{
    IReadOnlyList<ExecutionPlatform>? _executionPlatforms;

    public IReadOnlyList<ExecutionPlatform> GetExecutionPlatforms() =>
        _executionPlatforms ??= GetNetVersions().ToArray().AsReadOnly();

    private Lazy<ListSdks> _listSdks = new(() => new ListSdks());
    private string? _dotnetExe = null;
    private IEnumerable<ExecutionPlatform> GetNetVersions()
    {

        var sdks = _listSdks.Value.ToList();

        if (!sdks.Any())
        {
            return Array.Empty<ExecutionPlatform>();
        }

        var versions = new List<(string name, string tfm, NuGetVersion version)>();

        foreach (var sdk in sdks)
        {
            var versionName = sdk.Name;
            if (NuGetVersion.TryParse(versionName, out var version) && version.Major > 1)
            {
                var name = version.Major < 5 ? ".NET Core" : ".NET";
                var tfm = version.Major < 5 ? $"netcoreapp{version.Major}.{version.Minor}" : $"net{version.Major}.{version.Minor}";
                versions.Add((name, tfm, version));
            }
        }

        return versions
             .GroupBy(c => c.version.Major)
             .Select(c => c.MaxBy(i => i.version.Minor))
             .OrderBy(c => c.version.IsPrerelease)
             .ThenByDescending(c => c.version)
             .Select(version => new ExecutionPlatform(version.name, version.tfm, version.version, Architecture.X64, isDotNet: true));
    }

    public string DotNetExecutable()
    {

        if (_dotnetExe != null)
        {
            return _dotnetExe;
        }

        string dotnetExe;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            dotnetExe = "dotnet.exe";
        }
        else
        {
            dotnetExe = "dotnet";
        }

        var sdkPath = _listSdks.Value.Select(i => i.Path).Distinct().First();

        if (sdkPath != null)
        {
            dotnetExe = Path.GetFullPath(Path.Combine(sdkPath, "..", dotnetExe));
            if (File.Exists(dotnetExe))
            {
                _dotnetExe = dotnetExe;
                return dotnetExe;
            }
        }

        _dotnetExe = string.Empty;
        return string.Empty;
    }
}
