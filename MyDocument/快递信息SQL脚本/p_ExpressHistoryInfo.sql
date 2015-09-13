---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_ExpressHistoryInfo]'))
BEGIN
/*==============================================================*/
/* Table: p_ExpressHistoryInfo                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_ExpressHistoryInfo](
	[ExpressHistoryGUID] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[ExpressGUID] uniqueidentifier   ,
	[ExpressNo] nvarchar(100)   ,
	[ExpressCompanyCode] nvarchar(200)   ,
	[Receiver] nvarchar(500)   ,
	[State] tinyint,
	[CreatedOn] datetime   DEFAULT getdate() ,
	PRIMARY KEY(ExpressHistoryGUID)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '快递单号历史信息','user', @CurrentUser, 'table', 'p_ExpressHistoryInfo'
execute sp_addextendedproperty 'MS_Description',  '快递历史信息GUID' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'ExpressHistoryGUID'
execute sp_addextendedproperty 'MS_Description',  '快递信息GUID' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'ExpressGUID'
execute sp_addextendedproperty 'MS_Description',  '快递单号' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'ExpressNo'
execute sp_addextendedproperty 'MS_Description',  '快递公司简称' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'ExpressCompanyCode'
execute sp_addextendedproperty 'MS_Description',  '信息接收人' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'Receiver'
execute sp_addextendedproperty 'MS_Description',  '快递单当前的状态(
0：在途，即货物处于运输过程中；
1：揽件，货物已由快递公司揽收并且产生了第一条跟踪信息；
2：疑难，货物寄送过程出了问题；
3：签收，收件人已签收；
4：退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收；
5：派件，即快递正在进行同城派件；
6：退回，货物正处于退回发件人的途中;)' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'State'
execute sp_addextendedproperty 'MS_Description',  '创建日期' ,'user', @CurrentUser, 'table', 'p_ExpressHistoryInfo', 'column', 'CreatedOn'

END
GO
