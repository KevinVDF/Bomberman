using ClientWPF.Model;

namespace ClientWPF.ViewModels.StartedGame
{
    public class PlayerItem : LivingObjectItem
    {
        private Sprite _texture;
        public Sprite Textures
        {
            get { return _texture; }
            set { Set(() => Textures, ref _texture, value); }
        }
    }
}
