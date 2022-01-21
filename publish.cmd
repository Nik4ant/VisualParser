# Maybe this is a garbage, but it works for now

dotnet publish VisualParser.csproj -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained true
mkdir "%cd%\bin\Release\net6.0\win-x64\publish\Static"
xcopy "%cd%\src\Static" "%cd%\bin\Release\net6.0\win-x64\publish\Static" /s/h/e/k/f
cd "%cd%\bin\Release\net6.0\win-x64\publish"
start .