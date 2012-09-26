src\.nuget\nuget.exe spec build\BuildingBlocks.CopyManagement.nuspec -Force -AssemblyPath "build\BuildingBlocks.CopyManagement.dll"
src\.nuget\nuget.exe spec build\BuildingBlocks.Store.nuspec -Force -AssemblyPath "build\BuildingBlocks.Store.dll"
src\.nuget\nuget.exe spec build\BuildingBlocks.Store.RavenDB.nuspec -Force -AssemblyPath "build\BuildingBlocks.Store.RavenDB.dll"

src\.nuget\nuget.exe pack  build\BuildingBlocks.CopyManagement.nuspec
src\.nuget\nuget.exe pack  build\BuildingBlocks.Store.nuspec
src\.nuget\nuget.exe pack  build\BuildingBlocks.Store.RavenDB.nuspec