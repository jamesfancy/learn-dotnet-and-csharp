using System.Data.Common;

namespace FancyIdea.DependencyInjection.Sqlite;

// 注意到这个文件，已经不依赖任何 SQLite 或 MySQL 的命名空间，
// 这表示已经提取了公共业务代码

public class SomeRepository {
    // 通过属性注入 DbType
    public string? DbType { get; set; }

    public string GetDbType() => DbType ?? "Unknown";

    // 通过参数注入数据库连接对象和命令
    public string GetVersion(DbConnection conn, string sql) {
        conn.Open();
        DbCommand cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        return cmd.ExecuteScalar() as string ?? "unknown";
    }
}
