# TaskManager
任务管理平台
1.	系统简介
你曾经需要应用执行一个任务吗？这个任务每天或每周星期二晚上11：30，或许仅仅每个月的最后一天执行。一个自动执行而无须干预的任务在执行过程中如果发生一个严重错误，应用能够知到其执行失败并尝试重新执行吗？你和你的团队是用.NET编程吗？如果这些问题中任何一个你回答是，那么你可以使用Quartz.NET调度器。 Quartz.NET允许开发人员根据时间间隔（或天）来调度作业。它实现了作业和触发器的多对多关系，还能把多个作业与不同的触发器关联。

本系统通过window服务来集成Quartz.net,通过修改配置文件和添加相应Job即可完成作业添加，使用简单方便。

2.	项目结构
系统目前包含六个项目,如下图所示
 
3.	开发过程
添加一个新任务步骤如下
	在Mysoft.Task项目TaskSet文件夹下添加继承IJob接口的任务类,可以参照现有的IpProxyJob
 
 
	修改TaskConfig.xml配置文件
添加好具体的任务实现之后,需要配置任务的相关执行参数了。TaskConfig.xml在Mysoft.Task项目下
 
每个节点的意思都清楚的注释了，现在为我们刚加上的TestJob任务添加相关配置
  <Task>
    <TaskID>2</TaskID>
    <TaskName>测试任务,输出当前时间</TaskName>
    <TaskParam></TaskParam>
    <!--运行频率设置 每30分钟运行一次-->
    <CronExpressionString>* */30 * * * ?</CronExpressionString>
    <NameSpace>Mysoft.Task.TaskSet</NameSpace>
    <Class>TestJob</Class>
    <IsExcute>true</IsExcute>
  </Task>

最重要的部分运行频率CronExpressionString需要怎么写？这里也替大家考虑到了，可以使用在线Cron生成器来生成，地址http://jason.hahuachou.com/cron/index.htm
 
	完成以上两步,即完成了一个新任务的添加，是不是很简单呢。
4.	安装部署
由于项目使用的是Window服务来搭载Quartz.net,那么项目的安装即安装Window服务。
本来可以通过批处理来完成服务安装卸载的，本人嫌这个太麻烦，于是写了Windows服务安装卸载小工具,源码在WSWinForm项目里面。

生成整个解决方案以后,右键以管理员方式打开WSWinForm.exe文件，选择TaskManager服务进行安装运行
 
运行成功后会生成Logs文件夹，记录执行日志的
 
5.	其它说明
目前系统内置了三个定时任务
1.	代理ip爬虫
每隔一小时从网站上爬取最新的代理IP信息,爬下来的数据存在p_IPProxy表

2.	快递进度查询
可以查询目前各种快递的进度信息,相关表p_ExpressCompany(快递公司表)
p_ExpressInfo(快递信息表) p_ExpressHistoryInfo(快递信息历史记录表)
p_ExpressProcessDetail(快递进度信息表)

目前使用该任务需要自己手工网表p_ExpressInfo里面插入数据
INSERT INTO dbo.p_ExpressInfo( ExpressNo ,ExpressCompanyCode ,Receiver )
SELECT '880373857190629830' AS ExpressNo,'yuantong' AS ExpressCompanyCode,'youemail@qq.com' AS Receiver
UNION ALL
SELECT '9940055317546' AS ExpressNo,'youzhengguonei' AS ExpressCompanyCode,'youemail@qq.com' AS Receiver

其中youemail@qq.com为接收进度信息变更邮箱地址，只要有进度变更，系统会自动发邮件提醒。
ExpressNo：为快递单号
ExpressCompanyCode：快递公司Code  该值可从p_ExpressCompany表里面进行查询
 
CompanyCode即为ExpressCompanyCode

3.	消息通知(邮件)
消息通知相关表p_Message(待发送消息表),p_MessageHistory（已发送或者发送失败消息表）

目前快递任务有信息变更会插入一条消息，消息通知任务3分钟轮询一次p_Message表，对于每个消息最多会进行三次发送(前两次都失败),发送完成将p_Message数据插入到p_MessageHistory

快递进度信息收到的邮件效果如下，方便你随时跟踪你的快递信息
 


4.	系统配置
当然系统要正常运行还需要进行相关配置，配置文件在Config\ Config.xml里面，目前只有两条相关配置
 
数据库连接信息
使用者可以拿从SVN下载代码MyDocument项目下的任务系统”SQL合并脚本_20150911.sql”执行创建相关表和初始数据

邮件服务器信息
系统发邮件的功能时使用的SMTP服务器，目前可使用的有163邮箱，QQ邮箱，关于C#使用SMTP发邮件相关配置可自行百度。配置好这些内容以后
就可以使用windows安装卸载工具来启动任务系统了。
