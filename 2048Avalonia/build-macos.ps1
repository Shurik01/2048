$ProjectName = "2048Avalonia"

$PublishDir = Join-Path $PSScriptRoot "2048Avalonia\bin\Release\net10.0\publish\osx-x64"
$AppDir = Join-Path $PSScriptRoot "$ProjectName.app"

# Удаляем старый .app
if (Test-Path $AppDir) {
    Remove-Item $AppDir -Recurse -Force
}

# Создаём структуру macOS приложения
$ContentsDir = Join-Path $AppDir "Contents"
$MacOSDir = Join-Path $ContentsDir "MacOS"
$ResourcesDir = Join-Path $ContentsDir "Resources"

New-Item -ItemType Directory -Path $MacOSDir -Force | Out-Null
New-Item -ItemType Directory -Path $ResourcesDir -Force | Out-Null

# Копируем publish внутрь Contents/MacOS
Copy-Item "$PublishDir\*" $MacOSDir -Recurse -Force

# Создаём Info.plist
$InfoPlist = @"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN"
  "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>2048Avalonia</string>

    <key>CFBundleIdentifier</key>
    <string>com.example.2048avalonia</string>

    <key>CFBundleName</key>
    <string>2048Avalonia</string>

    <key>CFBundleDisplayName</key>
    <string>2048 Avalonia</string>

    <key>CFBundlePackageType</key>
    <string>APPL</string>

    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>

    <key>CFBundleVersion</key>
    <string>1.0.0</string>

    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>

    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
"@

Set-Content `
    -Path (Join-Path $ContentsDir "Info.plist") `
    -Value $InfoPlist `
    -Encoding UTF8

Write-Host ""
Write-Host "Готово!"
Write-Host "Создано: $AppDir"