using Hemera.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms.Shapes;

namespace Hemera.Models
{
    public class Attachment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Attachment(AttachmentType type, FileResult file)
        {
            Type = type;
            File = new FileData(file);
        }

        public Attachment() { }

        private AttachmentType _Type;
        public AttachmentType Type
        {
            get => _Type;
            set
            {
                _Type = value;

                Path = value switch
                {
                    AttachmentType.File => VarContainer.FileGeometry,
                    AttachmentType.Audio => VarContainer.AudioGeometry,
                };

                OnPropertyChanged();
            }
        }

        private Geometry _Path;
        [XmlIgnore]
        public Geometry Path
        {
            get => _Path;
            set
            {
                _Path = value;
                OnPropertyChanged();
            }
        }

        private FileData _File;
        public FileData File
        {
            get => _File;
            set
            {
                _File = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum AttachmentType : byte
    {
        File,
        Audio
    }
}
