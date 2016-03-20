---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_Task]'))
BEGIN
/*==============================================================*/
/* Table: p_Task                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_Task](
	[TaskID] uniqueidentifier   DEFAULT newsequentialid() ,
	[TaskName] nvarchar(300)   ,
	[TaskParam] nvarchar(max)   ,
	[CronExpressionString] nvarchar(200)   ,
	[Assembly] nvarchar(150)   ,
	[Class] nvarchar(150)   ,
	[Status] int   DEFAULT 0 ,
	[CreatedOn] datetime   DEFAULT getdate() ,
	[ModifyOn] datetime   ,
	[RecentRunTime] datetime   ,
	[LastRunTime] datetime   ,
	[CronRemark] nvarchar(300)   ,
	[Remark] nvarchar(1000)   ,
	PRIMARY KEY(TaskID)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '任务表','user', @CurrentUser, 'table', 'p_Task'
execute sp_addextendedproperty 'MS_Description',  '任务ID' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'TaskID'
execute sp_addextendedproperty 'MS_Description',  '任务名称' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'TaskName'
execute sp_addextendedproperty 'MS_Description',  '任务参数' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'TaskParam'
execute sp_addextendedproperty 'MS_Description',  '任务运行Cron表达式' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'CronExpressionString'
execute sp_addextendedproperty 'MS_Description',  '程序集名称' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'Assembly'
execute sp_addextendedproperty 'MS_Description',  '任务所在类包含命名空间' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'Class'
execute sp_addextendedproperty 'MS_Description',  '任务运行状态    0:运行 1:停止' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'Status'
execute sp_addextendedproperty 'MS_Description',  '创建时间' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'CreatedOn'
execute sp_addextendedproperty 'MS_Description',  '修改时间' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'ModifyOn'
execute sp_addextendedproperty 'MS_Description',  '最近运行时间' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'RecentRunTime'
execute sp_addextendedproperty 'MS_Description',  '下次运行时间' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'LastRunTime'
execute sp_addextendedproperty 'MS_Description',  '表达式中文说明' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'CronRemark'
execute sp_addextendedproperty 'MS_Description',  '备注' ,'user', @CurrentUser, 'table', 'p_Task', 'column', 'Remark'

END
GO

DELETE FROM p_Task WHERE TaskID IN('5ebaa648-d1e8-e511-b79d-54ee75868db8','5fbaa648-d1e8-e511-b79d-54ee75868db8','60baa648-d1e8-e511-b79d-54ee75868db8','61baa648-d1e8-e511-b79d-54ee75868db8')
INSERT INTO dbo.p_Task
        ( 
		  TaskID ,
          TaskName ,
          TaskParam ,
          CronExpressionString ,
          Assembly ,
          Class ,
		  Status,
          CronRemark 
        )
SELECT '5ebaa648-d1e8-e511-b79d-54ee75868db8' AS TaskID,'爬虫-获取代理IP' AS TaskName,'{"IPUrl":"http://www.xicidaili.com/nn","DefaultProxyIp":"",IsPingIp:false}' AS TaskParam,
'0 0 */1 * * ?' AS CronExpressionString,'Ywdsoft.Task' AS ASSEMBLY,'Ywdsoft.Task.TaskSet.IpProxyJob' AS Class,0 as Status,'每1个小时运行一次' AS CronRemark
UNION ALL
SELECT '5fbaa648-d1e8-e511-b79d-54ee75868db8','测试任务,输出当前时间','','0/10 * * * * ?' ,'Ywdsoft.Task','Ywdsoft.Task.TaskSet.TestJob',0,'每10秒运行一次'
UNION ALL
SELECT '60baa648-d1e8-e511-b79d-54ee75868db8','快递进度信息','222.45.58.64:8118','0 0 */1 * * ?' ,'Ywdsoft.Task','Ywdsoft.Task.TaskSet.ExpressProgressJob',0,'每1个小时运行一次'
UNION ALL
SELECT '61baa648-d1e8-e511-b79d-54ee75868db8','发送信息任务','','0 0/3 6-23 * * ?' ,'Ywdsoft.Task','Ywdsoft.Task.TaskSet.SendMessageJob',0,'每天6:00-23:00每3分钟运行一次'

GO
