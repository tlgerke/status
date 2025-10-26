<#
Replaces helper logging method calls with direct Serilog calls where safe.
Backs up files with a .bak extension before editing.
#>
$root = Get-Location
$files = Get-ChildItem -Path $root -Recurse -Include *.cs | Where-Object { $_.FullName -notmatch '\\bin\\' -and $_.FullName -notmatch '\\obj\\' }
foreach ($f in $files) {
 $text = Get-Content $f.FullName -Raw
 $orig = $text
 $text = $text -replace 'Helpers\.Logging\.LogInformation\(','Serilog.Log.Information('
 $text = $text -replace 'Helpers\.Logging\.LogWarning\(','Serilog.Log.Warning('
 $text = $text -replace 'Helpers\.Logging\.LogError\(([^,]+),\s*"([^"]+)"\)','Serilog.Log.Error($1, "$2")'
 $text = $text -replace 'Helpers\.Logging\.LogError\(("[^"]+"|[^"]+?)\)','Serilog.Log.Error($1)'
 if ($text -ne $orig) {
 Copy-Item $f.FullName ($f.FullName + '.bak') -Force
 Set-Content -Path $f.FullName -Value $text -Force
 Write-Host "Patched $($f.FullName)"
 }
}
Write-Host "Replacements complete. Review .bak files as needed."