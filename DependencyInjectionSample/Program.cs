using FancyIdea.DependencyInjection.Sample;
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

// 使用 RepositoryLoader 根据配置动态加载 Repository 实现，
// 现在 DependencyInjectionSample 是的数据库已经不需要修改任何代码了
// 注1：DependencyInjectionSample 已经去掉了对两个 Repository 实现项目的引用
// 主2：运行前需要手工拷贝需要的实现库 (XxxRepository.dll) 到生成目标目录
new RepositoryLoader(services).Load(dbType ?? "");

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
