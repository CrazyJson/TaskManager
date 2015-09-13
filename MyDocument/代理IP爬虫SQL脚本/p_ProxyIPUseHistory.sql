---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_ProxyIPUseHistory]'))
BEGIN
/*==============================================================*/
/* Table: p_ProxyIPUseHistory                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_ProxyIPUseHistory](
	[ProxyIPHistoryGuid] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[ProxyIP] nvarchar(50)   ,
	[CreatedOn] datetime DEFAULT GETDATE(),
	[CreateDay] nvarchar(10) DEFAULT convert(varchar(10),GETDATE(),120),
	[Type] nvarchar(50), 
	PRIMARY KEY(ProxyIPHistoryGuid)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '代理IP使用历史记录','user', @CurrentUser, 'table', 'p_ProxyIPUseHistory'
execute sp_addextendedproperty 'MS_Description',  '代理IP使用历史记录GUID' ,'user', @CurrentUser, 'table', 'p_ProxyIPUseHistory', 'column', 'ProxyIPHistoryGuid'
execute sp_addextendedproperty 'MS_Description',  '代理ip(包含端口)' ,'user', @CurrentUser, 'table', 'p_ProxyIPUseHistory', 'column', 'ProxyIp'
execute sp_addextendedproperty 'MS_Description',  '创建时间' ,'user', @CurrentUser, 'table', 'p_ProxyIPUseHistory', 'column', 'CreatedOn'
execute sp_addextendedproperty 'MS_Description',  '创建日期(年月日)' ,'user', @CurrentUser, 'table', 'p_ProxyIPUseHistory', 'column', 'CreateDay'
execute sp_addextendedproperty 'MS_Description',  '类型用来标识是哪个模块使用' ,'user', @CurrentUser, 'table', 'p_ProxyIPUseHistory', 'column', 'Type'

END
GO

