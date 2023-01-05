using FancyIdea.DependencyInjection.Sqlite;
using Microsoft.Data.Sqlite;
//using MySqlConnector;

// 这一版实现了简单的属性注入和参数注入，并把初始化代码解耦出来
// 这样换数据库的时候只需要修改初始化代码，不需要修改业务代码

SomeRepository repo = new() {
    DbType = "SQLite"       // 通过属性注入数据库类型
};

WriteLine($"[{repo.GetDbType()}] {repo.GetVersion(createSqliteConnection(), "select sqlite_version()")}");

SqliteConnection createSqliteConnection() {
    var csBuilder = new SqliteConnectionStringBuilder {
        DataSource = """d:\james\desktop\diSample.sqlite3""",
    };

    return new(csBuilder.ConnectionString);
}

//MySqlConnection createMysqlConnection() {
//    var csBuilder2 = new MySqlConnectionStringBuilder {
//        Server = "mysql.local",
//        Database = "test",
//        UserID = "test",
//        Password = "test@2023"
//    };

//    return new MySqlConnection(csBuilder2.ConnectionString);
//}