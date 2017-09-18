﻿namespace Framework.Application
{
    using Framework.Component;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class Process
    {
        protected virtual internal void Run(App app)
        {

        }
    }

    internal class ProcessList : IEnumerable<Process>
    {
        internal ProcessList(App app)
        {
            this.App = app;
        }

        public readonly App App;

        private List<Process> processList = new List<Process>();

        public IEnumerator<Process> GetEnumerator()
        {
            return processList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return processList.GetEnumerator();
        }

        private Process ProcessListInsert(Type typeProcess, Type typeProcessFind, bool isAfter)
        {
            // Already exists?
            foreach (Process process in processList)
            {
                UtilFramework.Assert(process.GetType() != typeProcess, "Page already contains process!");
            }
            // Create process
            Process result = (Process)UtilFramework.TypeToObject(typeProcess);
            if (typeProcessFind == null)
            {
                processList.Add(result);
            }
            else
            {
                int index = -1;
                bool isFind = false;
                // Find process
                foreach (Process process in processList)
                {
                    index += 1;
                    if (process.GetType() == typeProcessFind)
                    {
                        isFind = true;
                        break;
                    }
                }
                UtilFramework.Assert(isFind, "Process not found!");
                if (isAfter)
                {
                    index += 1;
                }
                processList.Insert(index, result);
            }
            return result;
        }

        /// <summary>
        /// Create process for this page.
        /// </summary>
        public TProcess Add<TProcess>() where TProcess : Process
        {
            return (TProcess)ProcessListInsert(typeof(TProcess), null, false);
        }

        public TProcess AddBefore<TProcess, TProcessFind>() where TProcess : Process where TProcessFind : Process
        {
            return (TProcess)ProcessListInsert(typeof(TProcess), typeof(TProcessFind), false);
        }

        public TProcess AddAfter<TProcess, TProcessFind>() where TProcess : Process where TProcessFind : Process
        {
            return (TProcess)ProcessListInsert(typeof(TProcess), typeof(TProcessFind), true);
        }

        /// <summary>
        /// Returns process of this page.
        /// </summary>
        public T Get<T>() where T : Process
        {
            return (T)processList.Where(item => item.GetType() == typeof(T)).FirstOrDefault();
        }
    }

    /// <summary>
    /// Set Button.IsClick to false.
    /// </summary>
    internal class ProcessButtonIsClickFalse : Process
    {
        protected internal override void Run(App app)
        {
            foreach (Button button in app.AppJson.ListAll().OfType<Button>())
            {
                button.IsClick = false;
            }
        }
    }

    /// <summary>
    /// Call method Page.ProcessBegin(); at the begin of the process chain.
    /// </summary>
    internal class ProcessPageBegin : Process
    {
        protected internal override void Run(App app)
        {
            foreach (var page in app.AppJson.ListAll().OfType<Page>())
            {
                page.RunBegin(app);
            }
        }
    }

    /// <summary>
    /// Call method Page.ProcessEnd(); at the End of the process chain.
    /// </summary>
    internal class ProcessPageEnd : Process
    {
        protected internal override void Run(App app)
        {
            foreach (var page in app.AppJson.ListAll().OfType<Page>())
            {
                page.RunEnd();
            }
        }
    }

    /// <summary>
    /// Call method Page.ProcessEnd(); at the End of the process chain.
    /// </summary>
    internal class ProcessLayout : Process
    {
        private void ValidateTwelve(LayoutRow layoutRow)
        {
            int widthTotal = 0;
            bool isCell = false;
            foreach (LayoutCell cell in layoutRow.List.OfType<LayoutCell>())
            {
                isCell = true;
                string find = "col-sm-";
                int index = cell.Css.IndexOf(find);
                UtilFramework.Assert(index != -1, "Cell width not defined!");
                string widthString = null;
                index += find.Length;
                while (index < cell.Css.Length && cell.Css[index] >= '0' && cell.Css[index] <= '9')
                {
                    widthString += cell.Css[index].ToString();
                    index += 1;
                }
                int width = int.Parse(widthString);
                widthTotal += width;
            }
            if (isCell) // Not an empty layout row.
            {
                UtilFramework.Assert(widthTotal == 12, "Css width total is not 12!");
            }
        }

        protected internal override void Run(App app)
        {
            foreach (Component component in app.AppJson.ListAll())
            {
                LayoutContainer layoutContainer = component as LayoutContainer;
                if (layoutContainer != null && !layoutContainer.Css.Contains("container"))
                {
                    layoutContainer.Css = "container " + layoutContainer.Css;
                }
                LayoutRow layoutRow = component as LayoutRow;
                if (layoutRow != null)
                {
                    if (layoutRow.Css == null || !layoutRow.Css.Contains("row"))
                    {
                        layoutRow.Css = "row " + layoutRow.Css;
                    }
                    ValidateTwelve(layoutRow);
                }
            }
        }
    }
}
