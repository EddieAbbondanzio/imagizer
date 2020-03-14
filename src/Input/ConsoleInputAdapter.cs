using System;

namespace BulkImageResizer {
    public class ConsoleInputAdapter : IInputAdapter {
        public string ReadLine() => Console.ReadLine();

        public void WriteLine(string line) => Console.WriteLine(line);
    }
}