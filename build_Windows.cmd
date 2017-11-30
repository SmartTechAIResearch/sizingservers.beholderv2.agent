REM 2017 Sizing Servers Lab
REM University College of West-Flanders, Department GKG 
echo sizingservers.beholderv2.agent for Windows build script
echo ----------
rmdir /S /Q Build
cd sizingservers.beholderv2.agent
dotnet restore
dotnet publish -c Debug
cd ..\sizingservers.beholderv2.agent.windows
dotnet restore
dotnet build -c Debug
cd ..
copy /Y Build\netcoreapp2.0\publish\* Build\
rmdir /S /Q Build\netcoreapp2.0\