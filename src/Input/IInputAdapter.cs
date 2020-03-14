namespace BulkImageResizer {
    public interface IInputAdapter {
        string ReadLine();
        void WriteLine(string line);
    }
}