using ClientWPF.Model;

namespace ClientWPF.ViewModels.StartedGame
{
    public class PlayerItem : LivingObjectItem
    {
        public Sprite Down { get; set; }

        public PlayerItem()
        {
            Down = new Sprite();
        }
    }
}
