using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ClientWPF.ViewModels.StartedGame
{
    public class Sprite
    {
        public List<Brush> Images { get; set; }

        public Sprite()
        {
            Images = new List<Brush>();
        }
    }
}
