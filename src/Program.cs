using VisualParser.Core;

namespace VisualParser
{
    internal static class Program {
        static void Main(string[] args) {
            // Handles info and driver installation if needed
            InfoManager.HandleInfo();
            Console.Write("Press any key to exit: ");
            Console.ReadKey();
        }
    }
}