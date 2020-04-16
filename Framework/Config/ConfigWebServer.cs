﻿namespace Framework.Config
{
    using Framework.DataAccessLayer;
    using Framework.Server;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Configuration used by deployed web server. This file is generated by cli build command.
    /// </summary>
    internal class ConfigServer
    {
        /// <summary>
        /// Gets or sets EnvironmentName. For example DEV, TEST or PROD.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Gets or sets IsUseDeveloperExceptionPage. If true, show detailed exceptions.
        /// </summary>
        public bool IsUseDeveloperExceptionPage { get; set; }

        /// <summary>
        /// Gets or sets IsServerSideRendering. By default this value is true. Can be changed on the deployed server for trouble shooting.
        /// </summary>
        public bool IsServerSideRendering { get; set; }

        /// <summary>
        /// Gets or sets ConnectionStringFramework. This value is copied from ConfigCli.ConnectionStringFramework by cli build command.
        /// </summary>
        public string ConnectionStringFramework { get; set; }

        /// <summary>
        /// Gets or sets ConnectionStringApplication. This value is copied from CliConfig.ConnectionStringApplication by cli build command.
        /// </summary>
        public string ConnectionStringApplication { get; set; }

        /// <summary>
        /// Returns WebServer ConnectionString.
        /// </summary>
        public static string ConnectionString(bool isFrameworkDb)
        {
            var configServer = ConfigServer.Load();
            if (isFrameworkDb == false)
            {
                return configServer.ConnectionStringApplication;
            }
            else
            {
                return configServer.ConnectionStringFramework;
            }
        }

        /// <summary>
        /// Returns ConnectionString for Application or Framework database.
        /// </summary>
        /// <param name="typeRow">Application or Framework data row.</param>
        public static string ConnectionString(Type typeRow)
        {
            bool isFrameworkDb = UtilDalType.TypeRowIsFrameworkDb(typeRow);
            return ConnectionString(isFrameworkDb);
        }

        public List<ConfigServerWebsite> WebsiteList { get; set; }

        /// <summary>
        /// Gets ConfigServer.json. Created und updated by CommandBuild. See also publish folder.
        /// </summary>
        private static string FileName
        {
            get
            {
                if (UtilServer.IsIssServer == false)
                {
                    return UtilFramework.FolderName + "ConfigServer.json";
                }
                else
                {
                    return UtilServer.FolderNameContentRoot() + "ConfigServer.json";
                }
            }
        }

        /// <summary>
        /// Returns default ConfigServer.json
        /// </summary>
        private static ConfigServer Init()
        {
            ConfigServer result = new ConfigServer();
            result.IsServerSideRendering = true;
            result.WebsiteList = new List<ConfigServerWebsite>();
            return result;
        }

        internal static ConfigServer Load()
        {
            ConfigServer result;
            if (File.Exists(FileName))
            {
                result = UtilFramework.ConfigLoad<ConfigServer>(FileName);
            }
            else
            {
                result = Init();
            }

            if (result.WebsiteList == null)
            {
                result.WebsiteList = new List<ConfigServerWebsite>();
            }
            foreach (var website in result.WebsiteList)
            {
                if (website.DomainNameList == null)
                {
                    website.DomainNameList = new List<ConfigServerWebsiteDomain>();
                }
            }
            return result;
        }

        internal static void Save(ConfigServer configServer)
        {
            UtilFramework.ConfigSave(configServer, FileName);
        }
    }

    internal class ConfigServerWebsite
    {
        /// <summary>
        /// Returns FolderNameServer. For example: "Application.Server/Framework/Application.Website/Website01/".
        /// </summary>
        public string FolderNameServerGet(ConfigServer configServer, string prefixRemove)
        {
            string result = string.Format("Application.Server/Framework/Application.Website/Master{0:00}/", configServer.WebsiteList.IndexOf(this) + 1);
            UtilFramework.Assert(result.StartsWith(prefixRemove));
            result = result.Substring(prefixRemove.Length);
            return result;
        }

        public List<ConfigServerWebsiteDomain> DomainNameList { get; set; }
    }

    /// <summary>
    /// DomainName to AppTypeName.
    /// </summary>
    internal class ConfigServerWebsiteDomain
    {
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets AppTypeName. Needs to derrive from AppJson. For example: "Application.AppJson, Application". If null, index.html is rendered without server side rendering.
        /// </summary>
        public string AppTypeName { get; set; }
    }
}
