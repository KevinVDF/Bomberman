using ClientWPF.Model;
using Common.DataContract;

namespace ClientWPF.ViewModels.StartedGame
{
    public class WallItem : LivingObjectItem
    {
        private Sprite _texture;
        public Sprite Textures
        {
            get { return _texture; }
            set { Set(() => Textures, ref _texture, value); }
        }

        public WallType WallType { get; set; }
    }
}
