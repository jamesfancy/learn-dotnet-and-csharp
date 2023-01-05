using FancyIdea.DependencyInjection.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace FancyIdea.DependencyInjection.Sqlite;

public static class Setup {
    public static ServiceCollection AddSqliteRepository(this ServiceCollection services) {
        services.AddTransient<IRepository, SqliteRepository>();
        return services;
    }
}
