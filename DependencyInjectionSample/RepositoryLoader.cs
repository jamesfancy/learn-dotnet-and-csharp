using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.Loader;
using Viyi.Util.Extensions;

namespace FancyIdea.DependencyInjection.Sample;

internal class RepositoryLoader {
    AssemblyLoadContext? loadContext;
    readonly ServiceCollection services;

    public RepositoryLoader(ServiceCollection services) {
        this.services = services;
    }

    public void Load(string dbType) {
        dbType = dbType.LetWhenNot(dbType == "mysql", "sqlite");
        var dllPath = GetDllPath(dbType);

        loadContext = new AssemblyLoadContext("RepositoryContext");
        var assembly = loadContext.LoadFromAssemblyPath(dllPath);
        var setup = assembly.GetTypes()
            .Where(type => type.Name == "Setup")
            .OrderBy(type => type.FullName?.Length)
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Not found 'Setup' class");
        AddToService(setup);

        WriteLine($"[info] loaded {dllPath}");
    }

    public void Unload() {
        loadContext?.Unload();
    }

    void AddToService(Type setup) {
        var method = setup.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name.Let(name => name.StartsWith("Add") && name.EndsWith("Repository")))
            .Where(m => m.GetParameters()
                .Let(parameters => parameters.Length == 1
                    && parameters[0].ParameterType.IsAssignableFrom(typeof(ServiceCollection))
                )
            )
            .FirstOrDefault()
            ?? throw new InvalidOperationException("Not found method to register services");
        method.Invoke(null, new object?[] { services });
    }

    static string GetDllPath(string dbType) {
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var targetName = $"{dbType}Repository.dll";
        var filename = Directory.EnumerateFiles(dir, "*Repository.dll")
            .Select(path => Path.GetFileName(path))
            .FirstOrDefault(name => string.Compare(name, targetName, true) == 0)
            ?? throw new InvalidOperationException($"Cannot load {targetName} (ignore case).");
        return Path.Combine(dir, filename);
    }
}
