using FancyIdea.DependencyInjection.Abstraction;
using FancyIdea.DependencyInjection.Sqlite;
using FancyIdea.DependencyInjectionSample.Repository;
using Microsoft.Extensions.Configuration;

// 初始化应用，读取配置
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false)
    .AddJsonFile("appsettings.local.json", true)
    .Build();

var dbType = configuration["dbType"]?.Trim().ToLower();

IRepository repo = dbType switch {
    "mysql" => new MysqlRepository(),
    _ => new SqliteRepository()
};

WriteLine($"[{repo.GetDbType()}] {repo.GetVersion()}");
