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
