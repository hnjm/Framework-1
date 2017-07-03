﻿namespace Framework.Application
{
    using Framework.Component;
    using Framework.DataAccessLayer;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Linq;

    /// <summary>
    /// Server side root object.
    /// </summary>
    public class App
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public App()
        {
            ProcessInit(processList);
        }

        /// <summary>
        ///  Returns assembly to search for classes when deserializing json. (For example: "Database.dbo.Airport")
        /// </summary>
        virtual internal Type TypeRowInAssembly()
        {
            return GetType();
        }

        /// <summary>
        /// Returns assembly and namespace to search for classes when deserializing json. (For example: "MyPage")
        /// </summary>
        virtual internal Type TypeComponentInNamespace()
        {
            return GetType();
        }

        /// <summary>
        /// Returns type of main page. Used for first html request.
        /// </summary>
        protected virtual internal Type TypePageMain()
        {
            return typeof(Page);
        }

        /// <summary>
        /// Gets or sets AppJson. This is the application root json component being transferred between server and client.
        /// </summary>
        public AppJson AppJson { get; private set; }

        /// <summary>
        /// Called after method UtilDataAccessLayer.ValueToText();
        /// </summary>
        protected virtual internal void CellValueToText(string gridName, string index, Cell cell, ref string result)
        {

        }

        /// <summary>
        /// Called before method UtilDataAccessLayer.ValueFromText();
        /// </summary>
        protected virtual internal void CellValueFromText(string gridName, string index, Cell cell, ref string result)
        {

        }

        internal AppJson Run(AppJson appJson, HttpContext httpContext)
        {
            this.AppJson = appJson;
            if (AppJson == null || AppJson.Session == null) // First request.
            {
                int requestCount = AppJson != null ? AppJson.RequestCount : 0;
                AppJson = new AppJson();
                AppJson.RequestCount = requestCount;
                AppJson.Session = Guid.NewGuid();
                AppJson.RequestUrl = string.Format("{0}://{1}/", httpContext.Request.Scheme, httpContext.Request.Host.Value);
                GridData().SaveJson(this); // Initialize AppJson.GridDataJson object.
                Type typePage = TypePageMain();
                PageShow(AppJson, typePage);
            }
            //
            foreach (Process process in processList)
            {
                process.Run(this);
            }
            //
            AppJson.ResponseCount += 1;
            AppJson.VersionServer = Framework.UtilFramework.VersionServer;
            //
            return AppJson;
        }

        private GridData gridData;

        /// <summary>
        /// Returns GridData. It makes sure method GridData.LoadJson(); has been called. It's called only once.
        /// </summary>
        public GridData GridData()
        {
            if (gridData == null)
            {
                gridData = new GridData();
                gridData.LoadJson(AppJson, this);
            }
            return gridData;
        }

        private bool isGridDataTextParse;

        /// <summary>
        /// Make sure method GridData.Text(); has been called. It's called only once.
        /// </summary>
        public void GridDataTextParse()
        {
            if (isGridDataTextParse == false)
            {
                isGridDataTextParse = true;
                GridData().TextParse(this);
            }
        }
        private ProcessList processListPrivate;

        private ProcessList processList
        {
            get
            {
                if (processListPrivate == null)
                {
                    processListPrivate = new ProcessList(this);
                }
                return processListPrivate;
            }
        }

        /// <summary>
        /// Override this method to register new process.
        /// </summary>
        protected virtual void ProcessInit(ProcessList processList)
        {
            processList.Add<ProcessPageBegin>();
            // Grid
            {
                processList.Add<ProcessGridIsClick>();
                processList.Add<ProcessGridOrderBy>();
                processList.Add<ProcessGridFilter>();
                processList.Add<ProcessGridLookUp>();
                processList.Add<ProcessGridSave>();
                processList.Add<ProcessGridCellButtonIsClick>();
                processList.Add<ProcessGridOrderByText>();
                processList.Add<ProcessGridFocusNull>();
                processList.Add<ProcessGridCellIsModifyFalse>();
                processList.Add<ProcessGridIsClickFalse>();
            }
            //
            processList.Add<ProcessButtonIsClickFalse>();
            processList.Add<ProcessLayout>();
            processList.Add<ProcessPageEnd>();
        }

        /// <summary>
        /// Returns currently visible page.
        /// </summary>
        public Page PageVisible(Component owner)
        {
            return owner.List.OfType<Page>().Where(item => item.IsHide == false).SingleOrDefault();
        }

        /// <summary>
        /// Show page. Creates new one if it doesn't exist.
        /// </summary>
        public Page PageShow(Component owner, Type typePage, bool isPageVisibleRemove = true)
        {
            Page pageVisible = PageVisible(owner);
            if (pageVisible != null)
            {
                owner.List.Remove(pageVisible);
            }
            var list = owner.List.OfType<Page>();
            foreach (Page page in list)
            {
                page.IsHide = true;
            }
            Page result = owner.List.OfType<Page>().Where(item => item.GetType() == typePage).SingleOrDefault(); // Make sure there is only one page of type!
            if (result == null)
            {
                result = (Page)Activator.CreateInstance(typePage);
                result.Constructor(owner, null, typeof(Div));
                result.InitJson(this);
            }
            result.IsHide = false;
            return result;
        }

        /// <summary>
        /// Show page. Creates new one if it doesn't exist.
        /// </summary>
        public TPage PageShow<TPage>(Component owner, bool isPageVisibleRemove = true) where TPage : Page, new()
        {
            return (TPage)PageShow(owner, typeof(TPage), isPageVisibleRemove);
        }
    }
}
