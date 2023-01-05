using FancyIdea.DependencyInjection.Mysql;
using FancyIdea.DependencyInjection.Sample;
using FancyIdea.DependencyInjection.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// 现在的 Program 是应用入口，也是应用初始化程序，干了 Startup 的工作
// 后面通过业务主类开始进行业务处理
// 这样就在应用程序内把初始化部分和业务处理部分解耦了
// 初始化部分会配置各种抽象接口的实现
// 业务处理部分则直接基于抽象层进行业务处理，不直接使用具体实现

// 初始化应用，读取配置
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .AddJsonFile("appsettings.local.json", true)
    .Build();
var dbType = configuration["dbType"]?.Trim().ToLower();

// 使用微软的 DI 框架，配置 ServiceCollection
ServiceCollection services = new();

// 将配置接口和实现关联配置工作交给每个数据库的实现库去完成，
// 只需要约定统一的一个配置入口，由主程序调用就行
_ = dbType switch {
    "mysql" => services.AddMysqlRepository(),
    _ => services.AddSqliteRepository(),
};

// DI 框架创建对象的时候会自动应用“构造函数注入方法”，
// 我们可以利用这一特点，用框架来产生 BusinessService 对象，
// 避免手工选择注入对象类型并产生对应实例（这就是 DI 框架存在的意义）
// 为此需要把 BusinessService 也配置进去，抽象接口和实现类都是它
services.AddTransient<BusinessService>();

// 产生实例，处理业务
var serviceProvider = services.BuildServiceProvider();
var service = serviceProvider.GetService<BusinessService>()
    ?? throw new InvalidOperationException("Cannot create BusinessService instnace");
service?.ProcessBusiness();
