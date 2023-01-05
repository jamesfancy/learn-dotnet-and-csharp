using Microsoft.Data.Sqlite;
//using MySqlConnector;

namespace FancyIdea.DependencyInjection.Sqlite;

// sqlite version
public class SomeRepository {
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

// mysql version
//public class SomeRepository {
//    public string GetDbType() => "MySQL";

//    public string GetVersion() {
//        var csBuilder2 = new MySqlConnectionStringBuilder {
//            Server = "mysql.local",
//            Database = "test",
//            UserID = "test",
//            Password = "test@2023"
//        };

//        using var conn = new MySqlConnection(csBuilder2.ConnectionString);
//        conn.Open();
//        var cmd = conn.CreateCommand();
//        cmd.CommandText = "select version()";
//        return cmd.ExecuteScalar() as string ?? "unknown";
//    }
//}
