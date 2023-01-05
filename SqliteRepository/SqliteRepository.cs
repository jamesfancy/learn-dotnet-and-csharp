using FancyIdea.DependencyInjection.Abstraction;
using Microsoft.Data.Sqlite;

namespace FancyIdea.DependencyInjection.Sqlite;

public class SqliteRepository : IRepository {
    public string GetDbType() => "SQLite";

    public string GetVersion() {
        var csBuilder = new SqliteConnectionStringBuilder {
            DataSource = """d:\james\desktop\diSample.sqlite3""",
        };

        using SqliteConnection sqliteConn = new(csBuilder.ConnectionString);

        sqliteConn.Open();
        SqliteCommand cmd = sqliteConn.CreateCommand();
        cmd.CommandText = "select sqlite_version()";
        return cmd.ExecuteScalar() as string ?? "unknown";
    }
}
