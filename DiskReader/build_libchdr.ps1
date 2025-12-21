echo "Building libchdr dependency for current system"

# Find the installation path of Visual Studio
$vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath

if (-not $vsPath) {
    Write-Error "Visual Studio with C++ build tools not found."
    exit 1
}

# Path to the Dev Shell module
$devShellPath = Join-Path $vsPath "Common7\Tools\Microsoft.VisualStudio.DevShell.dll"

# Import and initialize the environment (targeting x64)
Import-Module $devShellPath
Enter-VsDevShell -VsInstallPath $vsPath -SkipAutomaticLocation -Arch amd64

$srcDir = $PSScriptRoot + "\\libchdr"
$buildDir = $srcDir + "\\build"

if (!(Test-Path -Path $buildDir)) {
  New-Item -Path $buildDir -ItemType Directory
}

cmake -S $srcDir -B $buildDir -DCMAKE_BUILD_TYPE=Release -G Ninja 
cmake --build $buildDir
