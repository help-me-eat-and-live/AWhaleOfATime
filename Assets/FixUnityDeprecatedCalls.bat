@echo off
:: Fix deprecated Unity API usage in all .cs files under current directory

echo Running Unity API fixer...

for /R %%f in (*.cs) do (
    echo Fixing %%f...
    powershell -Command "(Get-Content \"%%f\") -replace 'GameObject\.FindObjectOfType\(([^)]+)\)', 'UnityEngine.Object.FindAnyObjectByType$1' | Set-Content \"%%f\""
    powershell -Command "(Get-Content \"%%f\") -replace 'Object\.FindObjectOfType\(([^)]+)\)', 'UnityEngine.Object.FindAnyObjectByType$1' | Set-Content \"%%f\""
    powershell -Command "(Get-Content \"%%f\") -replace 'FindObjectsOfType<([^>]+)>\\(\\)', 'FindObjectsByType<$1>(FindObjectsSortMode.None)' | Set-Content \"%%f\""
    powershell -Command "(Get-Content \"%%f\") -replace 'RenderingPath\.DeferredLighting', 'RenderingPath.DeferredShading' | Set-Content \"%%f\""
)

echo Done. All deprecated calls replaced.
pause
