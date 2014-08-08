using System;

using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common.Log;

namespace ClientWPF.ViewModels
{
    public class DebugViewModel : ViewModelBase
    {
        private ObservableCollection<Brush> _textures;
        public ObservableCollection<Brush> Textures
        {
            get { return _textures; }
            set { Set(() => Textures, ref _textures, value); }
        }

        public void Initialize()
        {
            Textures = new ObservableCollection<Brush>();
            string GlobalImagePath = ConfigurationManager.AppSettings["ImagePath"];
            string playersImagePath = String.Format(@"{0}\13Bomberman.png", GlobalImagePath);
            BitmapImage imageBmp = new BitmapImage(new Uri(playersImagePath));
            ShowDiffTextures(imageBmp);
        }

        private void ShowDiffTextures(BitmapImage imageBmp)
        {
            Textures.Add(ExtractBackgroundNone(imageBmp, 51, 37, 17, 27));
            Textures.Add(ExtractBackgroundUniform(imageBmp, 51, 37, 17, 27));
            Textures.Add(ExtractBackgroundUniform(imageBmp, 51, 37, 19, 28));
            Textures.Add(ExtractBackgroundUniformToFill(imageBmp, 51, 37, 15, 25));
            Textures.Add(ExtractBackgroundFill(imageBmp, 51, 37, 17, 27));
        }

        private Brush ExtractBackgroundNone(BitmapImage image, int posX, int posY, int width, int height)
        {
            Brush background = null;
            try
            {
                background = new ImageBrush(image)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(posX, posY, width, height),
                    Stretch = Stretch.None
                };
            }
            catch (Exception ex)
            {
                Log.WriteLine(Log.LogLevels.Error, "Error while extracting background texture. Image {0}. Exception: {1}", image.BaseUri, ex);
            }
            return background;
        }

        private Brush ExtractBackgroundUniform(BitmapImage image, int posX, int posY, int width, int height)
        {
            Brush background = null;
            try
            {
                background = new ImageBrush(image)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(posX, posY, width, height),
                    Stretch = Stretch.Uniform
                };
            }
            catch (Exception ex)
            {
                Log.WriteLine(Log.LogLevels.Error, "Error while extracting background texture. Image {0}. Exception: {1}", image.BaseUri, ex);
            }
            return background;
        }

        private Brush ExtractBackgroundUniformToFill(BitmapImage image, int posX, int posY, int width, int height)
        {
            Brush background = null;
            try
            {
                background = new ImageBrush(image)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(posX, posY, width, height),
                    Stretch = Stretch.UniformToFill
                };
            }
            catch (Exception ex)
            {
                Log.WriteLine(Log.LogLevels.Error, "Error while extracting background texture. Image {0}. Exception: {1}", image.BaseUri, ex);
            }
            return background;
        }

        private Brush ExtractBackgroundFill(BitmapImage image, int posX, int posY, int width, int height)
        {
            Brush background = null;
            try
            {
                background = new ImageBrush(image)
                {
                    ViewboxUnits = BrushMappingMode.Absolute,
                    Viewbox = new Rect(posX, posY, width, height),
                    Stretch = Stretch.Fill
                };
            }
            catch (Exception ex)
            {
                Log.WriteLine(Log.LogLevels.Error, "Error while extracting background texture. Image {0}. Exception: {1}", image.BaseUri, ex);
            }
            return background;
        }
    }

    public class DebugViewModelDesignData : DebugViewModel
    {
        
    }
}
