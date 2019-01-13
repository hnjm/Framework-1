﻿namespace Framework.DataAccessLayer
{
    using Framework.Config;
    using Framework.DataAccessLayer.DatabaseMemory;
    using Framework.Json;
    using Framework.Session;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using static Framework.DataAccessLayer.UtilDalType;
    using static Framework.Session.UtilSession;

    /// <summary>
    /// Linq to database or linq to memory.
    /// </summary>
    public enum DatabaseEnum
    {
        None = 0,

        /// <summary>
        /// Linq to database.
        /// </summary>
        Database = 1,

        /// <summary>
        /// Linq to memory shared by multiple requests (singleton scope).
        /// </summary>
        MemorySingleton = 2,

        /// <summary>
        /// Linq to memory (request scope).
        /// </summary>
        MemoryRequest = 3
    }

    /// <summary>
    /// Data access layer functions.
    /// </summary>
    public static class Data // public static class UtilDal
    {
        /// <summary>
        /// Returns memory where rows are stored.
        /// </summary>
        public static IList MemoryRowList(Type typeRow, DatabaseEnum databaseEnum = DatabaseEnum.MemorySingleton)
        {
            switch (databaseEnum)
            {
                case DatabaseEnum.MemorySingleton:
                    return DatabaseMemory.DatabaseMemoryInternal.Instance.RowListGet(typeRow);
                default:
                    throw new Exception("DatabaseEnum not supported!");
            }
        }

        /// <summary>
        /// Returns linq to memory query.
        /// </summary>
        public static List<TRow> MemoryRowList<TRow>(DatabaseEnum databaseEnum = DatabaseEnum.MemorySingleton) where TRow : Row
        {
            switch (databaseEnum)
            {
                case DatabaseEnum.MemorySingleton:
                    return (List<TRow>)MemoryRowList(typeof(TRow));
                default:
                    throw new Exception("Scope not supported!");
            }
        }

        /// <summary>
        /// DbContext caches Model by default by DbContext type. Framework needs
        /// a Model by DbContextInternal.TypeRow.
        /// </summary>
        internal class ModelCacheKeyFactory : IModelCacheKeyFactory
        {
            public object Create(DbContext context)
            {
                return ((DbContextInternal)context).TypeRow;
            }
        }

        /// <summary>
        /// DbContext for one TypeRow.
        /// </summary>
        internal class DbContextInternal : DbContext
        {
            public DbContextInternal(string connectionString, Type typeRow)
            {
                this.ConnectionString = connectionString;
                this.TypeRow = typeRow;
            }

            public readonly string ConnectionString;

            public readonly Type TypeRow;

            /// <summary>
            /// Gets Query for TypeRow.
            /// </summary>
            public IQueryable Query
            {
                get
                {
                    return (IQueryable)(this.GetType().GetTypeInfo().GetMethod("Set").MakeGenericMethod(TypeRow).Invoke(this, null));
                }
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseSqlServer(ConnectionString);
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                optionsBuilder.ReplaceService<IModelCacheKeyFactory, ModelCacheKeyFactory>();
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Entity model
                var entityBuilder = modelBuilder.Entity(TypeRow);
                SqlTableAttribute tableAttribute = (SqlTableAttribute)TypeRow.GetTypeInfo().GetCustomAttribute(typeof(SqlTableAttribute));
                entityBuilder.ToTable(tableAttribute.TableNameSql, tableAttribute.SchemaNameSql); // By default EF maps sql table name to class name.

                // Field model
                var fieldList = UtilDalType.TypeRowToFieldList(TypeRow);
                bool isPrimaryKey = false; // Sql view 
                foreach (var field in fieldList)
                {
                    if (field.FieldNameSql == null) // Calculated column. Do not include it in sql select.
                    {
                        entityBuilder.Ignore(field.PropertyInfo.Name);
                    }
                    else
                    {
                        if (field.IsPrimaryKey)
                        {
                            isPrimaryKey = true;
                            entityBuilder.HasKey(field.PropertyInfo.Name); // Prevent null exception if primary key name is not "Id".
                        }
                        var propertyBuilder = entityBuilder.Property(field.PropertyInfo.PropertyType, field.PropertyInfo.Name);
                        propertyBuilder.HasColumnName(field.FieldNameSql);
                        if (UtilDalType.FrameworkTypeFromEnum(field.FrameworkTypeEnum).SqlTypeName == "datetime")
                        {
                            // Prevent "Conversion failed when converting date and/or time from character string." exception for 
                            // sql field type "datetime" for dynamic linq where function. See also method QueryFilter();
                            propertyBuilder.HasColumnType("datetime");
                        }
                    }
                }

                if (isPrimaryKey == false)
                {
                    string propertyName = fieldList.Where(item => item.FieldNameSql != null).First().PropertyInfo.Name;
                    entityBuilder.HasKey(propertyName); // Prevent null exception if name of first field (of view) is not "Id". See also QueryTrackingBehavior.NoTracking;
                }
            }
        }

        /// <summary>
        /// Returns DbContext with ConnectionString and model for one row, defined in typeRow.
        /// </summary>
        internal static DbContextInternal DbContextInternalCreate(Type typeRow)
        {
            string connectionString = ConfigWebServer.ConnectionString(typeRow);
            if (connectionString == null)
            {
                throw new Exception("ConnectionString is null! (See also file: ConfigWebServer.json)");
            }

            if (UtilDalType.TypeRowIsTableNameSql(typeRow) == false)
            {
                throw new Exception("TypeRow does not have TableNameSql definition!");
            }

            return new DbContextInternal(connectionString, typeRow);
        }

        private static string ExecuteParamAddPrivate(FrameworkTypeEnum frameworkTypeEnum, string paramName, object value, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList)
        {
            FrameworkType frameworkType = UtilDalType.FrameworkTypeFromEnum(frameworkTypeEnum);

            // ParamName
            if (paramName == null)
            {
                paramName = $"@P{ paramList.Count }";
            }
            UtilFramework.Assert(paramName.StartsWith("@"), "Parameter does not start with @!");
            UtilFramework.Assert(paramList.Where(item => item.SqlParameter.ParameterName == paramName).Count() == 0, string.Format("ParamName already exists! ({0})", paramName));

            // Value
            if (value != null)
            {
                UtilFramework.Assert(value.GetType() == frameworkType.ValueType);
            }
            if (value is string && (string)value == "")
            {
                value = null;
            }
            if (value == null)
            {
                value = DBNull.Value;
            }

            SqlParameter parameter = new SqlParameter(paramName, frameworkType.DbType) { Value = value };
            paramList.Add((frameworkTypeEnum, parameter));

            return paramName;
        }

        /// <summary>
        /// Adds sql param.
        /// </summary>
        internal static void ExecuteParamAdd(FrameworkTypeEnum frameworkTypeEnum, string paramName, object value, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList)
        {
            ExecuteParamAddPrivate(frameworkTypeEnum, paramName, value, paramList);
        }

        /// <summary>
        /// Adds sql param and returns new paramName. For example "@P0".
        /// </summary>
        internal static string ExecuteParamAdd(FrameworkTypeEnum frameworkTypeEnum, object value, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList)
        {
            return ExecuteParamAddPrivate(frameworkTypeEnum, null, value, paramList);
        }

        /// <summary>
        /// Replaces sql params with text params and returns full sql for debugging.
        /// </summary>
        internal static string ExecuteParamDebug(string sql, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList)
        {
            paramList = paramList.OrderByDescending(item => item.SqlParameter.ParameterName).ToList(); // Replace first @P100, then @P10
            foreach (var param in paramList)
            {
                FrameworkType frameworkType = UtilDalType.FrameworkTypeFromEnum(param.FrameworkTypeEnum);
                string find = param.SqlParameter.ParameterName;
                string replace = frameworkType.ValueToSqlParameterDebug(param.SqlParameter.Value);
                sql = UtilFramework.Replace(sql, find, replace);
            }
            return sql;
        }

        /// <summary>
        /// Execute sql statement.
        /// </summary>
        /// <param name="sql">Sql can have "GO" batch seperator.</param>
        internal static async Task ExecuteNonQueryAsync(string sql, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList, bool isFrameworkDb, int? commandTimeout = null)
        {
            var sqlList = sql.Split(new string[] { "\r\nGO", "\nGO", "GO\r\n", "GO\n" }, StringSplitOptions.RemoveEmptyEntries);

            string connectionString = ConfigWebServer.ConnectionString(isFrameworkDb);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                foreach (string sqlItem in sqlList)
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlItem, sqlConnection);
                    if (commandTimeout.HasValue)
                    {
                        sqlCommand.CommandTimeout = commandTimeout.Value;
                    }
                    if (paramList?.Count > 0)
                    {
                        sqlCommand.Parameters.AddRange(paramList.Select(item => item.SqlParameter).ToArray());
                    }
                    await sqlCommand.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Returns multiple result sets from stored procedure or select.
        /// </summary>
        /// <param name="sql">For example: "SELECT 1 AS A SELECT 2 AS B"; or with parameter: "SELECT @P0 AS A";</param>
        /// <param name="paramList">See also method ExecuteParamAdd();</param>
        /// <returns>Returns (ResultSet, Row, ColumnName, Value) value list.</returns>
        internal static async Task<List<List<Dictionary<string, object>>>> ExecuteReaderMultipleAsync(string sql, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList = null, bool isFrameworkDb = false)
        {
            List<List<Dictionary<string, object>>> result = new List<List<Dictionary<string, object>>>();
            string connectionString = ConfigWebServer.ConnectionString(isFrameworkDb);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                if (paramList?.Count > 0)
                {
                    sqlCommand.Parameters.AddRange(paramList.Select(item => item.SqlParameter).ToArray());
                }
                using (SqlDataReader sqlDataReader = await sqlCommand.ExecuteReaderAsync())
                {
                    while (sqlDataReader.HasRows)
                    {
                        var rowList = new List<Dictionary<string, object>>();
                        result.Add(rowList);
                        while (sqlDataReader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            rowList.Add(row);
                            for (int i = 0; i < sqlDataReader.FieldCount; i++)
                            {
                                string columnName = sqlDataReader.GetName(i);
                                object value = sqlDataReader.GetValue(i);
                                if (value == DBNull.Value)
                                {
                                    value = null;
                                }
                                row.Add(columnName, value);
                            }
                        }
                        sqlDataReader.NextResult();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Execute sql statement.
        /// </summary>
        internal static async Task<List<Row>> ExecuteReaderAsync(Type typeRow, string sql, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList = null, bool isFrameworkDb = false)
        {
            var valueList = await ExecuteReaderMultipleAsync(sql, paramList, isFrameworkDb);
            return ExecuteReaderMultipleResultCopy(typeRow, valueList, 0);
        }

        /// <summary>
        /// Execute sql statement.
        /// </summary>
        internal static async Task<List<TRow>> ExecuteReaderAsync<TRow>(string sql, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList, bool isFrameworkDb = false) where TRow : Row
        {
            var valueList = await ExecuteReaderMultipleAsync(sql, paramList, isFrameworkDb);
            return ExecuteReaderMultipleResultCopy<TRow>(valueList, 0);
        }

        /// <summary>
        /// Convert one sql multiple result set to typed row list.
        /// </summary>
        /// <param name="valueList">Result of method ExecuteReaderAsync();</param>
        /// <param name="resultSetIndex">Sql multiple result set index.</param>
        /// <param name="typeRow">Type of row to copy to. Copy one multiple result set.</param>
        /// <returns>List of typed rows.</returns>
        internal static List<Row> ExecuteReaderMultipleResultCopy(Type typeRow, List<List<Dictionary<string, object>>> valueList, int resultSetIndex)
        {
            List<Row> result = new List<Row>();
            PropertyInfo[] propertyInfoList = UtilDalType.TypeRowToPropertyInfoList(typeRow);
            foreach (Dictionary<string, object> row in valueList[resultSetIndex])
            {
                Row rowResult = Data.RowCreate(typeRow);
                foreach (string columnName in row.Keys)
                {
                    object value = row[columnName];
                    PropertyInfo propertyInfo = propertyInfoList.Where(item => item.Name == columnName).FirstOrDefault();
                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(rowResult, value);
                    }
                }
                result.Add(rowResult);
            }
            return result;
        }

        /// <summary>
        /// Convert sql multiple result set to one typed row list.
        /// </summary>
        /// <typeparam name="T">Type of row to copy to. Copy one multiple result set.</typeparam>
        /// <param name="valueList">Result of method Execute(); multiple result set.</param>
        /// <param name="resultSetIndex">Sql multiple result set index.</param>
        /// <returns>Type of row to copy to. Copy one multiple result set.</returns>
        internal static List<T> ExecuteReaderMultipleResultCopy<T>(List<List<Dictionary<string, object>>> valueList, int resultSetIndex) where T : Row
        {
            List<Row> result = ExecuteReaderMultipleResultCopy(typeof(T), valueList, resultSetIndex);
            return result.Cast<T>().ToList();
        }

        /// <summary>
        /// Returns linq to database query.
        /// </summary>
        public static IQueryable Query(Type typeRow, DatabaseEnum databaseEnum = DatabaseEnum.Database)
        {
            switch (databaseEnum)
            {
                case DatabaseEnum.Database:
                    return DbContextInternalCreate(typeRow).Query;
                case DatabaseEnum.MemorySingleton:
                    return DatabaseMemoryInternal.Instance.RowListGet(typeRow).AsQueryable();
                default:
                    throw new Exception("Scope not supported!");
            }
        }

        /// <summary>
        /// Returns linq to database query.
        /// </summary>
        public static IQueryable<TRow> Query<TRow>(DatabaseEnum databaseEnum = DatabaseEnum.Database) where TRow : Row
        {
            return (IQueryable<TRow>)Query(typeof(TRow), databaseEnum);
        }

        /// <summary>
        /// Returns empty query to clear data grid and keep column definition.
        /// </summary>
        public static IQueryable<TRow> QueryEmpty<TRow>() where TRow: Row
        {
            return Enumerable.Empty<TRow>().AsQueryable();
        }

        /// <summary>
        /// Copy data row. Source and dest need not to be of same type. Only cells available on
        /// both records are copied.
        /// </summary>
        public static void RowCopy(Row rowSource, Row rowDest, string fieldNameSourcePrefix = null)
        {
            var propertyInfoDestList = UtilDalType.TypeRowToPropertyInfoList(rowDest.GetType());
            foreach (PropertyInfo propertyInfoDest in propertyInfoDestList)
            {
                string fieldNameDest = propertyInfoDest.Name;
                string fieldNameSource = fieldNameSourcePrefix + fieldNameDest;
                PropertyInfo propertyInfoSource = rowSource.GetType().GetTypeInfo().GetProperty(fieldNameSource);
                if (propertyInfoSource != null)
                {
                    object value = propertyInfoSource.GetValue(rowSource);
                    propertyInfoDest.SetValue(rowDest, value);
                }
            }
        }

        /// <summary>
        /// Clone data row.
        /// </summary>
        public static Row RowCopy(Row row)
        {
            Row result = (Row)UtilFramework.TypeToObject(row.GetType());
            RowCopy(row, result);
            return result;
        }

        /// <summary>
        /// Clone data row.
        /// </summary>
        public static TRow RowCopy<TRow>(TRow row) where TRow : Row
        {
            return (TRow)RowCopy((Row)row);
        }

        /// <summary>
        /// Returns true, if rows are identical.
        /// </summary>
        internal static bool RowEqual(Row rowA, Row rowB)
        {
            UtilFramework.Assert(rowA.GetType() == rowB.GetType());

            bool result = true;
            var propertyInfoList = UtilDalType.TypeRowToPropertyInfoList(rowA.GetType());
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                object valueA = propertyInfo.GetValue(rowA);
                object valueB = propertyInfo.GetValue(rowB);
                if (object.Equals(valueA, valueB) == false)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns new data row.
        /// </summary>
        public static Row RowCreate(Type typeRow)
        {
            return (Row)UtilFramework.TypeToObject(typeRow);
        }

        /// <summary>
        /// Select data from database.
        /// </summary>
        public static async Task<List<Row>> SelectAsync(IQueryable query)
        {
            UtilFramework.LogDebug(string.Format("SELECT ({0})", query.ElementType.Name));

            var list = await query.ToDynamicListAsync();
            List<Row> result = list.Cast<Row>().ToList();
            return result;
        }

        public static async Task<List<TRow>> SelectAsync<TRow>(IQueryable<TRow> query) where TRow : Row
        {
            var list = await SelectAsync((IQueryable)query);
            List<TRow> result = list.Cast<TRow>().ToList();
            return result;
        }

        internal static IQueryable QueryFilter(IQueryable query, string fieldName, object filterValue, FilterOperator filterOperator)
        {
            string predicate = fieldName;
            switch (filterOperator)
            {
                case FilterOperator.Equal:
                    predicate += " = @0";
                    break;
                case FilterOperator.Smaller:
                    predicate += " <= @0";
                    break;
                case FilterOperator.Greater:
                    predicate += " >= @0";
                    break;
                case FilterOperator.Like:
                    predicate += ".Contains(@0)";
                    break;
                default:
                    throw new Exception("Enum unknown!");
            }

            if (filterValue is DateTime)
            {
                // In order to prevent "Conversion failed when converting date and/or time from character string." exception for sql type "datetime"
                // dynamic linq where parameter needs to be passed as string and not as DateTime.
                //
                // SQL datetime
                // 2008-09-04 00:00:00.000
                //
                // SQL datetime2
                // 1979-02-03T00:00:00.0000000

                filterValue = ((DateTime)filterValue).ToString("yyyy-MM-dd HH:mmm:ss.fff");
            }

            return query.Where(predicate, filterValue);
        }

        /// <summary>
        /// Sql orderby.
        /// </summary>
        internal static IQueryable QueryOrderBy(IQueryable query, string fieldName, bool isSort)
        {
            if (isSort == true)
            {
                fieldName += " DESC";
            }
            return query.OrderBy(fieldName);
        }

        /// <summary>
        /// Sql paging.
        /// </summary>
        internal static IQueryable QuerySkipTake(IQueryable query, int skip, int take)
        {
            if (skip != 0)
            {
                query = query.Skip(skip);
            }
            query = query.Take(take);
            return query;
        }

        /// <summary>
        /// Delete data record from database.
        /// </summary>
        public static async Task Delete(Row row)
        {
            DbContext dbContext = DbContextInternalCreate(row.GetType());
            dbContext.Remove(row);
            int count = await dbContext.SaveChangesAsync();
            UtilFramework.Assert(count == 1, "Update failed!");
        }

        /// <summary>
        /// Insert data record. Primary key needs to be 0! Returned new row contains new primary key.
        /// </summary>
        public static async Task<TRow> InsertAsync<TRow>(TRow row, DatabaseEnum databaseEnum = DatabaseEnum.Database) where TRow : Row
        {
            UtilFramework.LogDebug(string.Format("INSERT ({0})", row.GetType().Name));

            switch (databaseEnum)
            {
                case DatabaseEnum.Database:
                    {
                        Row rowCopy = Data.RowCopy(row);
                        DbContext dbContext = DbContextInternalCreate(row.GetType());
                        dbContext.Add(row); // Throws NullReferenceException if no primary key is defined. // EF sets auto increment field to 2147482647.
                        try
                        {
                            int count = await dbContext.SaveChangesAsync();
                            UtilFramework.Assert(count == 1, "Update failed!");
                            //
                            // Exception: Database operation expected to affect 1 row(s) but actually affected 0 row(s). 
                            // Cause: No autoincrement on Id column or no Id set by application
                            //
                            // Exception: The conversion of a datetime2 data type to a datetime data type resulted in an out-of-range value.
                            // Cause: CSharp not nullable DateTime default value is "{1/1/0001 12:00:00 AM}" change it to nullable or set value for example to DateTime.Now
                        }
                        catch (Exception exception)
                        {
                            Data.RowCopy(rowCopy, row); // In case of exception, auto increment id stays -2147482647. Reverse it back.
                            throw exception;
                        }
                        break;
                    }
                case DatabaseEnum.MemorySingleton:
                    {
                        var rowList = Data.MemoryRowList(row.GetType(), databaseEnum);
                        rowList.Add(row);
                        break;
                    }
                default:
                    throw new Exception("Scope not supported!");
            }
            return row; // Return Row with new primary key.
        }

        /// <summary>
        /// Update data record on database.
        /// </summary>
        public static async Task UpdateAsync(Row row, Row rowNew, DatabaseEnum databaseEnum = DatabaseEnum.Database)
        {
            UtilFramework.LogDebug(string.Format("UPDATE ({0})", row.GetType().Name));

            UtilFramework.Assert(row.GetType() == rowNew.GetType());
            // if (Data.RowEqual(row, rowNew) == false) // See also: EntityState.Modified
            {
                switch (databaseEnum)
                {
                    case DatabaseEnum.Database:
                        {
                            row = Data.RowCopy(row); // Prevent modifications on SetValues(rowNew);
                            DbContext dbContext = Data.DbContextInternalCreate(row.GetType());
                            var tracking = dbContext.Attach(row);
                            tracking.CurrentValues.SetValues(rowNew);
                            tracking.State = EntityState.Modified; // Update also if row and rowNew are equal.
                            int count = await dbContext.SaveChangesAsync();
                            UtilFramework.Assert(count == 1, "Update failed!");
                            break;
                        }
                    case DatabaseEnum.MemorySingleton:
                        {
                            var rowList = Data.MemoryRowList(row.GetType(), databaseEnum);
                            PropertyInfo propertyInfo = UtilDalType.TypeRowToPropertyInfoList(row.GetType()).First(); // Assume first field is primary key.
                            object idNew = propertyInfo.GetValue(row);
                            int updateCount = 0;
                            foreach (Row rowMemory in rowList.Cast<Row>())
                            {
                                object id = propertyInfo.GetValue(rowMemory);
                                if (object.Equals(id, idNew))
                                {
                                    Data.RowCopy(rowNew, rowMemory);
                                    updateCount += 1;
                                }
                            }
                            if (updateCount == 0)
                            {
                                throw new Exception("Memory row could not be updated!");
                            }
                            if (updateCount > 1)
                            {
                                throw new Exception("More than one memory row updated!");
                            }
                            break;
                        }
                    default:
                        throw new Exception("Scope not supported!");
                }
            }
        }

        /// <summary>
        /// Convert database value to text.
        /// </summary>
        internal static void CellTextFromValue(Page page, Grid grid, GridRowSession gridRowSession, Field field, GridCellSession gridCellSession, Row row)
        {
            if (gridRowSession.Row != null)
            {
                string text = null;
                object value = field.PropertyInfo.GetValue(row);
                if (value != null)
                {
                    text = page.GridCellText(grid, row, field.PropertyInfo.Name); // Custom convert database value to cell text.
                    text = UtilFramework.StringNull(text);
                    if (text == null)
                    {
                        text = field.FrameworkType().CellTextFromValue(value);
                    }
                }
                gridCellSession.Text = text;
            }
        }

        /// <summary>
        /// Parse user entered cell and filter text. Text can be null.
        /// </summary>
        private static object CellTextParse(Field field, string text)
        {
            object result = field.FrameworkType().CellTextParse(text);
            return result;
        }

        /// <summary>
        /// Parse user entered cell text and write it to row. Text can be null.
        /// </summary>
        internal static void CellTextParse(Field field, string text, Row row, out string errorParse)
        {
            errorParse = null;
            object value = CellTextParse(field, text);
            bool isPrevent = false;
            bool isNullable = UtilFramework.IsNullable(field.PropertyInfo.PropertyType); // Do not write value to row if type is not nullable but text is null.
            isPrevent = (text == null) && !isNullable;
            if (!isPrevent)
            {
                field.PropertyInfo.SetValue(row, value);
            }
        }

        /// <summary>
        /// Default parse user entered filter text. Text can be null.
        /// </summary>
        internal static void CellTextParseFilter(Field field, string text, Filter filter, out string errorParse)
        {
            errorParse = null;
            object filterValue = CellTextParse(field, text);
            FilterOperator filterOperator = FilterOperator.Equal;
            if (field.PropertyInfo.PropertyType == typeof(string))
            {
                filterOperator = FilterOperator.Like;
            }
            filter.SetValue(field.PropertyInfo.Name, filterValue, filterOperator);
        }

        /// <summary>
        /// Returns true, if no delete key has been pressed and cell has no error.
        /// </summary>
        internal static bool CellTextParseIsAutocomplete(GridCellItem gridCellItem)
        {
            bool isDelete = UtilFramework.StringEmpty(gridCellItem.GridCellSession.TextOld).Length > UtilFramework.StringEmpty(gridCellItem.GridCellSession.Text).Length;
            bool isError = gridCellItem.GridCellSession.ErrorParse != null;

            return !isDelete && !isError;
        }
    }

    public enum FilterOperator
    {
        None = 0,
        Equal = 1,
        Smaller = 2,
        Greater = 3,
        Like = 4
    }

    internal class UtilDalUpsert
    {
        internal static string UpsertFieldNameToCsvList(string[] fieldNameList, string prefix)
        {
            string result = null;
            bool isFirst = true;
            foreach (string fieldName in fieldNameList)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    result += ", ";
                }
                result += prefix + fieldName;
            }
            return result;
        }

        internal static string UpsertFieldNameToAssignList(string[] fieldNameList, string prefixTarget, string prefixSource)
        {
            string result = null;
            bool isFirst = true;
            foreach (string fieldName in fieldNameList)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    result += ", ";
                }
                result += prefixTarget + fieldName + " = " + prefixSource + fieldName;
            }
            return result;
        }

        private static string UpsertSelect(Type typeRow, List<Row> rowList, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList)
        {
            StringBuilder sqlSelect = new StringBuilder();
            var fieldList = UtilDalType.TypeRowToFieldList(typeRow);

            // Row
            bool isFirstRow = true;
            foreach (Row row in rowList)
            {
                UtilFramework.Assert(row.GetType() == typeRow);
                if (isFirstRow)
                {
                    isFirstRow = false;
                }
                else
                {
                    sqlSelect.Append(" UNION ALL\r\n");
                }

                // Field
                sqlSelect.Append("(SELECT ");
                bool isFirstField = true;
                foreach (var field in fieldList)
                {
                    if (isFirstField)
                    {
                        isFirstField = false;
                    }
                    else
                    {
                        sqlSelect.Append(", ");
                    }
                    object value = field.PropertyInfo.GetValue(row);
                    string paramName = Data.ExecuteParamAdd(field.FrameworkTypeEnum, value, paramList);
                    sqlSelect.Append(string.Format("{0} AS {1}", paramName, field.FieldNameSql));
                }
                sqlSelect.Append(")");
            }
            return sqlSelect.ToString();
        }

        internal static async Task UpsertAsync(Type typeRow, List<Row> rowList, string[] fieldNameKeyList)
        {
            string tableName = UtilDalType.TypeRowToTableNameSql(typeRow);
            bool isFrameworkDb = UtilDalType.TypeRowIsFrameworkDb(typeRow);
            var fieldNameSqlList = UtilDalType.TypeRowToFieldList(typeRow).Where(item => item.IsPrimaryKey == false).Select(item => item.FieldNameSql).ToArray();

            string fieldNameKeySourceList = UpsertFieldNameToCsvList(fieldNameKeyList, "Source.");
            string fieldNameKeyTargetList = UpsertFieldNameToCsvList(fieldNameKeyList, "Target.");
            string fieldNameAssignList = UpsertFieldNameToAssignList(fieldNameSqlList, "Target.", "Source.");
            string fieldNameInsertList = UpsertFieldNameToCsvList(fieldNameSqlList, null);
            string fieldNameValueList = UpsertFieldNameToCsvList(fieldNameSqlList, "Source.");

            var paramList = new List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)>();
            string sqlSelect = UpsertSelect(typeRow, rowList, paramList);
            // string sqlDebug = Data.ExecuteParamDebug(sqlSelect, paramList);
            // var resultDebug = await Data.ExecuteReaderAsync(typeRow, sqlDebug);

            string sqlUpsert = @"
            MERGE INTO {0} AS Target
            USING ({1}) AS Source
	        ON NOT EXISTS(
                SELECT ({2})
                EXCEPT
                SELECT ({3}))
            WHEN MATCHED THEN
	            UPDATE SET 
                    {4}
            WHEN NOT MATCHED BY TARGET THEN
	            INSERT ({5})
	            VALUES ({6});
            ";
            sqlUpsert = string.Format(sqlUpsert, tableName, sqlSelect, fieldNameKeySourceList, fieldNameKeyTargetList, fieldNameAssignList, fieldNameInsertList, fieldNameValueList);
            // string sqlDebug = Data.ExecuteParamDebug(sqlUpsert, paramList);

            // Upsert
            await Data.ExecuteNonQueryAsync(sqlUpsert, paramList, isFrameworkDb);
        }

        internal static async Task UpsertAsync<TRow>(List<TRow> rowList, string[] fieldNameKeyList) where TRow : Row
        {
            await UpsertAsync(typeof(TRow), rowList.Cast<Row>().ToList(), fieldNameKeyList);
        }

        internal static async Task UpsertAsync<TRow>(List<TRow> rowList, string fieldNameKey) where TRow : Row
        {
            await UpsertAsync(rowList, new string[] { fieldNameKey });
        }

        /// <summary>
        /// Set IsExist flag to false on sql table.
        /// </summary>
        internal static async Task UpsertIsExistAsync(Type typeRow, string fieldNameIsExist = "IsExist")
        {
            string tableNameSql = UtilDalType.TypeRowToTableNameSql(typeRow);
            bool isFrameworkDb = UtilDalType.TypeRowIsFrameworkDb(typeRow);
            // IsExists
            string sqlIsExist = string.Format("UPDATE {0} SET {1}=CAST(0 AS BIT)", tableNameSql, fieldNameIsExist);
            await Data.ExecuteNonQueryAsync(sqlIsExist, null, isFrameworkDb);
        }

        internal static async Task UpsertIsExistAsync<TRow>(string fieldNameIsExist = "IsExist") where TRow : Row
        {
            await UpsertIsExistAsync(typeof(TRow));
        }
    }

    internal class UtilDalUpsertBuiltIn
    {
        public class FieldBuiltIn
        {
            /// <summary>
            /// Gets or sets Field. See also method UtilDalType.TypeRowToFieldList();
            /// </summary>
            public Field Field;

            /// <summary>
            /// Gets or sets IsKey. True, if "Id" or "IdName".
            /// </summary>
            public bool IsKey;

            /// <summary>
            /// Gets or sets IsIdName. True, if for example "TableIdName".
            /// </summary>
            public bool IsIdName;

            /// <summary>
            /// Gets or sets "IsId". True, if for example "TableId".
            /// </summary>
            public bool IsId;

            /// <summary>
            /// Gets or sets FieldNameIdSql. For example "TableId".
            /// </summary>
            public string FieldNameIdSql;

            /// <summary>
            /// Gets or sets TypeRowReference. Referenced table (or view) containing field "Id" and "Name". For example view "FrameworkTableBuiltIn".
            /// </summary>
            public Type TypeRowReference;
        }

        private static List<FieldBuiltIn> FieldBuiltInList(Type typeRow, string fieldNameSqlPrefix, List<Assembly> assemblyList)
        {
            List<FieldBuiltIn> result = new List<FieldBuiltIn>();
            var fieldList = UtilDalType.TypeRowToFieldList(typeRow);
            var fieldNameSqlList = fieldList.Select(item => item.FieldNameSql).ToList();
            var tableNameSqlList = UtilDalType.TableNameSqlList(assemblyList);

            // Populate result
            foreach (var field in fieldList)
            {
                FieldBuiltIn fieldBuiltIn = new FieldBuiltIn();
                fieldBuiltIn.Field = field;
                result.Add(fieldBuiltIn);
            }

            foreach (var fieldBuiltIn in result)
            {
                string fieldNameSql = fieldBuiltIn.Field.FieldNameSql;

                fieldBuiltIn.IsKey = fieldNameSql == "Id" || fieldNameSql == "IdName";

                string lastChar = ""; // Character before "IdName".
                if (fieldNameSql.Length > "IdName".Length)
                {
                    lastChar = fieldNameSql.Substring(fieldNameSql.Length - "IdName".Length - 1, 1);
                }
                bool lastCharIsLower = lastChar == lastChar.ToLower() && lastChar.Length == 1;
                if (fieldNameSql.EndsWith("IdName") && lastCharIsLower) // BuiltIn naming convention.
                {
                    string fieldNameIdSql = fieldNameSql.Substring(0, fieldNameSql.Length - "Name".Length); // BuiltIn naming convention.
                    if (fieldNameSqlList.Contains(fieldNameIdSql))
                    {
                        UtilDalType.TypeRowToTableNameSql(typeRow, out string schemaNameSql, out string tableNameSql);
                        string tableNameSqlBuiltIn = fieldNameSqlPrefix + fieldNameSql.Substring(0, fieldNameSql.Length - "IdName".Length) + "BuiltIn"; // Reference table
                        tableNameSqlBuiltIn = UtilDalType.TableNameSql(schemaNameSql, tableNameSqlBuiltIn);
                        var tableReference = tableNameSqlList.Where(item => item.Value == tableNameSqlBuiltIn).SingleOrDefault();
                        if (tableReference.Value != null)
                        {
                            List<string> propertyNameList = UtilDalType.TypeRowToPropertyInfoList(tableReference.Key).Select(item => item.Name).ToList();
                            if (propertyNameList.Contains("Id") && propertyNameList.Contains("IdName")) // BuiltIn naming convention.
                            {
                                fieldBuiltIn.IsIdName = true;
                                fieldBuiltIn.TypeRowReference = tableReference.Key;
                                fieldBuiltIn.FieldNameIdSql = fieldNameIdSql;

                                var fieldBuiltInId = result.Where(item => item.Field.FieldNameSql == fieldNameIdSql).Single();
                                fieldBuiltInId.IsId = true;
                                fieldBuiltInId.TypeRowReference = tableReference.Key;
                                fieldBuiltInId.FieldNameIdSql = fieldNameIdSql;
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static string UpsertSelect(Type typeRow, List<Row> rowList, string fieldNameSqlPrefix, List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)> paramList, List<Assembly> assemblyList)
        {
            StringBuilder sqlSelect = new StringBuilder();
            var fieldBuiltInList = FieldBuiltInList(typeRow, fieldNameSqlPrefix, assemblyList);
            var tableNameSqlList = UtilDalType.TableNameSqlList(assemblyList);

            // Row
            bool isFirstRow = true;
            foreach (Row row in rowList)
            {
                UtilFramework.Assert(row.GetType() == typeRow);
                if (isFirstRow)
                {
                    isFirstRow = false;
                }
                else
                {
                    sqlSelect.Append(" UNION ALL\r\n");
                }

                // Field
                sqlSelect.Append("(SELECT ");
                bool isFirstField = true;
                foreach (var fieldBuiltIn in fieldBuiltInList)
                {
                    bool isField = (fieldBuiltIn.IsId == false && fieldBuiltIn.IsIdName == false && fieldBuiltIn.IsKey == false) || fieldBuiltIn.IsIdName;
                    if (isField)
                    {
                        if (isFirstField)
                        {
                            isFirstField = false;
                        }
                        else
                        {
                            sqlSelect.Append(", ");
                        }
                        string fieldNameSql = fieldBuiltIn.Field.FieldNameSql;
                        object value = fieldBuiltIn.Field.PropertyInfo.GetValue(row);
                        string paramName = Data.ExecuteParamAdd(fieldBuiltIn.Field.FrameworkTypeEnum, value, paramList);
                        if (fieldBuiltIn.IsId == false && fieldBuiltIn.IsIdName == false)
                        {
                            sqlSelect.Append(string.Format("{0} AS {1}", paramName, fieldBuiltIn.Field.FieldNameSql));
                        }
                        else
                        {
                            if (fieldBuiltIn.IsIdName)
                            {
                                string tableNameSql = UtilDalType.TypeRowToTableNameCSharp(fieldBuiltIn.TypeRowReference);
                                string sqlBuiltIn = string.Format("(SELECT BuiltIn.Id FROM {0} BuiltIn WHERE BuiltIn.IdName = {1}) AS {2}", tableNameSql, paramName, fieldBuiltIn.FieldNameIdSql);
                                sqlSelect.Append(sqlBuiltIn);
                            }
                        }
                    }
                }
                sqlSelect.Append(")");
            }
            return sqlSelect.ToString();
        }

        /// <summary>
        /// Sql merge into for BuiltIn.
        /// </summary>
        /// <param name="typeRow">Type of rowList (can be empty).</param>
        /// <param name="rowList">Records to update.</param>
        /// <param name="fieldNameKeyList">Key fields for record identification.</param>
        /// <param name="fieldNameSqlPrefix">For example "Framework".</param>
        /// <param name="assemblyList">Assemblies in which to search reference tables.</param>
        internal static async Task UpsertAsync(Type typeRow, List<Row> rowList, string[] fieldNameKeyList, string fieldNameSqlPrefix, List<Assembly> assemblyList)
        {
            bool isFrameworkDb = UtilDalType.TypeRowIsFrameworkDb(typeRow);

            foreach (var rowListSplit in UtilFramework.Split(rowList, 100)) // Prevent error: "The server supports a maximum of 2100 parameters"
            {
                var paramList = new List<(FrameworkTypeEnum FrameworkTypeEnum, SqlParameter SqlParameter)>();
                string sqlSelect = UpsertSelect(typeRow, rowListSplit, fieldNameSqlPrefix, paramList, assemblyList);
                // string sqlDebug = Data.ExecuteParamDebug(sqlSelect, paramList); sqlSelect = sqlDebug;

                // Update underlying sql table if sql view ends with "BuiltIn".
                UtilDalType.TypeRowToTableNameSql(typeRow, out string schemaNameSql, out string tableNameSql);
                if (tableNameSql.EndsWith("BuiltIn"))
                {
                    tableNameSql = tableNameSql.Substring(0, tableNameSql.Length - "BuiltIn".Length);
                }
                tableNameSql = UtilDalType.TableNameSql(schemaNameSql, tableNameSql);

                var fieldNameSqlList = FieldBuiltInList(typeRow, fieldNameSqlPrefix, assemblyList).Where(item => item.IsIdName == false && item.Field.IsPrimaryKey == false && item.IsKey == false).Select(item => item.Field.FieldNameSql).ToArray();

                string fieldNameKeySourceList = UtilDalUpsert.UpsertFieldNameToCsvList(fieldNameKeyList, "Source.");
                string fieldNameKeyTargetList = UtilDalUpsert.UpsertFieldNameToCsvList(fieldNameKeyList, "Target.");
                string fieldNameAssignList = UtilDalUpsert.UpsertFieldNameToAssignList(fieldNameSqlList, "Target.", "Source.");
                string fieldNameInsertList = UtilDalUpsert.UpsertFieldNameToCsvList(fieldNameSqlList, null);
                string fieldNameValueList = UtilDalUpsert.UpsertFieldNameToCsvList(fieldNameSqlList, "Source.");

                string sqlUpsert = @"
                MERGE INTO {0} AS Target
                USING ({1}) AS Source
	            ON NOT EXISTS(
                    SELECT {2}
                    EXCEPT
                    SELECT {3})
                WHEN MATCHED THEN
	                UPDATE SET 
                        {4}
                WHEN NOT MATCHED BY TARGET THEN
	                INSERT ({5})
	                VALUES ({6});
                ";
                sqlUpsert = string.Format(sqlUpsert, tableNameSql, sqlSelect, fieldNameKeySourceList, fieldNameKeyTargetList, fieldNameAssignList, fieldNameInsertList, fieldNameValueList);
                // string sqlDebug = Data.ExecuteParamDebug(sqlUpsert, paramList);

                // Upsert
                await Data.ExecuteNonQueryAsync(sqlUpsert, paramList, isFrameworkDb);
            }
        }

        internal static async Task UpsertAsync<TRow>(List<TRow> rowList, string[] fieldNameKeyList, string fieldNameSqlPrefix, List<Assembly> assemblyList) where TRow : Row
        {
            await UpsertAsync(typeof(TRow), rowList.Cast<Row>().ToList(), fieldNameKeyList, fieldNameSqlPrefix, assemblyList);
        }

        internal static async Task UpsertAsync<TRow>(List<TRow> rowList, string fieldNameKey, List<Assembly> assemblyList, string fieldNameSqlPrefix) where TRow : Row
        {
            await UpsertAsync(rowList, new string[] { fieldNameKey }, fieldNameSqlPrefix, assemblyList);
        }
    }

    /// <summary>
    /// Framework type system.
    /// </summary>
    public enum FrameworkTypeEnum
    {
        None = 0,
        Int = 1,
        Smallint = 2,
        Tinyint = 3,
        Bigint = 4,
        Uniqueidentifier = 5,
        Datetime = 6,
        Datetime2 = 7,
        Date = 8,
        Char = 9,
        NChar = 23,
        Nvarcahr = 10,
        Varchar = 11,
        Text = 12,
        Ntext = 13,
        Bit = 14,
        Money = 15,
        Smallmoney = 24,
        Decimal = 16,
        Real = 17,
        Float = 18,
        Varbinary = 19,
        Sqlvariant = 20,
        Image = 21,
        Numeric = 22, // 24
    }

    internal static class UtilDalType
    {
        /// <summary>
        /// Returns true if typeRow is declared if Framework assembly.
        /// </summary>
        internal static bool TypeRowIsFrameworkDb(Type typeRow)
        {
            return typeRow.GetTypeInfo().Assembly == typeof(Data).Assembly; // Type is declared in Framework assembly.
        }

        /// <summary>
        /// Returns true if typeRow has database table.
        /// </summary>
        internal static bool TypeRowIsTableNameSql(Type typeRow)
        {
            bool result = false;
            SqlTableAttribute tableAttribute = (SqlTableAttribute)typeRow.GetTypeInfo().GetCustomAttribute(typeof(SqlTableAttribute));
            if (tableAttribute != null && (tableAttribute.SchemaNameSql != null || tableAttribute.TableNameSql != null))
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Returns rows defined in "Database" namespace in assemblies.
        /// </summary>
        /// <param name="assemblyList">Use method AppCli.AssemblyList(); when running in cli mode or method UtilServer.AssemblyList(); when running in web mode.</param>
        internal static List<Type> TypeRowList(List<Assembly> assemblyList)
        {
            Dictionary<string, Type> result = new Dictionary<string, Type>();
            foreach (Assembly assembly in assemblyList)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(Row)))
                    {
                        if (UtilFramework.TypeToName(type).StartsWith("Database."))
                        {
                            string tableNameCSharp = UtilDalType.TypeRowToTableNameCSharp(type);
                            if (result.ContainsKey(tableNameCSharp))
                            {
                                throw new Exception(string.Format("TableNameCSharp exists already in different assembly! ({0})", tableNameCSharp));

                            }
                            result.Add(tableNameCSharp, type);
                        }
                        else
                        {
                            throw new Exception(string.Format("Row class not defined in Database namespace! ({0})", UtilFramework.TypeToName(type)));
                        }
                    }
                }
            }
            return result.Values.ToList();
        }

        /// <summary>
        /// Returns row type as string. For example: "dbo.User". Omits "Database" namespace prefix.
        /// </summary>
        internal static string TypeRowToTableNameCSharp(Type typeRow)
        {
            string result = null;
            UtilFramework.Assert(UtilFramework.IsSubclassOf(typeRow, typeof(Row)), "Wrong type!");
            result = UtilFramework.TypeToName(typeRow);
            UtilFramework.Assert(result.StartsWith("Database.")); // If it is a calculated row which does not exist on database move it for example to namespace "Database.Calculated".
            result = result.Substring("Database.".Length); // Remove "Database" namespace.
            return result;
        }

        /// <summary>
        /// Returns (TypeRow, TableNameSql) list.
        /// </summary>
        internal static Dictionary<Type, string> TableNameSqlList(List<Assembly> assemblyList)
        {
            Dictionary<Type, string> result = new Dictionary<Type, string>();
            List<Type> typeRowList = TypeRowList(assemblyList);
            foreach (Type typeRow in typeRowList)
            {
                string tableNameSql = TypeRowToTableNameSql(typeRow);
                result.Add(typeRow, tableNameSql);
            }
            return result;
        }

        internal static void TypeRowToTableNameSql(Type typeRow, out string schemaNameSql, out string tableNameSql)
        {
            SqlTableAttribute tableAttribute = (SqlTableAttribute)typeRow.GetTypeInfo().GetCustomAttribute(typeof(SqlTableAttribute));
            schemaNameSql = tableAttribute.SchemaNameSql;
            tableNameSql = tableAttribute.TableNameSql;
        }

        /// <summary>
        /// Returns sql table name with schema name.
        /// </summary>
        internal static string TableNameSql(string schemaNameSql, string tableNameSql)
        {
            string result = string.Format("[{0}].[{1}]", schemaNameSql, tableNameSql);
            return result;
        }

        /// <summary>
        /// See also method TypeRowIsTableNameSql();
        /// </summary>
        internal static string TypeRowToTableNameSql(Type typeRow)
        {
            TypeRowToTableNameSql(typeRow, out string schemaNameSql, out string tableNameSql);
            string result = TableNameSql(schemaNameSql, tableNameSql);
            return result;
        }

        /// <summary>
        /// See also method TypeRowToFieldList();
        /// </summary>
        internal static PropertyInfo[] TypeRowToPropertyInfoList(Type typeRow)
        {
            return typeRow.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        internal class Field
        {
            public Field(PropertyInfo propertyInfo, string fieldNameSql, bool isPrimaryKey, FrameworkTypeEnum frameworkTypeEnum)
            {
                this.PropertyInfo = propertyInfo;
                this.FieldNameSql = fieldNameSql;
                this.IsPrimaryKey = isPrimaryKey;
                this.FrameworkTypeEnum = frameworkTypeEnum;
            }

            public readonly PropertyInfo PropertyInfo;

            public readonly string FieldNameSql;

            public readonly bool IsPrimaryKey;

            public readonly FrameworkTypeEnum FrameworkTypeEnum;

            public FrameworkType FrameworkType()
            {
                if (FrameworkTypeEnum == FrameworkTypeEnum.None)
                {
                    return UtilDalType.FrameworkTypeFromValueType(PropertyInfo.PropertyType);
                }
                return UtilDalType.FrameworkTypeFromEnum(FrameworkTypeEnum);
            }
        }

        /// <summary>
        /// See also method FrameworkTypeEnumToFrameworkType();
        /// </summary>
        internal static List<Field> TypeRowToFieldList(Type typeRow)
        {
            var result = new List<Field>();
            var propertyInfoList = TypeRowToPropertyInfoList(typeRow);
            foreach (PropertyInfo propertyInfo in propertyInfoList)
            {
                SqlFieldAttribute fieldAttribute = (SqlFieldAttribute)propertyInfo.GetCustomAttribute(typeof(SqlFieldAttribute));
                string fieldNameSql = null;
                FrameworkTypeEnum frameworkTypeEnum = FrameworkTypeEnum.None;
                bool isPrimaryKey = false;
                if (fieldAttribute != null)
                {
                    fieldNameSql = fieldAttribute.FieldNameSql;
                    frameworkTypeEnum = fieldAttribute.FrameworkTypeEnum;
                    isPrimaryKey = fieldAttribute.IsPrimaryKey;
                }
                result.Add(new Field(propertyInfo, fieldNameSql, isPrimaryKey, frameworkTypeEnum));
            }
            return result;
        }

        public static Type SqlTypeToType(int sqlType)
        {
            Type type = FrameworkTypeList().Where(item => item.Value.SqlType == sqlType).Single().Value.ValueType;
            return type;
        }

        public static FrameworkTypeEnum SqlTypeToFrameworkTypeEnum(int sqlType)
        {
            var result = FrameworkTypeList().Where(item => item.Value.SqlType == sqlType).SingleOrDefault().Value?.FrameworkTypeEnum;
            if (result == null)
            {
                return FrameworkTypeEnum.None;
            }
            else
            {
                return (FrameworkTypeEnum)result;
            }
        }

        public static FrameworkType FrameworkTypeFromEnum(FrameworkTypeEnum frameworkTypeEnum)
        {
            UtilFramework.Assert(frameworkTypeEnum != FrameworkTypeEnum.None, "FrameworkTypeEnum not defined!");
            return FrameworkTypeList().Where(item => item.Value.FrameworkTypeEnum == frameworkTypeEnum).Single().Value;
        }

        public static FrameworkType FrameworkTypeFromValueType(Type valueType)
        {
            valueType = UtilFramework.TypeUnderlying(valueType); // int? to int.
            if (valueType == typeof(int))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Int);
            }
            if (valueType == typeof(Guid))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Uniqueidentifier);
            }
            if (valueType == typeof(DateTime))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Datetime);
            }
            if (valueType == typeof(char))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Char);
            }
            if (valueType == typeof(string))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Nvarcahr);
            }
            if (valueType == typeof(bool))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Bit);
            }
            if (valueType == typeof(decimal))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Decimal);
            }
            if (valueType == typeof(float))
            {
                return FrameworkTypeFromEnum(FrameworkTypeEnum.Float);
            }
            throw new Exception("Type not found!");
        }

        [ThreadStatic]
        private static Dictionary<FrameworkTypeEnum, FrameworkType> frameworkTypeList;

        private static Dictionary<FrameworkTypeEnum, FrameworkType> FrameworkTypeList()
        {
            // See also: https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
            // See also: SELECT * FROM sys.types
            // See also: https://docs.microsoft.com/en-us/sql/t-sql/data-types/data-type-conversion-database-engine

            if (frameworkTypeList == null)
            {
                Dictionary<FrameworkTypeEnum, FrameworkType> result = new Dictionary<FrameworkTypeEnum, FrameworkType>();
                FrameworkType frameworkType;
                frameworkType = new FrameworkTypeInt(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeSmallint(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeTinyint(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeBigint(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeUniqueidentifier(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeDatetime(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeDatetime2(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeDate(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeChar(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeNChar(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeNvarcahr(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeVarchar(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeText(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeNtext(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeBit(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeMoney(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeSmallmoney(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeDecimal(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeReal(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeFloat(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeVarbinary(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new SqlTypeSqlvariant(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeImage(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkType = new FrameworkTypeNumeric(); result.Add(frameworkType.FrameworkTypeEnum, frameworkType);
                frameworkTypeList = result;
            }
            return frameworkTypeList;
        }
    }

    internal class FrameworkType
    {
        public FrameworkType(FrameworkTypeEnum frameworkTypeEnum, string sqlTypeName, int sqlType, Type valueType, DbType dbType, bool isNumber)
        {
            this.FrameworkTypeEnum = frameworkTypeEnum;
            this.SqlTypeName = sqlTypeName;
            this.SqlType = sqlType;
            this.ValueType = valueType;
            this.DbType = dbType;
            this.IsNumber = isNumber;
        }

        public readonly FrameworkTypeEnum FrameworkTypeEnum;

        /// <summary>
        /// Gets or sets SqlTypeName. For example: "int", "datetime", "datetime".
        /// </summary>
        public readonly string SqlTypeName;

        public readonly int SqlType;

        public readonly Type ValueType;

        public readonly DbType DbType;

        public readonly bool IsNumber;

        /// <summary>
        /// Convert database value to front end cell text.
        /// </summary>
        /// <param name="value">Value is never null when this method is called.</param>
        /// <returns>Returns text to display in cell.</returns>
        protected virtual internal string CellTextFromValue(object value)
        {
            string result = value.ToString();
            return result;
        }

        /// <summary>
        /// Parse user entered text to database value. Text can be null.
        /// </summary>
        protected virtual internal object CellTextParse(string text)
        {
            object result = null;
            if (text != null)
            {
                Type type = UtilFramework.TypeUnderlying(ValueType);
                result = Convert.ChangeType(text, type);
            }
            return result;
        }

        protected virtual internal string ValueToSqlParameterDebug(object value)
        {
            string result = null;
            if (value != null)
            {
                result = value.ToString();
            }
            if (IsNumber == false)
            {
                result = "'" + result + "'";
            }
            if (value == null)
            {
                result = "NULL";
            }
            return result;
        }
    }

    internal class FrameworkTypeInt : FrameworkType
    {
        public FrameworkTypeInt()
            : base(FrameworkTypeEnum.Int, "int", 56, typeof(Int32), DbType.Int32, true)
        {

        }
    }

    internal class FrameworkTypeSmallint : FrameworkType
    {
        public FrameworkTypeSmallint()
            : base(FrameworkTypeEnum.Smallint, "smallint", 52, typeof(Int16), DbType.Int16, true)
        {

        }
    }

    internal class FrameworkTypeTinyint : FrameworkType
    {
        public FrameworkTypeTinyint()
            : base(FrameworkTypeEnum.Tinyint, "tinyint", 48, typeof(byte), DbType.Byte, true)
        {

        }
    }

    internal class FrameworkTypeBigint : FrameworkType
    {
        public FrameworkTypeBigint()
            : base(FrameworkTypeEnum.Bigint, "bigint", 127, typeof(Int64), DbType.Int64, true)
        {

        }
    }

    internal class FrameworkTypeUniqueidentifier : FrameworkType
    {
        public FrameworkTypeUniqueidentifier()
            : base(FrameworkTypeEnum.Uniqueidentifier, "uniqueidentifier", 36, typeof(Guid), DbType.Guid, false)
        {

        }
    }

    internal class FrameworkTypeDatetime : FrameworkType
    {
        public FrameworkTypeDatetime()
            : base(FrameworkTypeEnum.Datetime, "datetime", 61, typeof(DateTime), DbType.DateTime, false)
        {

        }

        protected internal override string CellTextFromValue(object value)
        {
            string result = null;
            if (value != null)
            {
                result = ((DateTime)value).ToString("dd.MM.yyyy");
            }
            return result;
        }

        protected internal override object CellTextParse(string text)
        {
            object result = null;
            if (text != null)
            {
                result = DateTime.ParseExact(text, "d.M.yyyy", null); // Parse for example: "1.1.2000".
            }
            return result;
        }
    }

    internal class FrameworkTypeDatetime2 : FrameworkType
    {
        public FrameworkTypeDatetime2()
            : base(FrameworkTypeEnum.Datetime2, "datetime2", 42, typeof(DateTime), DbType.DateTime2, false)
        {

        }
    }

    internal class FrameworkTypeDate : FrameworkType
    {
        public FrameworkTypeDate()
            : base(FrameworkTypeEnum.Date, "date", 40, typeof(DateTime), DbType.Date, false)
        {

        }
    }

    internal class FrameworkTypeChar : FrameworkType
    {
        public FrameworkTypeChar()
            : base(FrameworkTypeEnum.Char, "char", 175, typeof(string), DbType.String, false)
        {

        }
    }

    internal class FrameworkTypeNChar : FrameworkType
    {
        public FrameworkTypeNChar()
            : base(FrameworkTypeEnum.NChar, "nchar", 239, typeof(string), DbType.StringFixedLength, false)
        {

        }
    }

    internal class FrameworkTypeNvarcahr : FrameworkType
    {
        public FrameworkTypeNvarcahr()
            : base(FrameworkTypeEnum.Nvarcahr, "nvarcahr", 231, typeof(string), DbType.String, false)
        {

        }
    }

    internal class FrameworkTypeVarchar : FrameworkType
    {
        public FrameworkTypeVarchar()
            : base(FrameworkTypeEnum.Varchar, "varchar", 167, typeof(string), DbType.String, false)
        {

        }
    }

    internal class FrameworkTypeText : FrameworkType // See also: https://stackoverflow.com/questions/564755/sql-server-text-type-vs-varchar-data-type
    {
        public FrameworkTypeText()
            : base(FrameworkTypeEnum.Text, "text", 35, typeof(string), DbType.String, false)
        {

        }
    }

    internal class FrameworkTypeNtext : FrameworkType
    {
        public FrameworkTypeNtext()
            : base(FrameworkTypeEnum.Ntext, "ntext", 99, typeof(string), DbType.String, false)
        {

        }
    }

    internal class FrameworkTypeBit : FrameworkType
    {
        public FrameworkTypeBit()
            : base(FrameworkTypeEnum.Bit, "bit", 104, typeof(bool), DbType.Boolean, false)
        {

        }

        protected internal override string ValueToSqlParameterDebug(object value)
        {
            string result = null;
            if (value != null)
            {
                UtilFramework.Assert(value.GetType() == ValueType);
                if ((bool)value == false)
                {
                    result = "CAST(0 AS BIT)";
                }
                else
                {
                    result = "CAST(1 AS BIT)";
                }
            }
            return result;
        }

        protected internal override object CellTextParse(string text)
        {
            if (text?.ToLower() == "false")
            {
                return false;
            }
            if (text?.ToLower() == "true")
            {
                return true;
            }
            return base.CellTextParse(text);
        }
    }

    internal class FrameworkTypeMoney : FrameworkType
    {
        public FrameworkTypeMoney()
            : base(FrameworkTypeEnum.Money, "money", 60, typeof(decimal), DbType.Decimal, true)
        {

        }
    }

    internal class FrameworkTypeSmallmoney : FrameworkType
    {
        public FrameworkTypeSmallmoney()
            : base(FrameworkTypeEnum.Smallmoney, "smallmoney", 122, typeof(decimal), DbType.Decimal, true)
        {

        }
    }

    internal class FrameworkTypeDecimal : FrameworkType
    {
        public FrameworkTypeDecimal()
            : base(FrameworkTypeEnum.Decimal, "decimal", 106, typeof(decimal), DbType.Decimal, true)
        {

        }
    }

    internal class FrameworkTypeReal : FrameworkType
    {
        public FrameworkTypeReal()
            : base(FrameworkTypeEnum.Real, "real", 59, typeof(Single), DbType.Single, true)
        {

        }
    }

    internal class FrameworkTypeFloat : FrameworkType
    {
        public FrameworkTypeFloat()
            : base(FrameworkTypeEnum.Float, "float", 62, typeof(double), DbType.Double, true)
        {

        }
    }

    internal class FrameworkTypeVarbinary : FrameworkType
    {
        public FrameworkTypeVarbinary()
            : base(FrameworkTypeEnum.Varbinary, "varbinary", 165, typeof(byte[]), DbType.Binary, false) // DbType.Binary?
        {

        }
    }

    internal class SqlTypeSqlvariant : FrameworkType
    {
        public SqlTypeSqlvariant()
            : base(FrameworkTypeEnum.Sqlvariant, "sql_variant", 98, typeof(object), DbType.Object, false)
        {

        }
    }

    internal class FrameworkTypeImage : FrameworkType
    {
        public FrameworkTypeImage()
            : base(FrameworkTypeEnum.Image, "image", 34, typeof(byte[]), DbType.Binary, false) // DbType.Binary?
        {

        }
    }

    internal class FrameworkTypeNumeric : FrameworkType
    {
        public FrameworkTypeNumeric()
            : base(FrameworkTypeEnum.Numeric, "numeric", 108, typeof(decimal), DbType.Decimal, true)
        {

        }
    }
}