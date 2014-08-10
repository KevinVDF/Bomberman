using System.Collections.Generic;
using System.Windows.Media;

namespace ClientWPF.Model
{
    public class Sprite
    {
        public List<Brush> Down { get; set; }

        public List<Brush> Left { get; set; }

        public List<Brush> Right { get; set; }

        public List<Brush> Up { get; set; }

        public Sprite()
        {
            Down = new List<Brush>();
            Left = new List<Brush>();
            Right = new List<Brush>();
            Up = new List<Brush>();
        }
    }
}
