---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_MessageHistory]'))
BEGIN
/*==============================================================*/
/* Table: p_MessageHistory                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_MessageHistory](
	[MessageHistoryGuid] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[MessageGuid] uniqueidentifier   ,
	[Receiver] nvarchar(50)   ,
	[Type] tinyint   DEFAULT 0 ,
	[Content] nvarchar(MAX)   ,
	[Subject] nvarchar(200)   ,
	[CreatedOn] datetime   ,
	[SendOn] datetime   DEFAULT getdate() ,
	[Staue] tinyint   DEFAULT 0 ,
	[Remark] nvarchar(max),
	PRIMARY KEY(MessageHistoryGuid)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '消息历史表','user', @CurrentUser, 'table', 'p_MessageHistory'
execute sp_addextendedproperty 'MS_Description',  '消息历史GUID' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'MessageHistoryGuid'
execute sp_addextendedproperty 'MS_Description',  '消息GUID' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'MessageGuid'
execute sp_addextendedproperty 'MS_Description',  '消息接收人' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'Receiver'
execute sp_addextendedproperty 'MS_Description',  '消息类型    0：短信 1：邮件' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'Type'
execute sp_addextendedproperty 'MS_Description',  '信息内容' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'Content'
execute sp_addextendedproperty 'MS_Description',  '邮件主题' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'Subject'
execute sp_addextendedproperty 'MS_Description',  '消息创建日期' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'CreatedOn'
execute sp_addextendedproperty 'MS_Description',  '消息发送日期' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'SendOn'

execute sp_addextendedproperty 'MS_Description',  '消息发送状态(
 0:发送成功
 1:发送失败
)' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'Staue'
execute sp_addextendedproperty 'MS_Description',  '备注' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'Remark'

END
GO

GO
IF NOT EXISTS(SELECT 1 FROM syscolumns WHERE id=OBJECT_ID('[p_MessageHistory]') AND name='FromType')
BEGIN
	ALTER TABLE [dbo].[p_MessageHistory] ADD FromType NVARCHAR(200)   	
	declare @CurrentUser sysname
	select @CurrentUser = user_name()
	execute sp_addextendedproperty 'MS_Description',  '消息来源(eg:快递进度)' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'FromType'
END
GO

GO
IF NOT EXISTS(SELECT 1 FROM syscolumns WHERE id=OBJECT_ID('[p_MessageHistory]') AND name='FkGUID')
BEGIN
	ALTER TABLE [dbo].[p_MessageHistory] ADD FkGUID uniqueidentifier   	
	declare @CurrentUser sysname
	select @CurrentUser = user_name()
	execute sp_addextendedproperty 'MS_Description',  '消息来源GUID' ,'user', @CurrentUser, 'table', 'p_MessageHistory', 'column', 'FkGUID'
END
GO
