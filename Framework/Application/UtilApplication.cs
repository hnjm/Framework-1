﻿namespace Framework.Application
{
    using Database.dbo;
    using Framework.Component;
    using Framework.DataAccessLayer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class ApplicationEventArgument
    {
        internal ApplicationEventArgument(App app, GridName gridName, Index index, string columnName)
        {
            this.App = app;
            this.GridName = gridName;
            this.Index = index;
            this.ColumnName = columnName;
        }

        public readonly App App;

        public readonly GridName GridName;

        public readonly Index Index;

        public readonly string ColumnName;
    }

    /// <summary>
    /// GridName is a Name or the name of a TypeRow or combined.
    /// </summary>
    public class GridName
    {
        internal GridName(string name, Type typeRow, bool isNameExclusive)
        {
            UtilFramework.Assert(Name == null || !Name.Contains("."));
            //
            this.Name = name;
            this.TypeRowInternal = typeRow;
            //
            if (isNameExclusive == false)
            {
                this.Name = UtilDataAccessLayer.TypeRowToTableNameCSharp(TypeRowInternal) + "." + Name;
            }
            //
            if (UtilFramework.IsSubclassOf(this.GetType(), typeof(GridNameTypeRow)))
            {
                UtilFramework.Assert(this.TypeRowInternal != null);
            }
        }

        public GridName(string name) 
            : this(name, null, true)
        {

        }

        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets TypeRowInternal. Not null only in derived class GridNameTypeRow.
        /// </summary>
        internal readonly Type TypeRowInternal;

        /// <summary>
        /// Gets GridName without TypeRow.
        /// </summary>
        internal string NameExclusive
        {
            get
            {
                string result = Name.Substring(Name.LastIndexOf(".") + ".".Length);
                if (result == "")
                {
                    result = null;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets IsNameExclusive. If true, Name is not combined with TypeRow.
        /// </summary>
        public bool IsNameExclusive
        {
            get
            {
                return Name == NameExclusive;
            }
        }

        /// <summary>
        /// Returns GridName or GridNameTypeRow as json string. In case of GridNameTypeRow type information gets lost!
        /// </summary>
        internal static string ToJson(GridName gridName)
        {
            return gridName.Name;
        }

        /// <summary>
        /// Returns GridName loaded from json. It never returns a GridNameTypeRow object.
        /// </summary>
        internal static GridName FromJson(string json)
        {
            GridName result = new GridName(null);
            result.Name = json;
            return result;
        }

        public override string ToString()
        {
            return base.ToString() + $" ({Name})";
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            GridName gridName = obj as GridName;
            if (gridName != null)
            {
                return object.Equals(Name, gridName.Name);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public static bool operator ==(GridName left, GridName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GridName left, GridName right)
        {
            return !Equals(left, right);
        }
    }

    public class GridNameTypeRow : GridName
    {
        public GridNameTypeRow(Type typeRow) 
            : base(null, typeRow, false)
        {

        }

        public GridNameTypeRow(Type typeRow, string name, bool isNameExclusive = false) 
            : base(name, typeRow, isNameExclusive)
        {

        }

        public GridNameTypeRow(Type typeRow, GridName gridName)
            : this(typeRow, gridName.NameExclusive, gridName.IsNameExclusive)
        {

        }

        public Type TypeRow
        {
            get
            {
                return base.TypeRowInternal;
            }
        }
    }

    public class GridName<TRow> : GridNameTypeRow where TRow : Row
    {
        public GridName()
            : base(typeof(TRow))
        {

        }

        public GridName(string name, bool isNameExclusive = false) 
            : base(typeof(TRow), name, isNameExclusive)
        {

        }
    }

    public enum IndexEnum
    {
        None = 0,
        Index = 1,
        Filter = 2,
        New = 3,
        Total = 4
    }

    /// <summary>
    /// Data grid row index.
    /// </summary>
    public class Index
    {
        public Index(string value)
        {
            this.Value = value;
            this.Enum = UtilApplication.IndexEnumFromText(value);
        }

        private Index(IndexEnum value)
        {
            UtilFramework.Assert(value != IndexEnum.Index);
            this.Value = UtilApplication.IndexEnumToText(value);
            this.Enum = value;
        }

        [ThreadStatic]
        private static Index filter;

        /// <summary>
        /// Gets Filter. Index for Filter row.
        /// </summary>
        public static Index Filter
        {
            get
            {
                if (filter == null)
                {
                    filter = new Index(IndexEnum.Filter);
                }
                return filter;
            }
        }

        [ThreadStatic]
        private static Index _new;

        /// <summary>
        /// Gets New. Index for New row.
        /// </summary>
        public static Index New
        {
            get
            {
                if (_new == null)
                {
                    _new = new Index(IndexEnum.New);
                }
                return _new;
            }
        }

        [ThreadStatic]
        private static Index indexTotal;

        /// <summary>
        /// Gets Total. Index for Total row.
        /// </summary>
        public static Index IndexTotal
        {
            get
            {
                if (indexTotal == null)
                {
                    indexTotal = new Index(IndexEnum.Total);
                }
                return indexTotal;
            }
        }

        public readonly string Value;

        public readonly IndexEnum Enum;

        public override string ToString()
        {
            return base.ToString() + $" ({Value})";
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {

            Index index = obj as Index;
            if (index != null)
            {
                return object.Equals(Value, index.Value);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public static bool operator ==(Index left, Index right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Index left, Index right)
        {
            return !Equals(left, right);
        }
    }

    /// <summary>
    /// Html cascading style sheets information.
    /// </summary>
    public class ConfigCssClass
    {
        private List<string> valueList = new List<string>();

        public void Add(string value)
        {
            if (!valueList.Contains(value))
            {
                valueList.Add(value);
            }
        }

        public void Remove(string value)
        {
            if (valueList.Contains(value))
            {
                valueList.Remove(value);
            }
        }

        public void Clear()
        {
            valueList.Clear();
        }

        public string ToHtml()
        {
            if (valueList.Count == 0)
            {
                return null;
            }
            StringBuilder result = new StringBuilder();
            bool isFirst = false;
            foreach (var value in valueList)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    result.Append(" ");
                }
                result.Append(value);
            }
            return result.ToString();
        }
    }

    public class ConfigGrid
    {
        /// <summary>
        /// Gets or sets PageRowCount. Number of rows loaded for grid.
        /// </summary>
        public int PageRowCount;

        /// <summary>
        /// Gets or sets IsInsert. If true, grid allows insert of new row.
        /// </summary>
        public bool IsInsert;
    }

    public class ConfigColumn
    {
        /// <summary>
        /// Gets or sets Text. This is the grid column header.
        /// </summary>
        public string Text;

        /// <summary>
        /// Gets or sets IsReadOnly. Column read only flag.
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// Gets or sets IsVisible. If false, data is not shown but still transfered to client.
        /// </summary>
        public bool IsVisible;

        /// <summary>
        /// Gets WidthPercent. This is the column width.
        /// </summary>
        public double WidthPercent
        {
            get;
            internal set;
        }
    }

    public class ConfigCell
    {
        /// <summary>
        /// Gets or sets IsReadOnly.
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// Gets or sets CellEnum. If not rendered as default (null), cell can be rendered as Button, Html or FileUpload.
        /// </summary>
        public GridCellEnum? CellEnum;

        /// <summary>
        /// Gets or sets CssClass. Html cascading style sheets information for cell.
        /// </summary>
        public ConfigCssClass CssClass;

        /// <summary>
        /// Gets or sets PlaceHolder. For example "Search" for filter or "New" for new row, when no text is displayed in input cell.
        /// </summary>
        public string PlaceHolder;
    }

    /// <summary>
    /// Loads database tables FrameworkConfigGridView and FrameworkConfigColumnView.
    /// </summary>
    internal class ConfigInternal
    {
        public ConfigInternal(App app)
        {
            this.App = app;
            this.tableNameCSharpList = new List<string>();
            this.dbConfigGridList = new List<FrameworkConfigGridView>();
            this.dbConfigColumnList = new List<FrameworkConfigColumnView>();
            this.configGridList = new Dictionary<GridNameTypeRow, ConfigGrid>();
            this.configColumnList = new Dictionary<GridNameTypeRow, Dictionary<string, ConfigColumn>>();
        }

        public readonly App App;

        private string GridNameToTableNameCSharp(GridName gridName)
        {
            string result = null;
            GridNameTypeRow gridNameTypeRow = App.GridData.GridNameTypeRow(gridName);
            if (gridNameTypeRow != null)
            {
                result = UtilDataAccessLayer.TypeRowToTableNameCSharp(gridNameTypeRow.TypeRow);
            }
            return result;
        }

        /// <summary>
        /// Load Config for GridName and all other known GridName.
        /// </summary>
        public void LoadDatabaseConfig(GridName gridName)
        {
            // Add gridName
            List<string> tableNameCSharpList = new List<string>();
            string tableNameCSharp = GridNameToTableNameCSharp(gridName);
            tableNameCSharpList.Add(tableNameCSharp);
            // Add Grid (Also not yet loaded Grid)
            foreach (Grid grid in App.AppJson.ListAll().OfType<Grid>())
            {
                GridNameTypeRow gridNameTypeRow = grid.GridNameInternal as GridNameTypeRow;
                if (gridNameTypeRow != null)
                {
                    tableNameCSharp = UtilDataAccessLayer.TypeRowToTableNameCSharp(gridNameTypeRow.TypeRow);
                    tableNameCSharpList.Add(tableNameCSharp);
                }
            }
            // In GridData defined grids.
            foreach (GridNameTypeRow gridNameTypeRow in App.GridData.GridNameTypeRowList())
            {
                tableNameCSharp = UtilDataAccessLayer.TypeRowToTableNameCSharp(gridNameTypeRow.TypeRow);
                tableNameCSharpList.Add(tableNameCSharp);
            }
            //
            tableNameCSharpList = tableNameCSharpList.Except(this.tableNameCSharpList).Distinct().Where(item => item != null).ToList(); // Exclude already loaded TableNameCSharp
            this.tableNameCSharpList.AddRange(tableNameCSharpList);
            //
            if (tableNameCSharpList.Count > 0)
            {
                // Load grid config and column config in parallel.
                Task taskLoadGrid = new Task(() =>
                {
                    var configGridList = UtilDataAccessLayer.Query<FrameworkConfigGridView>().Where(item => tableNameCSharpList.Contains(item.TableNameCSharp)).ToList(); // IsExist is not filtered here.
                    this.dbConfigGridList.AddRange(configGridList);
                });
                Task taskLoadColumn = new Task(() =>
                {
                    var configColumnList = UtilDataAccessLayer.Query<FrameworkConfigColumnView>().Where(item => tableNameCSharpList.Contains(item.TableNameCSharp)).ToList(); // IsExist is not filtered here.
                    this.dbConfigColumnList.AddRange(configColumnList);
                });
                taskLoadGrid.Start();
                taskLoadColumn.Start();
                Task.WhenAll(taskLoadGrid, taskLoadColumn).Wait();
            }
        }

        /// <summary>
        /// (TableNameCSharp). Ignores GridName and IsExist on database load.
        /// </summary>
        private List<string> tableNameCSharpList;

        private List<FrameworkConfigGridView> dbConfigGridList;

        private List<FrameworkConfigColumnView> dbConfigColumnList;

        private Dictionary<GridNameTypeRow, ConfigGrid> configGridList;

        /// <summary>
        /// (GridName, ColumnNameCSharp, ConfigColumn).
        /// </summary>
        private Dictionary<GridNameTypeRow, Dictionary<string, ConfigColumn>> configColumnList;

        private void AssertLoadConfig(GridNameTypeRow gridName)
        {
            string tableNameCSharp = UtilDataAccessLayer.TypeRowToTableNameCSharp(gridName.TypeRow);
            if (!tableNameCSharpList.Contains(tableNameCSharp))
            {
                LoadDatabaseConfig(null); // Happens, if request did not cause any grid to load.
            }
            UtilFramework.Assert(tableNameCSharpList.Contains(tableNameCSharp));
        }

        /// <summary>
        /// Grid config.
        /// </summary>
        public ConfigGrid ConfigGridGet(GridNameTypeRow gridName)
        {
            AssertLoadConfig(gridName);
            //
            if (!this.configGridList.ContainsKey(gridName))
            {
                ConfigGrid result = new ConfigGrid() // Default
                {
                    PageRowCount = 15,
                    IsInsert = true
                };
                //
                string tableNameCSharp = UtilDataAccessLayer.TypeRowToTableNameCSharp(gridName.TypeRow);
                string gridNameString = gridName.NameExclusive;
                FrameworkConfigGridView dbConfigGrid = dbConfigGridList.Where(item => item.TableNameCSharp == tableNameCSharp && item.GridName == gridNameString && item.GridIsExist == true && item.TableIsExist == true).SingleOrDefault();
                if (dbConfigGrid != null)
                {
                    if (dbConfigGrid.PageRowCountDefault != null)
                    {
                        result.PageRowCount = dbConfigGrid.PageRowCountDefault.Value;
                    }
                    if (dbConfigGrid.PageRowCount != null)
                    {
                        result.PageRowCount = dbConfigGrid.PageRowCount.Value;
                    }
                    if (dbConfigGrid.IsInsertDefault != null)
                    {
                        result.IsInsert = dbConfigGrid.IsInsertDefault.Value;
                    }
                    if (dbConfigGrid.IsInsert != null)
                    {
                        if (dbConfigGrid.IsInsertDefault != false) // If factory setting is no insert, it cannot be overridden.
                        {
                            result.IsInsert = dbConfigGrid.IsInsert.Value;
                        }
                    }
                }
                Row row = UtilDataAccessLayer.RowCreate(gridName.TypeRow);
                // Override programmatically
                row.ConfigGrid(result, new ApplicationEventArgument(App, gridName, null, null));
                //
                this.configGridList[gridName] = result;
            }
            return this.configGridList[gridName];
        }

        /// <summary>
        /// Column config.
        /// </summary>
        public ConfigColumn ConfigColumnGet(GridNameTypeRow gridName, Cell column)
        {
            AssertLoadConfig(gridName);
            //
            if (!this.configColumnList.ContainsKey(gridName))
            {
                this.configColumnList[gridName] = new Dictionary<string, ConfigColumn>();
            }
            if (!this.configColumnList[gridName].ContainsKey(column.ColumnNameCSharp))
            {
                ConfigColumn result = new ConfigColumn() // Default
                {
                    Text = column.ColumnNameCSharp,
                    IsVisible = true
                };
                //
                string tableNameCSharp = UtilDataAccessLayer.TypeRowToTableNameCSharp(gridName.TypeRow);
                string gridNameString = gridName.NameExclusive;
                var dbList = dbConfigColumnList.Where(item => item.TableNameCSharp == tableNameCSharp && item.GridName == gridNameString);
                FrameworkConfigColumnView dbConfigColumn = dbList.Where(item => item.ColumnNameCSharp == column.ColumnNameCSharp && item.TableIsExist == true && item.GridIsExist == true && item.ColumnIsExist == true).SingleOrDefault();
                if (dbConfigColumn != null)
                {
                    if (dbConfigColumn.TextDefault != null)
                    {
                        result.Text = dbConfigColumn.TextDefault;
                    }
                    if (dbConfigColumn.Text != null)
                    {
                        result.Text = dbConfigColumn.Text;
                    }
                    if (dbConfigColumn.IsVisibleDefault != null)
                    {
                        result.IsVisible = dbConfigColumn.IsVisibleDefault.Value;
                    }
                    if (dbConfigColumn.IsVisible != null)
                    {
                        if (dbConfigColumn.IsVisibleDefault == false) // If factory setting is hide, it cannot be overridden.
                        {
                            result.IsVisible = dbConfigColumn.IsVisible.Value;
                        }
                    }
                    // IsReadOnly
                    if (dbConfigColumn.IsReadOnlyDefault != null)
                    {
                        result.IsReadOnly = dbConfigColumn.IsReadOnlyDefault.Value;
                    }
                    if (dbConfigColumn.IsReadOnly != null)
                    {
                        if (dbConfigColumn.IsReadOnlyDefault != true) // If factory setting IsReadOnly, it cannot be overridden.
                        {
                            result.IsReadOnly = dbConfigColumn.IsReadOnly.Value;
                        }
                    }
                }
                // Override programmatically
                var applicationEventArgument = new ApplicationEventArgument(App, gridName, null, null);
                App.CellConfigColumn(column, result, applicationEventArgument);
                column.ConfigColumn(result, applicationEventArgument);
                //
                this.configColumnList[gridName][column.ColumnNameCSharp] = result;
            }
            return this.configColumnList[gridName][column.ColumnNameCSharp];
        }

        /// <summary>
        /// Cell config.
        /// </summary>
        internal ConfigCell ConfigCellGet(GridNameTypeRow gridName, Index index, Cell cell)
        {
            AssertLoadConfig(gridName);
            //
            ConfigCell result = new ConfigCell() // Default
            {
                IsReadOnly = false,
                CellEnum = null, // GridCellEnum.None,
                CssClass = new ConfigCssClass(),
                PlaceHolder = null
            };
            switch (index.Enum)
            {
                case IndexEnum.Filter:
                    result.PlaceHolder = "Search";
                    result.CssClass.Add("gridFilter");
                    break;
                case IndexEnum.New:
                    result.PlaceHolder = "New";
                    result.CssClass.Add("gridNew");
                    break;
            }
            ConfigColumn configColumn = ConfigColumnGet(gridName, cell);
            if (configColumn.IsReadOnly)
            {
                result.IsReadOnly = true; // If column IsReadOnly, every cell IsReadOnly.
            }
            if (result.IsReadOnly)
            {
                if (index.Enum != IndexEnum.Filter) // No readonly for filter!
                {
                    result.CellEnum = GridCellEnum.Html;
                    result.CssClass.Remove("gridNew");
                    result.CssClass.Add("gridReadOnly");
                }
            }
            // Override programmatically
            if (index.Enum != IndexEnum.Filter)
            {
                cell.ConfigCell(result, new ApplicationEventArgument(App, gridName, index, cell.ColumnNameCSharp));
            }
            // IsReadOnly config
            if (result.IsReadOnly)
            {
                if (index.Enum != IndexEnum.Filter) // No readonly for filter!
                {
                    result.CellEnum = GridCellEnum.Html;
                }
            }
            return result;
        }
    }

    public static class UtilApplication
    {
        /// <summary>
        /// Bitwise (01=Select; 10=MouseOver; 11=Select and MouseOver).
        /// </summary>
        internal static bool IsSelectGet(int isSelect)
        {
            return (isSelect & 1) == 1;
        }

        /// <summary>
        /// Bitwise (01=Select; 10=MouseOver; 11=Select and MouseOver).
        /// </summary>
        internal static int IsSelectSet(int isSelect, bool value)
        {
            if (value)
            {
                isSelect = isSelect | 1;
            }
            else
            {
                isSelect = isSelect & 2;
            }
            return isSelect;
        }

        /// <summary>
        /// Returns list of available class App.
        /// </summary>
        internal static Type[] ApplicationTypeList(Type typeInAssembly)
        {
            List<Type> result = new List<Type>();
            foreach (Type itemTypeInAssembly in UtilFramework.TypeInAssemblyList(typeInAssembly))
            {
                foreach (var type in itemTypeInAssembly.GetTypeInfo().Assembly.GetTypes())
                {
                    if (UtilFramework.IsSubclassOf(type, typeof(App)))
                    {
                        result.Add(type);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Returns TypeRowInAssembly. This is a type in an assembly. Search for row class in this assembly when deserializing json. (For example: "dbo.Airport")
        /// </summary>
        internal static Type TypeRowInAssembly(App app)
        {
            return app.GetType();
        }

        internal static string IndexEnumToText(IndexEnum indexEnum)
        {
            return indexEnum.ToString();
        }

        internal static IndexEnum IndexEnumFromText(string index)
        {
            if (IndexEnumToText(IndexEnum.Filter) == index)
            {
                return IndexEnum.Filter;
            }
            if (IndexEnumToText(IndexEnum.New) == index)
            {
                return IndexEnum.New;
            }
            if (IndexEnumToText(IndexEnum.Total) == index)
            {
                return IndexEnum.Total;
            }
            int indexInt;
            if (int.TryParse(index, out indexInt))
            {
                return IndexEnum.Index;
            }
            return IndexEnum.None;
        }

        /// <summary>
        /// Returns true, if column name contains "Id" according default naming convention.
        /// </summary>
        internal static bool ConfigColumnNameSqlIsId(string columnNameSql)
        {
            bool result = false;
            if (columnNameSql != null)
            {
                int index = 0;
                while (index != -1)
                {
                    index = columnNameSql.IndexOf("Id", index);
                    if (index != -1)
                    {
                        index += "Id".Length;
                        if (index < columnNameSql.Length)
                        {
                            string text = columnNameSql.Substring(index, 1);
                            if (text.ToUpper() == text)
                            {
                                result = true;
                                break;
                            }
                        }
                        else
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Serialize GridName or GridNameTypeRow object.
        /// </summary>
        public static string GridNameToJson(GridName gridName)
        {
            return GridName.ToJson(gridName);
        }

        /// <summary>
        /// Deserialize GridName. Always returns a GridName object. Never a GridNameTypeRow object.
        /// </summary>
        public static GridName GridNameFromJson(string json)
        {
            return GridName.FromJson(json);
        }

        /// <summary>
        /// Serialize Index object.
        /// </summary>
        public static string IndexToJson(Index index)
        {
            return index.Value;
        }

        /// <summary>
        /// Deserialize Index object.
        /// </summary>
        public static Index IndexFromJson(string json)
        {
            return new Index(json);
        }

        /// <summary>
        /// Serialize ApplicationEventArgument.
        /// </summary>
        public static string ApplicationEventArgumentToJson(ApplicationEventArgument e)
        {
            string gridNameJson = GridNameToJson(e.GridName);
            string indexJson = IndexToJson(e.Index);
            string columnNameJson = e.ColumnName;
            //
            UtilFramework.Assert(!gridNameJson.Contains("-"));
            UtilFramework.Assert(!indexJson.Contains("-"));
            UtilFramework.Assert(!columnNameJson.Contains("-"));
            //
            string result = string.Format("{0}-{1}-{2}", gridNameJson, indexJson, columnNameJson);
            return result;
        }

        /// <summary>
        /// Deserialize ApplicationEventArgument.
        /// </summary>
        public static ApplicationEventArgument ApplicationEventArgumentFromJson(App app, string json)
        {
            string gridNameJson = json.Split("-")[0];
            string indexJson = json.Split("-")[1];
            string columnNameJson = json.Split("-")[2];
            //
            GridName gridName = GridNameFromJson(gridNameJson);
            Index index = IndexFromJson(indexJson);
            string columnName = columnNameJson;
            //
            ApplicationEventArgument result = new ApplicationEventArgument(app, gridName, index, columnName);
            return result;
        }
    }
}
