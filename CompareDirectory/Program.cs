using Viyi.Util.Extensions;
using Viyi.Util.Linq;
using static System.Console;
// args[0] 源目录
// args[1] 备份目录

var sourceRoot = Path.GetFullPath(args[0]);
var backupRoot = Path.GetFullPath(args[1]);
var sourcePrefixLength = sourceRoot.Length + 1;
var backupPrefixLength = backupRoot.Length + 1;

var result = Compare(sourceRoot, backupRoot);
WriteLine();
Write(result ? "[OK] " : "[DIFF] ");
WriteLine(result ? "两个目录完全相同" : "两个目录存在差异，详见上面的信息");

bool Compare(string source, string backup) {
    if (!Directory.Exists(source)) { return false; }
    if (!Directory.Exists(backup)) { return false; }

    bool entiresResult = compareDirectEntires();

    // 进子目录
    var sourcceSubs = Directory.EnumerateDirectories(source).Select(Path.GetFileName);
    var backupSubs = Directory.EnumerateDirectories(backup).Select(Path.GetFileName);
    var subs = sourcceSubs.Intersect(backupSubs);

    var result = entiresResult;
    foreach (var sub in subs) {
        var subResult = Compare(Path.Combine(source, sub!), Path.Combine(backup, sub!));
        result = result && subResult;
    }

    return result;

    // local function
    bool compareDirectEntires() {
        var sourceFiles = Directory.EnumerateFileSystemEntries(source)
            .Select(filePath => Path.GetFileName(filePath))
            .ToHashSet();

        var backupFiles = Directory.EnumerateFileSystemEntries(backup)
            .Select(filePath => Path.GetFileName(filePath))
            .ToHashSet();

        var additional = backupFiles
            .Except(sourceFiles)
            .Peek(filename => {
                var path = Path.Combine(backup, filename);
                var dirMark = isDirectory(path) ? " [DIR]" : "";
                WriteLine($"[+] {path[backupPrefixLength..]}{dirMark}");
            });

        var reduce = sourceFiles
            .Except(backupFiles)
            .Also(reduce => reduce.ForEach(filename => {
                var path = Path.Combine(source, filename);
                var dirMark = isDirectory(path) ? " [DIR]" : "";
                WriteLine($"[-] {path[sourcePrefixLength..]}{dirMark}");
            }));

        return !(additional.Any() || reduce.Any());
    }

    static bool isDirectory(string path) => Directory.Exists(path);
}