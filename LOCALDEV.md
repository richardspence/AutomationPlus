# Setup

## Lib folder
Copy the referenced dlls into the lib folder via

```ps1
$files = '0Harmondy.dll', 'Unity.TestMeshPro.dll', 'UnityEngine.CoreModule.dll', 'UnityEngine.dll', 'UnityEngine.TextRenderingModule.dll', 'UnityEngine.UI.dll', 'Unity.TextMeshPro.dll'

foreach ($file in $files){
    & xcopy ${env:ProgramFiles(x86)}\Steam\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\$file .\lib\ /Y
}
```

# Appendix
Useful directories:
| Dir | Description |
| --- | ---|
| %LOCALAPPDATA%\..\locallow\klei\Oxygen Not Included | Player Logs | 
| %ProgramFiles(x86)%\Steam\steamapps\common\OxygenNotIncluded | Oni |