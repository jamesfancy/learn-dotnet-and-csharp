using FancyIdea.DependencyInjection.Sqlite;

SomeRepository repo = new();
WriteLine($"[{repo.GetDbType()}] {repo.GetVersion()}");
