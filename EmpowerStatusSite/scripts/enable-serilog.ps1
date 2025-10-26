<#
.SYNOPSIS
Adds the SERILOG conditional symbol to the first .csproj found (or a specified project) and optionally runs the replacement script to switch Logging calls to Serilog.

.PARAMETER ProjectPath
Optional path to the .csproj file. If not provided the script finds the first .csproj under the current directory.

.PARAMETER ApplyReplacements
If set, the script will call apply-serilog-replacements.ps1 to rewrite Helpers.Logging usages to Serilog.Log calls.

.EXAMPLE
.
PS> .\enable-serilog.ps1 -ApplyReplacements

#>
param(
 [string]$ProjectPath = "",
 [switch]$ApplyReplacements
)

function Find-Csproj {
 param($start)
 return Get-ChildItem -Path $start -Filter *.csproj -Recurse -ErrorAction SilentlyContinue | Select-Object -First1
}

if (-not $ProjectPath) {
 $proj = Find-Csproj (Get-Location)
 if (-not $proj) { Write-Error "No .csproj found in or under current directory."; exit1 }
 $ProjectPath = $proj.FullName
}

Write-Host "Updating project: $ProjectPath"
[xml]$xml = Get-Content $ProjectPath
$ns = @{msb='http://schemas.microsoft.com/developer/msbuild/2003'}

$propertyGroups = $xml.Project.PropertyGroup | Where-Object { $_.Condition -ne $null }
if (-not $propertyGroups) {
 # fallback: use first PropertyGroup
 $propertyGroups = $xml.Project.PropertyGroup | Select-Object -First1
}

$modified = $false
foreach ($pg in $propertyGroups) {
 $dc = $pg.DefineConstants
 if ($dc) {
 $vals = $dc -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }
 if ($vals -notcontains 'SERILOG') {
 $vals += 'SERILOG'
 $pg.DefineConstants = ($vals -join ';')
 $modified = $true
 Write-Host "Added SERILOG to existing DefineConstants in PropertyGroup with Condition '$($pg.Condition)'."
 }
 } else {
 # create DefineConstants
 $pg.AppendChild($xml.CreateElement('DefineConstants', $xml.Project.NamespaceURI)) | Out-Null
 $pg.DefineConstants = 'SERILOG'
 $modified = $true
 Write-Host "Created DefineConstants in PropertyGroup with Condition '$($pg.Condition)'."
 }
}

if ($modified) {
 $xml.Save($ProjectPath)
 Write-Host "Project updated. Please reload the project in Visual Studio if it is open."
} else {
 Write-Host "Project already contains SERILOG in DefineConstants. No change made."
}

if ($ApplyReplacements) {
 $scriptPath = Join-Path (Split-Path $MyInvocation.MyCommand.Definition) 'apply-serilog-replacements.ps1'
 if (Test-Path $scriptPath) {
 Write-Host "Applying source replacements to use Serilog.Log (will back up files with .bak extension)..."
 & $scriptPath
 } else {
 Write-Error "Replacement script not found at $scriptPath"
 }
}

Write-Host "Done."