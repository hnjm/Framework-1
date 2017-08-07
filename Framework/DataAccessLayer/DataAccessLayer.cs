﻿namespace Framework.DataAccessLayer
{
    using Framework.Application;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Base class for every database row.
    /// </summary>
    public class Row
    {
        protected virtual bool IsReadOnly()
        {
            return false;
        }

        protected virtual internal void Update(App app, Row row)
        {
            UtilDataAccessLayer.Update(row, this);
        }

        /// <summary>
        /// Override this method for example to save data to underlying database tables from sql view.
        /// </summary>
        protected virtual internal void Insert(App app)
        {
            UtilDataAccessLayer.Insert(this);
        }

        /// <summary>
        /// Override this method to reload record after update or insert.
        /// </summary>
        protected virtual internal void Select()
        {

        }
    }

    /// <summary>
    /// Base class for every database field.
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Constructor for column.
        /// </summary>
        internal void Constructor(string tableNameSql, string fieldNameSql, string fieldNameCSharp, Type typeRow, Type typeField, PropertyInfo propertyInfo)
        {
            this.TableNameSql = tableNameSql;
            this.FieldNameSql = fieldNameSql;
            this.FieldNameCSharp = fieldNameCSharp;
            this.TypeRow = typeRow;
            this.TypeField = typeField;
            this.PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Constructor for column and cell. Switch between column and cell mode. (Column mode: row = null; Cell mode: row != null).
        /// </summary>
        internal Cell Constructor(object row)
        {
            this.Row = row;
            return this;
        }

        /// <summary>
        /// Gets sql TableName.
        /// </summary>
        public string TableNameSql { get; private set; }


        /// <summary>
        /// Gets sql FieldName. If null, then it's a calculated column.
        /// </summary>
        public string FieldNameSql { get; private set; }

        /// <summary>
        /// Gets Csharp FieldName.
        /// </summary>
        public string FieldNameCSharp { get; private set; }

        /// <summary>
        /// Gets TypeRow.
        /// </summary>
        public Type TypeRow { get; private set; }

        /// <summary>
        /// Gets TypeField.
        /// </summary>
        public Type TypeField { get; private set; }

        internal PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets Row. Null, if column.
        /// </summary>
        public object Row { get; private set; }

        protected virtual internal void CellIsReadOnly(ref bool result)
        {

        }

        protected virtual internal void ColumnText(ref string result)
        {

        }

        /// <summary>
        /// Parse user entered text.
        /// </summary>
        protected virtual internal void CellTextParse(App app, string gridName, string index, ref string result)
        {

        }

        /// <summary>
        /// Override for custom formatting like adding units of measurement. Called after method UtilDataAccessLayer.ValueToText();  Inverse function is CellValueFromText.
        /// </summary>
        protected virtual internal void CellValueToText(App app, string gridName, string index, ref string result)
        {

        }

        /// <summary>
        /// Override to parse custom formating like value with units of measurement. Called before user entered text is parsed with method UtilDataAccessLayer.ValueFromText(); Inverse function is CellValueToText.
        /// </summary>
        protected virtual internal void CellValueFromText(App app, string gridName, string index, ref string result)
        {
            
        }

        /// <summary>
        /// Returns true, if data cell is to be rendered as button. Override method CellProcessButtonIsClick(); to process click event.
        /// </summary>
        protected virtual internal void CellIsButton(App app, string gridName, string index, ref bool result)
        {

        }

        protected virtual internal void CellIsHtml(App app, string gridName, string index, ref bool result)
        {

        }

        protected virtual internal void CellIsFileUpload(App app, string gridName, string index, ref bool result)
        {

        }

        protected virtual internal void ColumnWidthPercent(ref double widthPercent)
        {

        }

        protected virtual internal void CellLookUp(out Type typeRow, out List<Row> rowList)
        {
            typeRow = null;
            rowList = null;
        }

        /// <summary>
        /// Override to handle clicked LookUp row.
        /// </summary>
        /// <param name="row">LoowUp row which has been clicked.</param>
        protected virtual internal void CellLookUpIsClick(Row row, ref string result)
        {

        }

        protected virtual internal void ColumnIsVisible(ref bool isVisible)
        {

        }

        protected virtual internal void ColumnIsReadOnly(ref bool result)
        {

        }

        protected virtual internal void CellProcessButtonIsClick(App app, string gridName, string index, string fieldName)
        {

        }

        /// <summary>
        /// Gets or sets Value. Throws exception if cell is in column mode.
        /// </summary>
        public object Value
        {
            get
            {
                UtilFramework.Assert(Row != null, "Column mode!");
                return PropertyInfo.GetValue(Row);
            }
            set
            {
                UtilFramework.Assert(Row != null, "Column mode!");
                PropertyInfo.SetValue(Row, value);
            }
        }
    }

    /// <summary>
    /// Base class for every database field.
    /// </summary>
    public class Cell<TRow> : Cell
    {
        public new TRow Row
        {
            get
            {
                return (TRow)base.Row;
            }
        }
    }

    /// <summary>
    /// Sql table name and field name.
    /// </summary>
    public class SqlNameAttribute : Attribute
    {
        public SqlNameAttribute(string sqlName)
        {
            this.SqlName = sqlName;
        }

        public readonly string SqlName;
    }

    public class TypeCellAttribute : Attribute
    {
        public TypeCellAttribute(Type typeCell)
        {
            Framework.UtilFramework.Assert(typeCell.GetTypeInfo().IsSubclassOf(typeof(Cell)));
            this.TypeCell = typeCell;
        }

        public readonly Type TypeCell;
    }
}
