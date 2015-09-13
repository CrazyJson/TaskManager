---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_ExpressInfo]'))
BEGIN
/*==============================================================*/
/* Table: p_ExpressInfo                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_ExpressInfo](
	[ExpressGUID] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[ExpressNo] nvarchar(100)   ,
	[ExpressCompanyCode] nvarchar(200)   ,
	[Receiver] nvarchar(500)   ,
	[CreatedOn] datetime   DEFAULT getdate() ,
	PRIMARY KEY(ExpressGUID)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '快递单号信息','user', @CurrentUser, 'table', 'p_ExpressInfo'
execute sp_addextendedproperty 'MS_Description',  '快递信息GUID' ,'user', @CurrentUser, 'table', 'p_ExpressInfo', 'column', 'ExpressGUID'
execute sp_addextendedproperty 'MS_Description',  '快递单号' ,'user', @CurrentUser, 'table', 'p_ExpressInfo', 'column', 'ExpressNo'
execute sp_addextendedproperty 'MS_Description',  '快递公司简称' ,'user', @CurrentUser, 'table', 'p_ExpressInfo', 'column', 'ExpressCompanyCode'
execute sp_addextendedproperty 'MS_Description',  '信息接收人' ,'user', @CurrentUser, 'table', 'p_ExpressInfo', 'column', 'Receiver'
execute sp_addextendedproperty 'MS_Description',  '创建日期' ,'user', @CurrentUser, 'table', 'p_ExpressInfo', 'column', 'CreatedOn'

END
GO
