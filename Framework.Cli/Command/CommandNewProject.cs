﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Framework.Cli.Command
{
    /// <summary>
    /// Cli command to create a new project from template.
    /// For debug run in an empty folder: "dotnet run --project C:\Temp\GitHub\ApplicationDemo\ExternalGit\ApplicationDoc\Framework\Framework.Cli -- new".
    /// </summary>
    internal class CommandNewProject : CommandBase
    {
        public CommandNewProject(AppCli appCli)
            : base(appCli, "new", "Create new project")
        {

        }

        private static void FolderCopy(string folderNameSource, string folderNameDest)
        {
            // Copy ApplicationHelloWorld template
            // Console.WriteLine(string.Format("Copy {0} to {1}", folderNameSource, folderNameDest));
            UtilCli.FolderCopy(folderNameSource, folderNameDest);
        }

        protected internal override void Execute()
        {
            Uri baseUri = new Uri(typeof(CommandNewProject).Assembly.Location);
            string folderNameFramework = new Uri(baseUri, "../../../../").AbsolutePath;

            string folderNameApplicationHelloWorld = folderNameFramework + "Framework.Cli/Template/Application/ApplicationHelloWorld/";

            string folderNameSource = folderNameApplicationHelloWorld;
            string folderNameDest = Directory.GetCurrentDirectory().Replace(@"\", "/") + "/";

            // Console.WriteLine("Source=" + folderNameSource);
            // Console.WriteLine("Dest=" + folderNameDest);

            // Check dest folder is empty
            var list = Directory.GetFileSystemEntries(folderNameDest);
            var isGitOnly = list.Length == 1 && list[0] == folderNameDest + ".git"; // Empty git folder
            if (list.Length > 0 && !isGitOnly)
            {
                UtilCli.ConsoleWriteLineColor("This folder needs to be empty!", ConsoleColor.Red);
                return;
            }

            var isSubmodule = UtilCli.ConsoleReadYesNo("For Framework use Submodule?");

            Console.WriteLine("Installing...");

            // Copy ApplicationHelloWorld
            FolderCopy(folderNameSource, folderNameDest);

            if (isSubmodule)
            {
                var info = new ProcessStartInfo
                {
                    WorkingDirectory = folderNameDest,
                    FileName = "git",
                    Arguments = "submodule add https://github.com/WorkplaceX/Framework.git"
                };
                Process.Start(info).WaitForExit();
            }
            else
            {
                // Copy Framework
                UtilCli.FolderCreate(folderNameDest + "Framework/");
                FolderCopy(folderNameFramework, folderNameDest + "Framework/");
            }

            // Start new cli
            UtilCli.ConsoleWriteLineColor("Installation successfull!", ConsoleColor.Green);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                UtilCli.ConsoleWriteLineColor("Start cli now with command .\\wpx.cmd", ConsoleColor.DarkGreen);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                UtilCli.ConsoleWriteLineColor("Start cli now with command ./wpx.sh", ConsoleColor.DarkGreen);
            }
        }
    }
}
