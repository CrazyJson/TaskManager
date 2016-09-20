---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[Configuration_Options]'))
BEGIN
/*==============================================================*/
/* Table: p_Task                                              */
/*==============================================================*/
CREATE TABLE [dbo].[Configuration_Options](
	[OptionId] nvarchar(50) NOT NULL,
	[OptionType] nvarchar(50)  NOT NULL ,
	[OptionName] nvarchar(100) NOT NULL  ,
	[Key] nvarchar(50) NOT NULL  ,
	[Value] nvarchar(2000)   ,
	[ValueType] nvarchar(50) NOT NULL   ,
	PRIMARY KEY(OptionId)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '系统参数配置选项表','user', @CurrentUser, 'table', 'Configuration_Options'
execute sp_addextendedproperty 'MS_Description',  '选项ID' ,'user', @CurrentUser, 'table', 'Configuration_Options', 'column', 'OptionId'
execute sp_addextendedproperty 'MS_Description',  '选项类型' ,'user', @CurrentUser, 'table', 'Configuration_Options', 'column', 'OptionType'
execute sp_addextendedproperty 'MS_Description',  '选项名称' ,'user', @CurrentUser, 'table', 'Configuration_Options', 'column', 'OptionName'
execute sp_addextendedproperty 'MS_Description',  '参数Key值' ,'user', @CurrentUser, 'table', 'Configuration_Options', 'column', 'Key'
execute sp_addextendedproperty 'MS_Description',  '选项值' ,'user', @CurrentUser, 'table', 'Configuration_Options', 'column', 'Value'
execute sp_addextendedproperty 'MS_Description',  '值类型(1:数字 2:字符串 3:布尔型 true or false 4:密码) 5:文本域' ,'user', @CurrentUser, 'table', 'Configuration_Options', 'column', 'ValueType'

END
GO