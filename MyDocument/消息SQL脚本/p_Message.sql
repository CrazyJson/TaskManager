---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_Message]'))
BEGIN
/*==============================================================*/
/* Table: p_Message                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_Message](
	[MessageGuid] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[Receiver] nvarchar(50)   ,
	[Content] nvarchar(MAX)   ,
	[Subject] nvarchar(200)   ,
	[Type] tinyint   DEFAULT 0 ,
	[CreatedOn] datetime   DEFAULT getdate() ,
	PRIMARY KEY(MessageGuid)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '消息表','user', @CurrentUser, 'table', 'p_Message'
execute sp_addextendedproperty 'MS_Description',  '消息GUID' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'MessageGuid'
execute sp_addextendedproperty 'MS_Description',  '消息接收人' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'Receiver'
execute sp_addextendedproperty 'MS_Description',  '信息内容' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'Content'
execute sp_addextendedproperty 'MS_Description',  '邮件主题' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'Subject'
execute sp_addextendedproperty 'MS_Description',  '消息类型    0：短信 1：邮件' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'Type'
execute sp_addextendedproperty 'MS_Description',  '消息创建日期' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'CreatedOn'

END
GO

GO
IF NOT EXISTS(SELECT 1 FROM syscolumns WHERE id=OBJECT_ID('[p_Message]') AND name='FromType')
BEGIN
	ALTER TABLE [dbo].[p_Message] ADD FromType NVARCHAR(200)   	
	declare @CurrentUser sysname
	select @CurrentUser = user_name()
	execute sp_addextendedproperty 'MS_Description',  '消息来源(eg:快递进度)' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'FromType'
END
GO

GO
IF NOT EXISTS(SELECT 1 FROM syscolumns WHERE id=OBJECT_ID('[p_Message]') AND name='FkGUID')
BEGIN
	ALTER TABLE [dbo].[p_Message] ADD FkGUID uniqueidentifier   	
	declare @CurrentUser sysname
	select @CurrentUser = user_name()
	execute sp_addextendedproperty 'MS_Description',  '消息来源GUID' ,'user', @CurrentUser, 'table', 'p_Message', 'column', 'FkGUID'
END
GO
