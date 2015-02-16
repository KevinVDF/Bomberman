using ClientWPF.Model;

namespace ClientWPF.ViewModels.StartedGame
{
    public class BombItem : LivingObjectItem
    {
        private Sprite _texture;
        public Sprite Textures
        {
            get { return _texture; }
            set { Set(() => Textures, ref _texture, value); }
        }

        public int PlayerId { get; set; }

        private int _power;
        public int Power
        {
            get { return _power; } 
            set { Set(() => Power, ref _power, value); }
        }
    }
}
