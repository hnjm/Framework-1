﻿namespace Framework.BuildTool
{
    using Database.dbo;
    using Framework.Application;
    using Framework.Application.Setup;
    using Framework.BuildTool.DataAccessLayer;
    using Microsoft.Extensions.CommandLineUtils;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AppBuildTool
    {
        public AppBuildTool(App app)
        {
            this.App = app;
        }

        /// <summary>
        /// Gets App. Used for TypeRowInAssembly.
        /// </summary>
        public readonly App App;

        public void Run(string[] args)
        {
            CommandLineApplication commandLineApplication = UtilBuildTool.CommandLineApplicationCreate();
            RegisterCommand(commandLineApplication);
            try
            {
                commandLineApplication.Execute(args);
            }
            catch (Exception exception)
            {
                UtilFramework.LogError(UtilFramework.ExceptionToText(exception));
                Environment.Exit(1); // echo Exit Code is %errorlevel%
            }
        }

        /// <summary>
        /// Override to register additional application specific commands or clear command list.
        /// </summary>
        protected virtual void RegisterCommand(List<Command> result)
        {

        }

        /// <summary>
        /// Overwrite this method to filter out only specific application tables for which to generate code. For example only tables starting with "Explorer".
        /// </summary>
        /// <param name="list">Input list.</param>
        /// <returns>Returns filtered output list.</returns>
        protected virtual internal MetaSqlSchema[] GenerateFilter(MetaSqlSchema[] list)
        {
            // return list.Where(item => item.SchemaName == "dbo" && item.TableName.StartsWith("Explorer")).ToArray();
            return list;
        }

        /// <summary>
        /// Override to register application on table FrameworkApplication.
        /// </summary>
        protected virtual void DbFrameworkApplicationView(List<FrameworkApplicationView> result)
        {

        }

        internal List<FrameworkApplicationView> DbFrameworkApplicationView()
        {
            List<FrameworkApplicationView> result = new List<FrameworkApplicationView>();
            result.Add(new FrameworkApplicationView() { Text = "Setup Application", Path = "setup", IsActive = true, Type = UtilFramework.TypeToName(typeof(AppSetup)) });
            DbFrameworkApplicationView(result); // Override to register new applications.
            return result;
        }

        private void RegisterCommand(CommandLineApplication commandLineApplication)
        {
            List<Command> result = new List<Command>();
            result.Add(new CommandConnectionString()); // CLI shows commands in alphabetical order
            result.Add(new CommandCheck());
            result.Add(new CommandOpen());
            result.Add(new CommandServe());
            result.Add(new CommandUnitTest());
            result.Add(new CommandRunSqlCreate(this));
            result.Add(new CommandGenerate(this));
            result.Add(new CommandBuildClient());
            result.Add(new CommandInstallAll());
            RegisterCommand(result);
            foreach (Command command in result)
            {
                Command.Register(commandLineApplication, command);
            }
        }
    }
}
