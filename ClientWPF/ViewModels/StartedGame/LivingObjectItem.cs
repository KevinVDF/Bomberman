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

        public int PositionX
        {
            get { return X*50 + (50 - Width)/2; }
        }

        public int PositionY
        {
            get { return Y*50 + (50 - Height)/2; }
        }

        private int _x;
        public int X
        {
            get { return _x; }
            set
            {
                if(Set(() => X, ref _x, value))
                    OnPropertyChanged("PositionX");
            }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set
            {
                if(Set(() => Y, ref _y, value))
                    OnPropertyChanged("PositionY");
            }
        }
        
        private int _zIndex;
        public int ZIndex
        {
            get { return _zIndex; }
            set { Set(() => ZIndex, ref _zIndex, value); }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set
            {
                if (Set(() => Width, ref _width, value))
                    OnPropertyChanged("PositionX");
            }
        }

        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                if (Set(() => Height, ref _height, value))
                    OnPropertyChanged("PositionY");
            }
        }
        
    }
}
