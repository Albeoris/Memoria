param(
    [Parameter(Mandatory = $true)]
    [String] $AssemblyPath,
    [Parameter(Mandatory = $true)]
    [String] $PatcherPath,
    [Parameter(Mandatory = $true)]
    [String] $OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$resolvedAssemblyPath = (Resolve-Path -LiteralPath $AssemblyPath).Path
$resolvedPatcherPath = (Resolve-Path -LiteralPath $PatcherPath).Path
$assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($resolvedAssemblyPath).Version
$buildTimeUtc = [DateTime]::new(2000, 1, 1, 0, 0, 0, [DateTimeKind]::Utc).AddDays($assemblyVersion.Build).AddSeconds($assemblyVersion.Revision * 2)
$manifest = [ordered]@{
    SchemaVersion = 1
    BuildTimeUtc = $buildTimeUtc.ToString('O', [Globalization.CultureInfo]::InvariantCulture)
    PatcherSize = (Get-Item -LiteralPath $resolvedPatcherPath).Length
}

$resolvedOutputPath = [IO.Path]::GetFullPath($OutputPath)
$outputDirectory = [IO.Path]::GetDirectoryName($resolvedOutputPath)
[IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
$json = $manifest | ConvertTo-Json
[IO.File]::WriteAllText($resolvedOutputPath, $json + [Environment]::NewLine, [Text.UTF8Encoding]::new($false))
Write-Host "Generated update manifest for build $($manifest.BuildTimeUtc): $resolvedOutputPath"
