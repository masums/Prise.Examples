var target = Argument("target", "default");
var configuration = Argument("configuration", "Debug");

private void DeleteIfExists(string directory){
     var deleteSettings = new DeleteDirectorySettings{
        Force= true,
        Recursive = true
    };
     if (DirectoryExists(directory))
    {
      DeleteDirectory(directory, deleteSettings);
    }
}

private void CleanProject(string projectDirectory){
    var projectFile = $"{projectDirectory}/{projectDirectory}.csproj";
    var bin = $"{projectDirectory}/bin";
    var obj = $"{projectDirectory}/obj";
    var cleanSettings = new DotNetCoreCleanSettings
    {
        Configuration = configuration
    };
    DeleteIfExists(bin);
    DeleteIfExists(obj);
    DotNetCoreClean(projectFile, cleanSettings);
}

Task("clean").Does( () =>
{ 
    CleanProject("HelloWorldPlugin");
    DeleteIfExists("MyHost/Plugins");
});

Task("build")
  .IsDependentOn("clean")
  .Does( () =>
{ 
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration
    };

    DotNetCoreBuild("HelloWorldPlugin/HelloWorldPlugin.csproj", settings);
});

Task("publish")
  .IsDependentOn("build")
  .Does(() =>
  { 
    DotNetCorePublish("HelloWorldPlugin/HelloWorldPlugin.csproj", new DotNetCorePublishSettings
    {
        NoBuild = true,
        Configuration = configuration,
        OutputDirectory = "publish/HelloWorldPlugin",
        NoDependencies = true
    });
    CopyDirectory("publish/HelloWorldPlugin", "MyHost/Plugins");
  });

Task("default")
  .IsDependentOn("publish");

RunTarget(target);