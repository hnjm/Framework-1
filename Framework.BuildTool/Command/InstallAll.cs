﻿namespace Framework.BuildTool
{
    public class CommandInstallAll : Command
    {
        public CommandInstallAll() 
            : base("installAll", "npm install; dotnet restore; (Run from CMD, PS won't work. Python error can be ignored)")
        {

        }

        public override void Run()
        {
            ConnectionManagerCheck.JsonFileCreateIfNotExists();
            //
            UtilFramework.Log("Client>npm install");
            UtilBuildTool.NpmInstall(Framework.UtilFramework.FolderName + "Submodule/Client/");
            // Application
            UtilFramework.Log("Application>dotnet restore");
            UtilBuildTool.DotNetRestore(Framework.UtilFramework.FolderName + "Application/");
            UtilFramework.Log("Application>dotnet build");
            UtilBuildTool.DotNetBuild(Framework.UtilFramework.FolderName + "Application/");
            // Server
            UtilFramework.Log("Server>dotnet restore");
            UtilBuildTool.DotNetRestore(Framework.UtilFramework.FolderName + "Server/");
            UtilFramework.Log("Server>dotnet build");
            UtilBuildTool.DotNetBuild(Framework.UtilFramework.FolderName + "Server/");
            // Office
            if (Framework.UtilFramework.IsLinux == false)
            {
                UtilBuildTool.MSBuild(Framework.UtilFramework.FolderName + "Submodule/Office/Office.csproj"); // Office is not (yet) a .NET Core library.
            }
            new CommandBuildClient().Run();
        }
    }
}
