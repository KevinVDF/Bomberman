using System.Windows.Media;
using ClientWPF.MVVM;

namespace ClientWPF.ViewModels.StartedGame
{
    public abstract class LivingObjectItem : ObservableObject
    {
        private Brush _imageInUse;
        public Brush ImageInUse
        {
            get { return _imageInUse; }
            set { Set(() => ImageInUse, ref _imageInUse, value); }
        }

        private int _positionX;
        public int PositionX
        {
            get { return _positionX; }
            set { Set(() => PositionX,ref _positionX, (value * 50)+ 100); }
        }

        private int _positionY;
        public int PositionY
        {
            get { return _positionY; }
            set { Set(()=>PositionY, ref _positionY , value*50); }
        }
    }
}
