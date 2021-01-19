using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace Hemera.Models
{
    public class FileData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set
            {
                _FileName = value;
                OnPropertyChanged();
            }
        }

        private string _FullPath;
        public string FullPath
        {
            get => _FullPath;
            set
            {
                _FullPath = value;
                OnPropertyChanged();
            }
        }

        public FileData(FileResult fileResult)
        {
            FileName = fileResult.FileName;
            FullPath = fileResult.FullPath;
        }

        public FileData()
        {
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
