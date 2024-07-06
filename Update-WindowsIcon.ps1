$source = "$PSScriptRoot/build/MacOS/icon.iconset"
$destination = "$PSScriptRoot/src/Stravaig.ConnOfficer/Assets/Icons/app-icon.ico"

Write-Host "Converting icons from $source into $destination"

magick `
  $SOURCE/icon_16x16.png `
  $SOURCE/icon_32x32.png `
  $SOURCE/icon_32x32@2x.png `
  $SOURCE/icon_128x128.png `
  $SOURCE/icon_256x256.png `
  $DESTINATION