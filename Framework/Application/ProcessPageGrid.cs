﻿namespace Framework.Application
{
    using Framework.Component;
    using Framework.DataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Process OrderBy click.
    /// </summary>
    internal class ProcessGridOrderBy : Process
    {
        private void DatabaseLoad(App app, AppJson appJson, string gridName, string fieldNameOrderBy, bool isOrderByDesc)
        {
            GridDataJson gridDataJson = appJson.GridDataJson;
            //
            GridData gridData = app.GridData;
            Type typeRow = gridData.TypeRow(gridName);
            gridData.LoadDatabase(gridName, null, fieldNameOrderBy, isOrderByDesc, typeRow);
            gridData.SaveJson();
        }

        protected internal override void Run(App app)
        {
            AppJson appJson = app.AppJson;
            // Detect OrderBy click
            foreach (string gridName in appJson.GridDataJson.ColumnList.Keys.ToArray())
            {
                foreach (GridColumn gridColumn in appJson.GridDataJson.ColumnList[gridName])
                {
                    if (gridColumn.IsClick)
                    {
                        GridQuery gridQuery = appJson.GridDataJson.GridQueryList[gridName];
                        if (gridQuery.FieldNameOrderBy == gridColumn.FieldName)
                        {
                            gridQuery.IsOrderByDesc = !gridQuery.IsOrderByDesc;
                        }
                        else
                        {
                            gridQuery.FieldNameOrderBy = gridColumn.FieldName;
                            gridQuery.IsOrderByDesc = false;
                        }
                        DatabaseLoad(app, appJson, gridName, gridQuery.FieldNameOrderBy, gridQuery.IsOrderByDesc);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set OrderBy up or down arrow.
    /// </summary>
    internal class ProcessGridOrderByText : Process
    {
        protected internal override void Run(App app)
        {
            AppJson appJson = app.AppJson;
            //
            foreach (string gridName in appJson.GridDataJson.ColumnList.Keys)
            {
                GridQuery gridQuery = appJson.GridDataJson.GridQueryList[gridName];
                foreach (GridColumn gridColumn in appJson.GridDataJson.ColumnList[gridName])
                {
                    gridColumn.IsClick = false;
                    if (gridColumn.FieldName == gridQuery.FieldNameOrderBy)
                    {
                        if (gridQuery.IsOrderByDesc)
                        {
                            gridColumn.Text = "▼" + gridColumn.Text;
                        }
                        else
                        {
                            gridColumn.Text = "▲" + gridColumn.Text;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Process data grid filter.
    /// </summary>
    internal class ProcessGridFilter : Process
    {
        protected internal override void Run(App app)
        {
            AppJson appJson = app.AppJson;
            //
            List<string> gridNameList = new List<string>();
            foreach (string gridName in appJson.GridDataJson.ColumnList.Keys)
            {
                foreach (GridRow gridRow in appJson.GridDataJson.RowList[gridName])
                {
                    if (new Index(gridRow.Index).Enum == IndexEnum.Filter)
                    {
                        foreach (GridColumn gridColumn in appJson.GridDataJson.ColumnList[gridName])
                        {
                            GridCell gridCell = appJson.GridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                            if (gridCell.IsModify)
                            {
                                if (!gridNameList.Contains(gridName))
                                {
                                    gridNameList.Add(gridName);
                                }
                            }
                        }
                    }
                }
            }
            //
            foreach (string gridName in gridNameList)
            {
                app.GridDataTextParse();
                GridData gridData = app.GridData;
                gridData.LoadDatabaseReload(gridName);
                gridData.SaveJson();
            }
        }
    }

    /// <summary>
    /// Grid row or cell is clicked. Set focus.
    /// </summary>
    internal class ProcessGridIsClick : Process
    {
        private void ProcessGridSelectRowClear(AppJson appJson, string gridName)
        {
            foreach (GridRow gridRow in appJson.GridDataJson.RowList[gridName])
            {
                gridRow.IsSelectSet(false);
            }
        }

        private void ProcessGridSelectCell(AppJson appJson, string gridName, Index index, string fieldName)
        {
            GridDataJson gridDataJson = appJson.GridDataJson;
            //
            gridDataJson.FocusGridNamePrevious = gridDataJson.FocusGridName;
            gridDataJson.FocusIndexPrevious = gridDataJson.FocusIndex;
            gridDataJson.FocusFieldNamePrevious = gridDataJson.FocusFieldName;
            //
            gridDataJson.FocusGridName = gridName;
            gridDataJson.FocusIndex = index.Value;
            gridDataJson.FocusFieldName = fieldName;
            ProcessGridSelectCellClear(appJson);
            gridDataJson.CellList[gridName][fieldName][index.Value].IsSelect = true;
        }

        private void ProcessGridSelectCellClear(AppJson appJson)
        {
            GridDataJson gridDataJson = appJson.GridDataJson;
            foreach (string gridName in gridDataJson.RowList.Keys)
            {
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        gridCell.IsSelect = false;
                    }
                }
            }
        }

        protected internal override void Run(App app)
        {
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            foreach (GridQuery gridQuery in gridDataJson.GridQueryList.Values)
            {
                string gridName = gridQuery.GridName;
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    if (gridRow.IsClick)
                    {
                        ProcessGridSelectRowClear(app.AppJson, gridName);
                        gridRow.IsSelectSet(true);
                    }
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        if (gridCell.IsClick == true)
                        {
                            ProcessGridSelectCell(app.AppJson, gridName, new Index(gridRow.Index), gridColumn.FieldName);
                        }
                    }
                }
            }
        }
    }

    internal class ProcessGridIsClickMasterDetail : Process
    {
        private void MasterDetailIsClick(App app, string gridNameMaster, Row rowMaster)
        {
            GridData gridData = app.GridData;
            foreach (string gridName in gridData.GridNameList())
            {
                Type typeRow = gridData.TypeRow(gridName);
                Row rowTable = UtilDataAccessLayer.RowCreate(typeRow); // RowTable is the API. No data in record!
                bool isReload = false;
                rowTable.MasterDetail(app, gridNameMaster, rowMaster, ref isReload);
                if (isReload)
                {
                    gridData.LoadDatabaseReload(gridName);
                }
            }
        }

        protected internal override void Run(App app)
        {
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            foreach (GridQuery gridQuery in gridDataJson.GridQueryList.Values)
            {
                string gridName = gridQuery.GridName;
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    if (gridRow.IsClick)
                    {
                        Index gridRowIndex = new Index(gridRow.Index);
                        if (gridRowIndex.Enum == IndexEnum.Index)
                        {
                            GridData gridData = app.GridData;
                            var row = gridData.Row(gridName, gridRowIndex);
                            MasterDetailIsClick(app, gridName, row);
                            break;
                        }
                    }
                }
            }
        }
     }

    /// <summary>
    /// Save GridData back to json.
    /// </summary>
    internal class ProcessGridSaveJson : Process
    {
        protected internal override void Run(App app)
        {
            app.GridData.SaveJson();
        }
    }

    /// <summary>
    /// Set row and cell IsClick to false
    /// </summary>
    internal class ProcessGridIsClickFalse : Process
    {
        protected internal override void Run(App app)
        {
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            foreach (GridQuery gridQuery in gridDataJson.GridQueryList.Values)
            {
                string gridName = gridQuery.GridName;
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    gridRow.IsClick = false;
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        gridCell.IsClick = false;
                    }
                }
            }
            //
            gridDataJson.FocusGridNamePrevious = null;
            gridDataJson.FocusIndexPrevious = null;
            gridDataJson.FocusFieldNamePrevious = null;
        }
    }

    internal class ProcessGridCellIsModifyFalse : Process
    {
        protected internal override void Run(App app)
        {
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            //
            foreach (string gridName in gridDataJson.RowList.Keys)
            {
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        if (gridCell.IsModify)
                        {
                            gridCell.IsModify = false;
                        }
                    }
                }
            }
        }
    }

    internal class ProcessGridLookUpIsClick : Process
    {
        protected internal override void Run(App app)
        {
            Row rowLookUp = null;
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            foreach (string gridName in gridDataJson.RowList.Keys)
            {
                if (gridName == "LookUp")
                {
                    foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                    {
                        if (gridRow.IsClick)
                        {
                            Index gridRowIndex = new Index(gridRow.Index);
                            if (gridRowIndex.Enum == IndexEnum.Index)
                            {
                                GridData gridData = app.GridData;
                                rowLookUp = gridData.Row("LookUp", gridRowIndex);
                            }
                        }
                    }
                }
            }
            //
            if (rowLookUp != null)
            {
                GridData gridData = app.GridData;
                var row = gridData.Row(gridDataJson.FocusGridNamePrevious, new Index(gridDataJson.FocusIndexPrevious));
                Cell cell = UtilDataAccessLayer.CellList(row.GetType(), row).Where(item => item.FieldNameCSharp == gridDataJson.FocusFieldNamePrevious).First();
                Cell cellLookUp = UtilDataAccessLayer.CellList(rowLookUp.GetType(), rowLookUp).Where(item => item.FieldNameCSharp == gridDataJson.FocusFieldName).First();
                string result = cellLookUp.Value.ToString();
                cell.CellLookUpIsClick(rowLookUp, ref result);
                GridCell gridCell = gridDataJson.CellList[gridDataJson.FocusGridNamePrevious][gridDataJson.FocusFieldNamePrevious][gridDataJson.FocusIndexPrevious];
                gridCell.IsModify = true;
                gridCell.IsO = true;
                gridCell.O = gridCell.T;
                gridCell.T = result;
                gridData.LoadJson();
            }
        }
    }

    /// <summary>
    /// Open LookUp grid.
    /// </summary>
    internal class ProcessGridLookUp : Process
    {
        protected internal override void Run(App app)
        {
            bool isLookUp = false;
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            foreach (string gridName in gridDataJson.RowList.Keys)
            {
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        if (gridCell.IsClick || gridCell.IsModify)
                        {
                            isLookUp = true;
                            break;
                        }
                    }
                }
            }
            //
            if (isLookUp)
            {
                if (gridDataJson.FocusFieldName != null)
                {
                    GridData gridData = app.GridData;
                    Type typeRow = gridData.TypeRow(gridDataJson.FocusGridName);
                    var row = gridData.Row(gridDataJson.FocusGridName, new Index(gridDataJson.FocusIndex));
                    Cell cell = UtilDataAccessLayer.CellList(typeRow, row).Where(item => item.FieldNameCSharp == gridDataJson.FocusFieldName).First();
                    List<Row> rowList;
                    cell.CellLookUp(out typeRow, out rowList);
                    gridData.LoadRow("LookUp", typeRow, rowList);
                    gridData.SaveJson();
                }
            }
        }
    }

    /// <summary>
    /// Set focus to null, if cell does not exist anymore.
    /// </summary>
    internal class ProcessGridFocusNull : Process
    {
        protected internal override void Run(App app)
        {
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            bool isExist = false; // Focused field exists
            if (gridDataJson.FocusFieldName != null)
            {
                if (gridDataJson.RowList[gridDataJson.FocusGridName].Exists(item => item.Index == gridDataJson.FocusIndex)) // Focused row exists
                {
                    if (gridDataJson.ColumnList[gridDataJson.FocusGridName].Exists(item => item.FieldName == gridDataJson.FocusFieldName)) // Focused column exists
                    {
                        isExist = true;
                    }
                }
            }
            if (isExist == false)
            {
                if (app.AppJson.GridDataJson != null)
                {
                    app.AppJson.GridDataJson.FocusFieldName = null;
                    app.AppJson.GridDataJson.FocusGridName = null;
                    app.AppJson.GridDataJson.FocusIndex = null;
                }
            }
        }
    }

    internal class ProcessGridSave : Process
    {
        protected internal override void Run(App app)
        {
            bool isSave = false;
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            foreach (string gridName in gridDataJson.RowList.Keys)
            {
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        if (gridCell.IsModify)
                        {
                            isSave = true;
                            break;
                        }
                    }
                }
            }
            //
            if (isSave)
            {
                app.GridData.TextParse();
                app.GridData.SaveDatabase();
                app.GridData.SaveJson();
            }
        }
    }

    /// <summary>
    /// Cell rendered as button is clicked.
    /// </summary>
    internal class ProcessGridCellButtonIsClick : Process
    {
        protected internal override void Run(App app)
        {
            GridDataJson gridDataJson = app.AppJson.GridDataJson;
            //
            string gridNameClick = null;
            string indexClick = null;
            string fieldNameClick = null;
            foreach (string gridName in gridDataJson.RowList.Keys)
            {
                foreach (GridRow gridRow in gridDataJson.RowList[gridName])
                {
                    foreach (var gridColumn in gridDataJson.ColumnList[gridName])
                    {
                        GridCell gridCell = gridDataJson.CellList[gridName][gridColumn.FieldName][gridRow.Index];
                        if (gridCell.IsModify && gridCell.CellEnum == GridCellEnum.Button)
                        {
                            gridNameClick = gridName;
                            indexClick = gridRow.Index;
                            fieldNameClick = gridColumn.FieldName;
                            break;
                        }
                    }
                }
            }
            //
            if (gridNameClick != null)
            {
                Row row = app.GridData.Row(gridNameClick, new Index(indexClick));
                Type typeRow = app.GridData.TypeRow(gridNameClick);
                Cell cell = UtilDataAccessLayer.CellList(typeRow, row).Where(item => item.FieldNameCSharp == fieldNameClick).Single();
                bool isReload = false;
                bool isException = false;
                try
                {
                    cell.CellButtonIsClick(app, gridNameClick, new Index(indexClick), row, fieldNameClick, ref isReload);
                }
                catch (Exception exception)
                {
                    isException = true;
                    app.GridData.ErrorRowSet(gridNameClick, new Index(indexClick), UtilFramework.ExceptionToText(exception));
                }
                if (isReload && isException == false)
                {
                    app.GridData.LoadDatabaseReload(gridNameClick);
                }
            }
        }
    }
}