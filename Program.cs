using VisualParser.Core;

namespace VisualParser
{
    internal static class Program {
        static void Main(string[] args) {
            InfoManager.HandleInfo();
            Console.Write("Press any key to exit: ");
            Console.ReadKey();
        }
    }
}