using FancyIdea.DependencyInjection.Abstraction;
using MySqlConnector;

namespace FancyIdea.DependencyInjectionSample.Repository;

public class MysqlRepository : IRepository {
    public string GetDbType() => "MySQL";

    public string GetVersion() {
        var csBuilder2 = new MySqlConnectionStringBuilder {
            Server = "mysql.local",
            Database = "test",
            UserID = "test",
            Password = "test@2023"
        };

        using var conn = new MySqlConnection(csBuilder2.ConnectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "select version()";
        return cmd.ExecuteScalar() as string ?? "unknown";
    }
}
