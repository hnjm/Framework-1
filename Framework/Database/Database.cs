// Do not modify this file. It's generated by Framework.Cli.

namespace Database.dbo
{
    using Framework.DataAccessLayer;
    using System;

    [SqlTable("dbo", "FrameworkConfigField")]
    public class FrameworkConfigField : Row
    {
        [SqlField("Id", true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("ConfigGridId", FrameworkTypeEnum.Int)]
        public int ConfigGridId { get; set; }

        [SqlField("FieldId", FrameworkTypeEnum.Int)]
        public int FieldId { get; set; }

        [SqlField("InstanceName", FrameworkTypeEnum.Nvarcahr)]
        public string InstanceName { get; set; }

        [SqlField("Text", FrameworkTypeEnum.Nvarcahr)]
        public string Text { get; set; }

        [SqlField("Description", FrameworkTypeEnum.Nvarcahr)]
        public string Description { get; set; }

        [SqlField("IsVisible", FrameworkTypeEnum.Bit)]
        public bool? IsVisible { get; set; }

        [SqlField("IsReadOnly", FrameworkTypeEnum.Bit)]
        public bool? IsReadOnly { get; set; }

        [SqlField("Sort", FrameworkTypeEnum.Float)]
        public double? Sort { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkConfigFieldDisplay")]
    public class FrameworkConfigFieldDisplay : Row
    {
        [SqlField("ConfigGridId", FrameworkTypeEnum.Int)]
        public int? ConfigGridId { get; set; }

        [SqlField("ConfigGridTableId", FrameworkTypeEnum.Int)]
        public int ConfigGridTableId { get; set; }

        [SqlField("ConfigGridTableIdName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigGridTableIdName { get; set; }

        [SqlField("ConfigGridTableNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigGridTableNameCSharp { get; set; }

        [SqlField("ConfigGridConfigName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigGridConfigName { get; set; }

        [SqlField("ConfigGridIsExist", FrameworkTypeEnum.Bit)]
        public bool? ConfigGridIsExist { get; set; }

        [SqlField("FieldId", FrameworkTypeEnum.Int)]
        public int FieldId { get; set; }

        [SqlField("FieldTableId", FrameworkTypeEnum.Int)]
        public int FieldTableId { get; set; }

        [SqlField("FieldFieldNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string FieldFieldNameCSharp { get; set; }

        [SqlField("FieldFieldNameSql", FrameworkTypeEnum.Nvarcahr)]
        public string FieldFieldNameSql { get; set; }

        [SqlField("FieldFieldSort", FrameworkTypeEnum.Int)]
        public int FieldFieldSort { get; set; }

        [SqlField("FieldIsExist", FrameworkTypeEnum.Bit)]
        public bool FieldIsExist { get; set; }

        [SqlField("ConfigFieldId", FrameworkTypeEnum.Int)]
        public int? ConfigFieldId { get; set; }

        [SqlField("ConfigFieldConfigGridId", FrameworkTypeEnum.Int)]
        public int? ConfigFieldConfigGridId { get; set; }

        [SqlField("ConfigFieldFieldId", FrameworkTypeEnum.Int)]
        public int? ConfigFieldFieldId { get; set; }

        [SqlField("ConfigFieldInstanceName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigFieldInstanceName { get; set; }

        [SqlField("ConfigFieldText", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigFieldText { get; set; }

        [SqlField("ConfigFieldDescription", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigFieldDescription { get; set; }

        [SqlField("ConfigFieldIsVisible", FrameworkTypeEnum.Bit)]
        public bool? ConfigFieldIsVisible { get; set; }

        [SqlField("ConfigFieldIsReadOnly", FrameworkTypeEnum.Bit)]
        public bool? ConfigFieldIsReadOnly { get; set; }

        [SqlField("ConfigFieldSort", FrameworkTypeEnum.Float)]
        public double? ConfigFieldSort { get; set; }
    }

    [SqlTable("dbo", "FrameworkConfigFieldIntegrate")]
    public class FrameworkConfigFieldIntegrate : Row
    {
        [SqlField("Id", FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("ConfigGridId", FrameworkTypeEnum.Int)]
        public int ConfigGridId { get; set; }

        [SqlField("ConfigGridIdName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigGridIdName { get; set; }

        [SqlField("FieldId", FrameworkTypeEnum.Int)]
        public int FieldId { get; set; }

        [SqlField("FieldIdName", FrameworkTypeEnum.Nvarcahr)]
        public string FieldIdName { get; set; }

        [SqlField("InstanceName", FrameworkTypeEnum.Nvarcahr)]
        public string InstanceName { get; set; }

        [SqlField("TableNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("ConfigName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("FieldNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameCSharp { get; set; }

        [SqlField("Text", FrameworkTypeEnum.Nvarcahr)]
        public string Text { get; set; }

        [SqlField("Description", FrameworkTypeEnum.Nvarcahr)]
        public string Description { get; set; }

        [SqlField("IsVisible", FrameworkTypeEnum.Bit)]
        public bool? IsVisible { get; set; }

        [SqlField("IsReadOnly", FrameworkTypeEnum.Bit)]
        public bool? IsReadOnly { get; set; }

        [SqlField("Sort", FrameworkTypeEnum.Float)]
        public double? Sort { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkConfigGrid")]
    public class FrameworkConfigGrid : Row
    {
        [SqlField("Id", true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("TableId", FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("ConfigName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("RowCountMax", FrameworkTypeEnum.Int)]
        public int? RowCountMax { get; set; }

        [SqlField("IsAllowInsert", FrameworkTypeEnum.Bit)]
        public bool? IsAllowInsert { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkConfigGridDisplay")]
    public class FrameworkConfigGridDisplay : Row
    {
        [SqlField("Id", FrameworkTypeEnum.Int)]
        public int? Id { get; set; }

        [SqlField("TableId", FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("TableNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("TableNameSql", FrameworkTypeEnum.Nvarcahr)]
        public string TableNameSql { get; set; }

        [SqlField("ConfigName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("RowCountMax", FrameworkTypeEnum.Int)]
        public int? RowCountMax { get; set; }

        [SqlField("IsAllowInsert", FrameworkTypeEnum.Bit)]
        public bool? IsAllowInsert { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool? IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkConfigGridIntegrate")]
    public class FrameworkConfigGridIntegrate : Row
    {
        [SqlField("Id", FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("IdName", FrameworkTypeEnum.Nvarcahr)]
        public string IdName { get; set; }

        [SqlField("TableId", FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("TableIdName", FrameworkTypeEnum.Nvarcahr)]
        public string TableIdName { get; set; }

        [SqlField("TableNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("ConfigName", FrameworkTypeEnum.Nvarcahr)]
        public string ConfigName { get; set; }

        [SqlField("RowCountMax", FrameworkTypeEnum.Int)]
        public int? RowCountMax { get; set; }

        [SqlField("IsAllowInsert", FrameworkTypeEnum.Bit)]
        public bool? IsAllowInsert { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkDeployDb")]
    public class FrameworkDeployDb : Row
    {
        [SqlField("Id", true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("FileName", FrameworkTypeEnum.Nvarcahr)]
        public string FileName { get; set; }

        [SqlField("Date", FrameworkTypeEnum.Datetime2)]
        public DateTime? Date { get; set; }
    }

    [SqlTable("dbo", "FrameworkField")]
    public class FrameworkField : Row
    {
        [SqlField("Id", true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("TableId", FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("FieldNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameCSharp { get; set; }

        [SqlField("FieldNameSql", FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameSql { get; set; }

        [SqlField("Sort", FrameworkTypeEnum.Int)]
        public int Sort { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkFieldIntegrate")]
    public class FrameworkFieldIntegrate : Row
    {
        [SqlField("Id", FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("IdName", FrameworkTypeEnum.Nvarcahr)]
        public string IdName { get; set; }

        [SqlField("TableId", FrameworkTypeEnum.Int)]
        public int TableId { get; set; }

        [SqlField("TableIdName", FrameworkTypeEnum.Nvarcahr)]
        public string TableIdName { get; set; }

        [SqlField("FieldNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameCSharp { get; set; }

        [SqlField("FieldNameSql", FrameworkTypeEnum.Nvarcahr)]
        public string FieldNameSql { get; set; }

        [SqlField("Sort", FrameworkTypeEnum.Int)]
        public int Sort { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkTable")]
    public class FrameworkTable : Row
    {
        [SqlField("Id", true, FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("TableNameCSharp", FrameworkTypeEnum.Nvarcahr)]
        public string TableNameCSharp { get; set; }

        [SqlField("TableNameSql", FrameworkTypeEnum.Nvarcahr)]
        public string TableNameSql { get; set; }

        [SqlField("IsExist", FrameworkTypeEnum.Bit)]
        public bool IsExist { get; set; }
    }

    [SqlTable("dbo", "FrameworkTableIntegrate")]
    public class FrameworkTableIntegrate : Row
    {
        [SqlField("Id", FrameworkTypeEnum.Int)]
        public int Id { get; set; }

        [SqlField("IdName", FrameworkTypeEnum.Nvarcahr)]
        public string IdName { get; set; }
    }
}
