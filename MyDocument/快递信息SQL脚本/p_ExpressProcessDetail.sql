---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_ExpressProcessDetail]'))
BEGIN
/*==============================================================*/
/* Table: p_ExpressProcessDetail                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_ExpressProcessDetail](
	[ExpressProcessDetailGuid] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[GroupNo] int,
	[ExpressNo] nvarchar(100)   ,
	[Time] datetime   ,
	[Context] nvarchar(500)   ,
	PRIMARY KEY(ExpressProcessDetailGuid)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '快递单进度明细','user', @CurrentUser, 'table', 'p_ExpressProcessDetail'
execute sp_addextendedproperty 'MS_Description',  '快递单进度明细GUID' ,'user', @CurrentUser, 'table', 'p_ExpressProcessDetail', 'column', 'ExpressProcessDetailGuid'
execute sp_addextendedproperty 'MS_Description',  '进度明细组内编号' ,'user', @CurrentUser, 'table', 'p_ExpressProcessDetail', 'column', 'GroupNo'
execute sp_addextendedproperty 'MS_Description',  '快递单号' ,'user', @CurrentUser, 'table', 'p_ExpressProcessDetail', 'column', 'ExpressNo'
execute sp_addextendedproperty 'MS_Description',  '跟踪信息的时间' ,'user', @CurrentUser, 'table', 'p_ExpressProcessDetail', 'column', 'Time'
execute sp_addextendedproperty 'MS_Description',  '每条跟综信息的描述' ,'user', @CurrentUser, 'table', 'p_ExpressProcessDetail', 'column', 'Context'

END
GO

GO

GO
IF NOT EXISTS(SELECT 1 FROM syscolumns WHERE id=OBJECT_ID('[p_ExpressProcessDetail]') AND name='State')
BEGIN
	ALTER TABLE [dbo].[p_ExpressProcessDetail] ADD State TINYINT   	
	declare @CurrentUser sysname
	select @CurrentUser = user_name()
	execute sp_addextendedproperty 'MS_Description',  '快递单当前的状态(
	0：在途，即货物处于运输过程中；
	1：揽件，货物已由快递公司揽收并且产生了第一条跟踪信息；
	2：疑难，货物寄送过程出了问题；
	3：签收，收件人已签收；
	4：退签，即货物由于用户拒签、超区等原因退回，而且发件人已经签收；
	5：派件，即快递正在进行同城派件；
	6：退回，货物正处于退回发件人的途中;)' ,'user', @CurrentUser, 'table', 'p_ExpressProcessDetail', 'column', 'State'
END
GO
