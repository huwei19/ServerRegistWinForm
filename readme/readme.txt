1. 关联 分服模拟器.png
2. 开发环境 vs2019
3. exe输出目录
	a. 需包含resource文件夹
		例：bin\Debug\resource
		Config.txt模拟器配置文件
		ServerConfigList.txt服务器配置文件
	b. log日志输出文件
		bin\Debug\log\format_20220803.log
4. Program.cs 程序主入口
	a. 注册并初始化日志
	b. 初始化模拟器
	c. 开启winform窗体
5. EmulatorEntity.cs文件
	服务器信息类：ServerInfo
	注册权重类：RegRateInfo
	模拟器配置类：EmulatorConfig
6. Form1.cs 文件
	a. Form1_Load()方法，初始化UI的展示
	b. 关键的2个计时器Timer空间，
		一个每1秒执行，用来触发模拟器核心逻辑，如每秒登陆、每秒注册、每秒登出等
		一个每0.1秒执行，用来实时更新，listview中的进度条和注册按钮位置；
7. Form1_ListView.cs 文件
	用来处理 form窗口中的 ListView具体逻辑，如UI展示更新，UI位置更新等
8. GameLog.cs文件
	日志处理文件
9. EmulatorManager.cs文件
	模拟器核心文件，数据存储、每秒处理逻辑等都在里面