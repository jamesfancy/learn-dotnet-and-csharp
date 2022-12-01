using System.Security.Cryptography;
using Viyi.Strings.Codec.Base16;
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

long getFileSize(string filePath) => new FileInfo(filePath).Length;

string calcFileHash(string filePath) {
    var md5 = MD5.Create();
    using var stream = new FileStream(filePath, FileMode.Open);
    byte[] buffer = new byte[4 * 1024];

    int length;
    while ((length = stream.Read(buffer)) > 0) {
        md5.TransformBlock(buffer, 0, length, null, 0);
    }
    md5.TransformFinalBlock(buffer, 0, 0);
    return md5.Hash?.EncodeBase16() ?? "";
}

bool Compare(string source, string backup) {
    if (!Directory.Exists(source)) { return false; }
    if (!Directory.Exists(backup)) { return false; }

    bool filesResult = compareFiles(source, backup);
    bool entiresResult = compareDirectEntires();
    bool result = compareInSubs(source, backup);

    return filesResult && entiresResult && result;

    bool compareFiles(string source, string backup) {
        var sourceFiles = Directory.EnumerateFiles(source).Select(Path.GetFileName);
        var backupFiles = Directory.EnumerateFiles(backup).Select(Path.GetFileName);
        var files = sourceFiles.Intersect(backupFiles);

        bool result = true;
        foreach (var filename in files.NotNull()) {
            var sourceFile = Path.Combine(source, filename);
            var backupFile = Path.Combine(backup, filename);

            if (
                getFileSize(sourceFile) != getFileSize(backupFile)
                || calcFileHash(sourceFile) != calcFileHash(backupFile)
            ) {
                result = false;
                var path = Path.Combine(backup, filename);
                WriteLine($"[x] {path[backupPrefixLength..]}");
            }
        }
        return result;
    }

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

    bool compareInSubs(string source, string backup) {
        // 进子目录
        var sourceSubs = Directory.EnumerateDirectories(source).Select(Path.GetFileName);
        var backupSubs = Directory.EnumerateDirectories(backup).Select(Path.GetFileName);
        var subs = sourceSubs.Intersect(backupSubs);

        var result = true;
        foreach (var sub in subs) {
            var subResult = Compare(Path.Combine(source, sub!), Path.Combine(backup, sub!));
            result = result && subResult;
        }

        return result;
    }
}