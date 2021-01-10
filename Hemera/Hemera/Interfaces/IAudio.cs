namespace Hemera.Interfaces
{
    public interface IAudio
    {
        bool checkPermission();
        bool startRecord(string filePath);
        void stopRecord();
    }
}
