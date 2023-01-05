using FancyIdea.DependencyInjection.Abstraction;
using FancyIdea.DependencyInjection.Sample;
using FancyIdea.DependencyInjection.Sqlite;
using FancyIdea.DependencyInjectionSample.Repository;
using Microsoft.Extensions.Configuration;

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

// 注意这里是手工产生的 Repository 对象，并手工注入 BusinessService
IRepository repo = dbType switch {
    "mysql" => new MysqlRepository(),
    _ => new SqliteRepository()
};

// 初始化完成后，调用业务操作入口，开始业务处理
var service = new BusinessService(repo);
service.ProcessBusiness();
