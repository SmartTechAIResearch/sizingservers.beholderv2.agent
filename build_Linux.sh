# 2017 Sizing Servers Lab
# University College of West-Flanders, Department GKG 
echo sizingservers.beholderv2.agent for Linux build script
echo ----------
rm -rf Build
cd sizingservers.beholderv2.agent
dotnet restore
dotnet publish -c Debug
cd ../sizingservers.beholderv2.agent.linux
dotnet restore
dotnet publish -c Debug
cd ..
mv Build/netcoreapp2.0/publish/* Build
rm -r Build/netcoreapp2.0
mv Build/Linux/netcoreapp2.0/publish/* Build/Linux
rm -r Build/Linux/netcoreapp2.0