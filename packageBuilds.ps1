Compress-Archive -Path .\client\build -DestinationPath .\server\public\win.zip -Force
Compress-Archive -Path .\client\build.app -DestinationPath .\server\public\mac.zip -Force
svn commit server/public -m "updated clients"