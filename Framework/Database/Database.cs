﻿using System;
using System.Collections.Generic;
using Framework.Application;
using Framework.DataAccessLayer;
using System.Linq;

namespace Database.dbo
{
    public partial class FrameworkFileStorage
    {
        [SqlColumn(null, typeof(FrameworkFileStorage_Download))]
        public string Download { get; set; }
    }

    public partial class FrameworkFileStorage_Download : Cell<FrameworkFileStorage>
    {
        protected override internal void CellIsHtml(App app, string gridName, string index, ref bool result)
        {
            result = true;
        }

        protected override internal void CellValueToText(App app, string gridName, string index, ref string result)
        {
            result = null;
            if (UtilApplication.IndexEnumFromText(index) == IndexEnum.Index && Row.Name != null)
            {
                string fileNameOnly = Row.Name;
                if (Row.Name.Contains("/"))
                {
                    fileNameOnly = Row.Name.Substring(Row.Name.LastIndexOf("/") + 1);
                    if (fileNameOnly.Length == 0)
                    {
                        fileNameOnly = Row.Name;
                    }
                }
                result = string.Format("<a href={0} target='blank'>{1}</a>", Row.Name, fileNameOnly);
            }
        }
    }

    public partial class FrameworkFileStorage
    {
        protected internal override void Insert(App app, ref Row rowRefresh)
        {
            if (app.DbFrameworkApplication != null)
            {
                this.ApplicationId = app.DbFrameworkApplication.Id;
            }
            base.Insert(app, ref rowRefresh);
        }
    }

    public partial class FrameworkFileStorage_Data : Cell<FrameworkFileStorage>
    {
        protected override internal void CellIsFileUpload(App app, string gridName, string index, ref bool result)
        {
            result = true;
        }

        protected override internal void CellValueToText(App app, string gridName, string index, ref string result)
        {
            result = "File Upload";
        }
    }

    public partial class FrameworkApplicationView
    {
        protected internal override void Update(App app, Row row, Row rowNew, ref Row rowRefresh)
        {
            // Row
            var application = new FrameworkApplication();
            UtilDataAccessLayer.RowCopy(row, application);
            // RowNew
            var applicationNew = new FrameworkApplication();
            UtilDataAccessLayer.RowCopy(rowNew, applicationNew);
            //
            UtilDataAccessLayer.Update(application, applicationNew);
            rowRefresh = UtilDataAccessLayer.Select<FrameworkApplicationView>().Where(item => item.Id == this.Id).First();
        }

        protected internal override void Insert(App app, ref Row rowRefresh)
        {
            var application = new FrameworkApplication();
            UtilDataAccessLayer.RowCopy(this, application);
            UtilDataAccessLayer.Insert(application);
            rowRefresh = UtilDataAccessLayer.Select<FrameworkApplicationView>().Where(item => item.Id == application.Id).First();
        }

        protected internal override void Select()
        {
            UtilDataAccessLayer.RowCopy(UtilDataAccessLayer.Select<FrameworkApplicationView>().Where(item => item.Id == this.Id).First(), this);
        }
    }

    public partial class FrameworkApplicationView_Type
    {
        protected internal override void CellTextParse(App app, string gridName, string index, ref string result)
        {
            string text = result;
            var applicationType = UtilDataAccessLayer.Select<FrameworkApplicationType>().Where(item => item.Name == text).FirstOrDefault();
            if (applicationType == null)
            {
                throw new Exception(string.Format("Type unknown! ({0})", result));
            }
            Row.ApplicationTypeId = applicationType.Id;
        }
    }

    public partial class FrameworkApplicationView_Type
    {
        protected internal override void CellLookUp(out Type typeRow, out List<Row> rowList)
        {
            typeRow = typeof(FrameworkApplicationType);
            rowList = UtilDataAccessLayer.Select(typeRow, null, null, false, 0, 5);
        }

        protected internal override void CellLookUpIsClick(Row row, ref string result)
        {
            result = ((FrameworkApplicationType)row).Name;
        }
    }

    public partial class FrameworkConfigColumnView
    {
        protected internal override void Update(App app, Row row, Row rowNew, ref Row rowRefresh)
        {
            FrameworkConfigColumn config = UtilDataAccessLayer.Select<FrameworkConfigColumn>().Where(item => item.Id == this.ConfigId).FirstOrDefault();
            if (config == null)
            {
                config = new FrameworkConfigColumn();
                UtilDataAccessLayer.RowCopy(rowNew, config);
                UtilDataAccessLayer.Insert(config);
            }
            else
            {
                FrameworkConfigColumn configNew = UtilDataAccessLayer.RowClone(config);
                UtilDataAccessLayer.RowCopy(rowNew, configNew);
                UtilDataAccessLayer.Update(config, configNew);
            }
            rowRefresh = UtilDataAccessLayer.Select<FrameworkConfigColumnView>().Where(item => item.ColumnId == this.ColumnId).First();
        }
    }

    public partial class FrameworkConfigTableView
    {
        protected internal override void Update(App app, Row row, Row rowNew, ref Row rowRefresh)
        {
            var config = UtilDataAccessLayer.Select<FrameworkConfigTable>().Where(item => item.Id == this.ConfigId).FirstOrDefault();
            if (config == null)
            {
                config = new FrameworkConfigTable();
                UtilDataAccessLayer.RowCopy(rowNew, config);
                UtilDataAccessLayer.Insert(config);
            }
            else
            {
                FrameworkConfigTable configNew = UtilDataAccessLayer.RowClone(config);
                UtilDataAccessLayer.RowCopy(rowNew, configNew);
                UtilDataAccessLayer.Update(config, configNew);
            }
            rowRefresh = UtilDataAccessLayer.Select<FrameworkConfigTableView>().Where(item => item.TableId == this.TableId).First();
        }
    }
}