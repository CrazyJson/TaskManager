---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_IPProxy]'))
BEGIN
/*==============================================================*/
/* Table: p_IPProxy                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_IPProxy](
	[ProxyGuid] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[Country] nvarchar(50)   ,
	[IP] nvarchar(100)   ,
	[Port] nvarchar(10)   ,
	[ProxyIp] nvarchar(110)   ,
	[Position] nvarchar(100)   ,
	[Anonymity] nvarchar(20)   ,
	[Type] nvarchar(20)   ,
	[Speed] varchar(20)   ,
	[ConnectTime] varchar(20)   ,
	[VerifyTime] varchar(20)   ,
	PRIMARY KEY(ProxyGuid)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 'ip代理','user', @CurrentUser, 'table', 'p_IPProxy'
execute sp_addextendedproperty 'MS_Description',  '代理GUID' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'ProxyGuid'
execute sp_addextendedproperty 'MS_Description',  '国家简称' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'Country'
execute sp_addextendedproperty 'MS_Description',  'ip代理地址' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'IP'
execute sp_addextendedproperty 'MS_Description',  '代理端口' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'Port'
execute sp_addextendedproperty 'MS_Description',  '代理ip(包含端口)' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'ProxyIp'
execute sp_addextendedproperty 'MS_Description',  'ip位置' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'Position'
execute sp_addextendedproperty 'MS_Description',  '匿名类型' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'Anonymity'
execute sp_addextendedproperty 'MS_Description',  'http类型' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'Type'
execute sp_addextendedproperty 'MS_Description',  '连接速度' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'Speed'
execute sp_addextendedproperty 'MS_Description',  '连接时间' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'ConnectTime'
execute sp_addextendedproperty 'MS_Description',  '验证时间' ,'user', @CurrentUser, 'table', 'p_IPProxy', 'column', 'VerifyTime'

END
GO
