// Do not modify this file. It's generated by Framework.Cli.

namespace Database.dbo
{
    using Framework.Dal;
    using System;

    [SqlTable("dbo", "FrameworkConfigField")]
    public class FrameworkConfigField : Row
    {
        [SqlField("Id", typeof(FrameworkConfigField_Id), true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("ConfigGridId", typeof(FrameworkConfigField_ConfigGridId), FrameworkTypeEnum.Int)]
        public int ConfigGridId { get; set; }

        [SqlField("FieldId", typeof(FrameworkConfigField_FieldId), FrameworkTypeEnum.Int)]
        public int FieldId { get; set; }

        [SqlField("Text", typeof(FrameworkConfigField_Text), FrameworkTypeEnum.Nvarcahr)]
        public string Text { get; set; }

        [SqlField("Description", typeof(FrameworkConfigField_Description), FrameworkTypeEnum.Nvarcahr)]
        public string Description { get; set; }

        [SqlField("IsVisible", typeof(FrameworkConfigField_IsVisible), FrameworkTypeEnum.Bit)]
        public bool IsVisible { get; set; }

        [SqlField("IsReadOnly", typeof(FrameworkConfigField_IsReadOnly), FrameworkTypeEnum.Bit)]
        public bool IsReadOnly { get; set; }

        [SqlField("Sort", typeof(FrameworkConfigField_Sort), FrameworkTypeEnum.Float)]
        public double? Sort { get; set; }
    }

    public class FrameworkConfigField_Id : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_ConfigGridId : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_FieldId : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_Text : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_Description : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_IsVisible : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_IsReadOnly : Cell<FrameworkConfigField> { }

    public class FrameworkConfigField_Sort : Cell<FrameworkConfigField> { }

    [SqlTable("dbo", "FrameworkConfigFieldBuiltIn")]
    public class FrameworkConfigFieldBuiltIn : Row
    {
        [SqlField("Id", typeof(FrameworkConfigFieldBuiltIn_Id), FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("ConfigGridId", typeof(FrameworkConfigFieldBuiltIn_ConfigGridId), FrameworkTypeEnum.Int)]
        public int ConfigGridId { get; set; }

        [SqlField("ConfigGridIdName", typeof(FrameworkConfigFieldBuiltIn_ConfigGridIdName), FrameworkTypeEnum.Nvarcahr)]
        public string ConfigGridIdName { get; set; }

        [SqlField("FieldId", typeof(FrameworkConfigFieldBuiltIn_FieldId), FrameworkTypeEnum.Int)]
        public int FieldId { get; set; }

        [SqlField("FieldIdName", typeof(FrameworkConfigFieldBuiltIn_FieldIdName), FrameworkTypeEnum.Nvarcahr)]
        public string FieldIdName { get; set; }

        [SqlField("TableNameCSharp", typeof(FrameworkConfigFieldBuiltIn_TableNameCSharp), FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("ConfigName", typeof(FrameworkConfigFieldBuiltIn_ConfigName), FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("FieldNameCSharp", typeof(FrameworkConfigFieldBuiltIn_FieldNameCSharp), FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameCSharp { get; set; }

        [SqlField("Text", typeof(FrameworkConfigFieldBuiltIn_Text), FrameworkTypeEnum.Nvarcahr)]
        public string Text { get; set; }

        [SqlField("Description", typeof(FrameworkConfigFieldBuiltIn_Description), FrameworkTypeEnum.Nvarcahr)]
        public string Description { get; set; }

        [SqlField("IsVisible", typeof(FrameworkConfigFieldBuiltIn_IsVisible), FrameworkTypeEnum.Bit)]
        public bool IsVisible { get; set; }

        [SqlField("IsReadOnly", typeof(FrameworkConfigFieldBuiltIn_IsReadOnly), FrameworkTypeEnum.Bit)]
        public bool IsReadOnly { get; set; }

        [SqlField("Sort", typeof(FrameworkConfigFieldBuiltIn_Sort), FrameworkTypeEnum.Float)]
        public double? Sort { get; set; }
    }

    public class FrameworkConfigFieldBuiltIn_Id : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_ConfigGridId : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_ConfigGridIdName : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_FieldId : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_FieldIdName : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_TableNameCSharp : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_ConfigName : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_FieldNameCSharp : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_Text : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_Description : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_IsVisible : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_IsReadOnly : Cell<FrameworkConfigFieldBuiltIn> { }

    public class FrameworkConfigFieldBuiltIn_Sort : Cell<FrameworkConfigFieldBuiltIn> { }

    [SqlTable("dbo", "FrameworkConfigGrid")]
    public class FrameworkConfigGrid : Row
    {
        [SqlField("Id", typeof(FrameworkConfigGrid_Id), true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("TableId", typeof(FrameworkConfigGrid_TableId), FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("ConfigName", typeof(FrameworkConfigGrid_ConfigName), FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("RowCountMax", typeof(FrameworkConfigGrid_RowCountMax), FrameworkTypeEnum.Int)]
        public int? RowCountMax { get; set; }

        [SqlField("IsAllowInsert", typeof(FrameworkConfigGrid_IsAllowInsert), FrameworkTypeEnum.Bit)]
        public bool? IsAllowInsert { get; set; }

        [SqlField("IsExist", typeof(FrameworkConfigGrid_IsExist), FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    public class FrameworkConfigGrid_Id : Cell<FrameworkConfigGrid> { }

    public class FrameworkConfigGrid_TableId : Cell<FrameworkConfigGrid> { }

    public class FrameworkConfigGrid_ConfigName : Cell<FrameworkConfigGrid> { }

    public class FrameworkConfigGrid_RowCountMax : Cell<FrameworkConfigGrid> { }

    public class FrameworkConfigGrid_IsAllowInsert : Cell<FrameworkConfigGrid> { }

    public class FrameworkConfigGrid_IsExist : Cell<FrameworkConfigGrid> { }

    [SqlTable("dbo", "FrameworkConfigGridBuiltIn")]
    public class FrameworkConfigGridBuiltIn : Row
    {
        [SqlField("Id", typeof(FrameworkConfigGridBuiltIn_Id), FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("IdName", typeof(FrameworkConfigGridBuiltIn_IdName), FrameworkTypeEnum.Nvarcahr)]
        public string IdName { get; set; }

        [SqlField("TableId", typeof(FrameworkConfigGridBuiltIn_TableId), FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("TableIdName", typeof(FrameworkConfigGridBuiltIn_TableIdName), FrameworkTypeEnum.Nvarcahr)]
        public string TableIdName { get; set; }

        [SqlField("TableNameCSharp", typeof(FrameworkConfigGridBuiltIn_TableNameCSharp), FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("ConfigName", typeof(FrameworkConfigGridBuiltIn_ConfigName), FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("RowCountMax", typeof(FrameworkConfigGridBuiltIn_RowCountMax), FrameworkTypeEnum.Int)]
        public int? RowCountMax { get; set; }

        [SqlField("IsAllowInsert", typeof(FrameworkConfigGridBuiltIn_IsAllowInsert), FrameworkTypeEnum.Bit)]
        public bool? IsAllowInsert { get; set; }

        [SqlField("IsExist", typeof(FrameworkConfigGridBuiltIn_IsExist), FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    public class FrameworkConfigGridBuiltIn_Id : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_IdName : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_TableId : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_TableIdName : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_TableNameCSharp : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_ConfigName : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_RowCountMax : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_IsAllowInsert : Cell<FrameworkConfigGridBuiltIn> { }

    public class FrameworkConfigGridBuiltIn_IsExist : Cell<FrameworkConfigGridBuiltIn> { }

    [SqlTable("dbo", "FrameworkField")]
    public class FrameworkField : Row
    {
        [SqlField("Id", typeof(FrameworkField_Id), true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("TableId", typeof(FrameworkField_TableId), FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("FieldNameCSharp", typeof(FrameworkField_FieldNameCSharp), FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameCSharp { get; set; }

        [SqlField("FieldNameSql", typeof(FrameworkField_FieldNameSql), FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameSql { get; set; }

        [SqlField("IsExist", typeof(FrameworkField_IsExist), FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    public class FrameworkField_Id : Cell<FrameworkField> { }

    public class FrameworkField_TableId : Cell<FrameworkField> { }

    public class FrameworkField_FieldNameCSharp : Cell<FrameworkField> { }

    public class FrameworkField_FieldNameSql : Cell<FrameworkField> { }

    public class FrameworkField_IsExist : Cell<FrameworkField> { }

    [SqlTable("dbo", "FrameworkFieldBuiltIn")]
    public class FrameworkFieldBuiltIn : Row
    {
        [SqlField("Id", typeof(FrameworkFieldBuiltIn_Id), FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("IdName", typeof(FrameworkFieldBuiltIn_IdName), FrameworkTypeEnum.Nvarcahr)]
        public string IdName { get; set; }

        [SqlField("TableId", typeof(FrameworkFieldBuiltIn_TableId), FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("TableIdName", typeof(FrameworkFieldBuiltIn_TableIdName), FrameworkTypeEnum.Nvarcahr)]
        public string TableIdName { get; set; }

        [SqlField("FieldNameCSharp", typeof(FrameworkFieldBuiltIn_FieldNameCSharp), FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameCSharp { get; set; }

        [SqlField("FieldNameSql", typeof(FrameworkFieldBuiltIn_FieldNameSql), FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameSql { get; set; }

        [SqlField("IsExist", typeof(FrameworkFieldBuiltIn_IsExist), FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    public class FrameworkFieldBuiltIn_Id : Cell<FrameworkFieldBuiltIn> { }

    public class FrameworkFieldBuiltIn_IdName : Cell<FrameworkFieldBuiltIn> { }

    public class FrameworkFieldBuiltIn_TableId : Cell<FrameworkFieldBuiltIn> { }

    public class FrameworkFieldBuiltIn_TableIdName : Cell<FrameworkFieldBuiltIn> { }

    public class FrameworkFieldBuiltIn_FieldNameCSharp : Cell<FrameworkFieldBuiltIn> { }

    public class FrameworkFieldBuiltIn_FieldNameSql : Cell<FrameworkFieldBuiltIn> { }

    public class FrameworkFieldBuiltIn_IsExist : Cell<FrameworkFieldBuiltIn> { }

    [SqlTable("dbo", "FrameworkScript")]
    public class FrameworkScript : Row
    {
        [SqlField("Id", typeof(FrameworkScript_Id), true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("FileName", typeof(FrameworkScript_FileName), FrameworkTypeEnum.Nvarcahr)]
        public string FileName { get; set; }

        [SqlField("Date", typeof(FrameworkScript_Date), FrameworkTypeEnum.Datetime2)]
        public DateTime? Date { get; set; }
    }

    public class FrameworkScript_Id : Cell<FrameworkScript> { }

    public class FrameworkScript_FileName : Cell<FrameworkScript> { }

    public class FrameworkScript_Date : Cell<FrameworkScript> { }

    [SqlTable("dbo", "FrameworkTable")]
    public class FrameworkTable : Row
    {
        [SqlField("Id", typeof(FrameworkTable_Id), true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("TableNameCSharp", typeof(FrameworkTable_TableNameCSharp), FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("TableNameSql", typeof(FrameworkTable_TableNameSql), FrameworkTypeEnum.Nvarcahr)]
        public string TableNameSql { get; set; }

        [SqlField("IsExist", typeof(FrameworkTable_IsExist), FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    public class FrameworkTable_Id : Cell<FrameworkTable> { }

    public class FrameworkTable_TableNameCSharp : Cell<FrameworkTable> { }

    public class FrameworkTable_TableNameSql : Cell<FrameworkTable> { }

    public class FrameworkTable_IsExist : Cell<FrameworkTable> { }

    [SqlTable("dbo", "FrameworkTableBuiltIn")]
    public class FrameworkTableBuiltIn : Row
    {
        [SqlField("Id", typeof(FrameworkTableBuiltIn_Id), FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("IdName", typeof(FrameworkTableBuiltIn_IdName), FrameworkTypeEnum.Nvarcahr)]
        public string IdName { get; set; }
    }

    public class FrameworkTableBuiltIn_Id : Cell<FrameworkTableBuiltIn> { }

    public class FrameworkTableBuiltIn_IdName : Cell<FrameworkTableBuiltIn> { }
}
