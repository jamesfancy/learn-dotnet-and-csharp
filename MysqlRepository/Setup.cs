using FancyIdea.DependencyInjection.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace FancyIdea.DependencyInjection.Mysql;

public static class Setup {
    public static ServiceCollection AddMysqlRepository(this ServiceCollection services) {
        services.AddTransient<IRepository, MysqlRepository>();
        return services;
    }
}
