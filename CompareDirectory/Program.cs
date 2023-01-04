using static System.Console;

namespace FancyIdea.CompareDirectory {
    public static class Program {
        public static void Main(string[] args) {
            // args[0] 源目录
            // args[1] 备份目录
            var result = new Comparer().Compare(args[0], args[1]);
            WriteLine();
            Write(result ? "[OK] " : "[DIFF] ");
            WriteLine(result ? "两个目录完全相同" : "两个目录存在差异，详见上面的信息");
        }
    }
}