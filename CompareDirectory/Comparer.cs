using System.Security.Cryptography;
using Viyi.Strings.Codec.Base16;
using Viyi.Util.Linq;
using static System.Console;

namespace FancyIdea.CompareDirectory;

class Comparer {
    private int sourcePrefixLength;
    private int backupPrefixLength;

    public bool Compare(string source, string backup) {
        var sourceRoot = Path.GetFullPath(source);
        var backupRoot = Path.GetFullPath(backup);

        sourcePrefixLength = sourceRoot.Length + 1;
        backupPrefixLength = backupRoot.Length + 1;

        return InternalCompare(source, backup);
    }


    bool InternalCompare(string source, string backup) {
        if (!Directory.Exists(source)) { return false; }
        if (!Directory.Exists(backup)) { return false; }

        bool entiresResult = CompareDirectEntires(source, backup);
        bool filesResult = CompareFiles(source, backup);
        bool result = compareInSubs(source, backup);

        return filesResult && entiresResult && result;
    }

    private bool CompareFiles(string source, string backup) {
        var sourceFiles = Directory.EnumerateFiles(source).Select(Path.GetFileName);
        var backupFiles = Directory.EnumerateFiles(backup).Select(Path.GetFileName);
        var files = sourceFiles.Intersect(backupFiles);

        bool result = true;
        foreach (var filename in files.NotNull()) {
            var sourceFile = Path.Combine(source, filename);
            var backupFile = Path.Combine(backup, filename);

            if (
                GetFileSize(sourceFile) != GetFileSize(backupFile)
                || CalcFileHash(sourceFile) != CalcFileHash(backupFile)
            ) {
                result = false;
                var path = Path.Combine(backup, filename);
                WriteLine($"[x] {path[backupPrefixLength..]}");
            }
        }
        return result;
    }

    private bool compareInSubs(string source, string backup) {
        // 进子目录
        var sourceSubs = Directory.EnumerateDirectories(source).Select(Path.GetFileName);
        var backupSubs = Directory.EnumerateDirectories(backup).Select(Path.GetFileName);
        var subs = sourceSubs.Intersect(backupSubs);

        var result = true;
        foreach (var sub in subs) {
            var subResult = InternalCompare(Path.Combine(source, sub!), Path.Combine(backup, sub!));
            result = result && subResult;
        }
        return result;
    }

    private bool CompareDirectEntires(string source, string backup) {
        var sourceFiles = new HashSet<string>();    // reduce
        // length 6, 0 ~ 5 < 6
        foreach (string entry in Directory.GetFileSystemEntries(source)) {
            sourceFiles.Add(Path.GetFileName(entry));
        }

        // 备份目录里多余的
        List<string> additional = new();
        foreach (string entry in Directory.EnumerateFileSystemEntries(backup)) {
            var filename = Path.GetFileName(entry);
            if (!sourceFiles.Remove(filename)) {
                additional.Add(filename);
            }
        }
        var reduce = sourceFiles;

        foreach (var filename in additional) {
            printDiff(Path.Combine(backup, filename), backupPrefixLength, "+");
        }

        foreach (var filename in reduce) {
            printDiff(Path.Combine(source, filename), sourcePrefixLength, "-");
        }

        return additional.Count <= 0 && reduce.Count <= 0;

        static void printDiff(string path, int prefixLength, string mark) {
            var dirMark = IsDirectory(path) ? " [DIR]" : "";
            WriteLine($"[{mark}] {path[prefixLength..]}{dirMark}");
        }
    }

    private static bool IsDirectory(string path) =>
        new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory);

    private static string CalcFileHash(string filePath) {
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

    private static long GetFileSize(string filePath) => new FileInfo(filePath).Length;
}
