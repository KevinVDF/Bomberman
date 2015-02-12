using ClientWPF.Model;

namespace ClientWPF.ViewModels.StartedGame
{
    public class BonusItem : LivingObjectItem
    {
        public int Id { get; set; }

        private Sprite _texture;
        public Sprite Textures
        {
            get { return _texture; }
            set { Set(() => Textures, ref _texture, value); }
        }

        public int? PlayerId { get; set; }
    }
}
