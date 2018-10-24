﻿CREATE TABLE FrameworkTable /* Used for configuration. Contains all in source code defined tables. */
(
	Id INT PRIMARY KEY IDENTITY,
	TableNameCSharp NVARCHAR(256) NOT NULL UNIQUE, -- See also method UtilDataAccessLayer.TypeRowToNameCSharp();
	TableNameSql NVARCHAR(256), -- Can be null for memory rows.
	IsExist BIT NOT NULL
)

GO
CREATE VIEW FrameworkTableBuiltIn AS
SELECT
	Id,
	TableNameCSharp AS IdName
FROM
	FrameworkTable
GO

CREATE TABLE FrameworkField /* Used for configuration. Contains all in source code defined columns. Also calculated fields. */
(
	Id INT PRIMARY KEY IDENTITY,
	TableId INT FOREIGN KEY REFERENCES FrameworkTable(Id) NOT NULL ,
	FieldNameCSharp NVARCHAR(256) NOT NULL,
	FieldNameSql NVARCHAR(256), -- Can be null for calculated columns.
	IsExist BIT NOT NULL
	INDEX IX_FrameworkField UNIQUE (TableId, FieldNameCSharp)
)

GO
CREATE VIEW FrameworkFieldBuiltIn
AS
SELECT
	Field.Id,
	CONCAT((SELECT TableBuiltIn.IdName FROM FrameworkTableBuiltIn TableBuiltIn WHERE TableBuiltIn.Id = Field.TableId), '; ', Field.FieldNameCSharp) AS IdName,
	Field.TableId,
	(SELECT TableBuiltIn.IdName FROM FrameworkTableBuiltIn TableBuiltIn WHERE TableBuiltIn.Id = Field.TableId) AS TableIdName,
	Field.FieldNameCSharp,
	Field.FieldNameSql,
	Field.IsExist
FROM
	FrameworkField Field
GO

CREATE TABLE FrameworkConfigGrid
(
	Id INT PRIMARY KEY IDENTITY,
	TableId INT FOREIGN KEY REFERENCES FrameworkTable(Id) NOT NULL ,
	ConfigName NVARCHAR(256),
	RowCountMax INT,
	IsAllowInsert BIT,
	IsExist BIT NOT NULL
	INDEX IX_FrameworkConfigGrid UNIQUE (TableId, ConfigName)
)

GO
CREATE VIEW FrameworkConfigGridBuiltIn
AS
SELECT 
	ConfigGrid.Id,
	CONCAT((SELECT FrameworkTable.TableNameCSharp FROM FrameworkTable WHERE FrameworkTable.Id = ConfigGrid.TableId), '; ', ConfigGrid.ConfigName) AS IdName,
	ConfigGrid.TableId,
	(SELECT TableBuiltIn.IdName FROM FrameworkTableBuiltIn TableBuiltIn WHERE TableBuiltIn.Id = ConfigGrid.TableId) AS TableIdName,
	(SELECT FrameworkTable.TableNameCSharp FROM FrameworkTable FrameworkTable WHERE FrameworkTable.Id = ConfigGrid.TableId) AS TableNameCSharp,
	ConfigGrid.ConfigName,
	ConfigGrid.RowCountMax,
	ConfigGrid.IsAllowInsert,
	ConfigGrid.IsExist
FROM
	FrameworkConfigGrid ConfigGrid
GO

CREATE TABLE FrameworkConfigField
(
	Id INT PRIMARY KEY IDENTITY,
	ConfigGridId INT FOREIGN KEY REFERENCES FrameworkConfigGrid(Id) NOT NULL,
	FieldId INT FOREIGN KEY REFERENCES FrameworkField(Id) NOT NULL,
	Text NVARCHAR(256), -- Column header text.
	Description NVARCHAR(256), -- Column header description.
	IsVisible BIT NOT NULL,
	IsReadOnly BIT NOT NULL,
	Sort FLOAT,
	INDEX IX_FrameworkConfigField UNIQUE (ConfigGridId, FieldId)
)

GO
CREATE VIEW FrameworkConfigFieldBuiltIn
AS
SELECT 
	/* Id */
    ConfigField.Id,
	ConfigField.ConfigGridId,
	(SELECT ConfigGridBuiltIn.IdName FROM FrameworkConfigGridBuiltIn ConfigGridBuiltIn WHERE ConfigGridBuiltIn.Id = ConfigField.ConfigGridId) AS ConfigGridIdName,
	ConfigField.FieldId,
	(SELECT FieldBuiltIn.IdName FROM FrameworkFieldBuiltIn FieldBuiltIn WHERE FieldBuiltIn.Id = ConfigField.FieldId) AS FieldIdName,
	/* Extension */
	(SELECT FrameworkTable.TableNameCSharp FROM FrameworkConfigGrid Grid, FrameworkTable FrameworkTable WHERE Grid.Id = ConfigField.ConfigGridId AND FrameworkTable.Id = Grid.TableId) AS TableNameCSharp,
	(SELECT Grid.ConfigName FROM FrameworkConfigGrid Grid WHERE Grid.Id = ConfigField.ConfigGridId) AS ConfigName,
	(SELECT Field.FieldNameCSharp FROM FrameworkField Field WHERE Field.Id = ConfigField.FieldId) AS FieldNameCSharp,
	/* Data */
	ConfigField.Text,
	ConfigField.Description,
	ConfigField.IsVisible,
	ConfigField.IsReadOnly,
	ConfigField.Sort
FROM
	FrameworkConfigField ConfigField
GO
