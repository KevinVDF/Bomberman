﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using ClientWPF.Logic;
using Common.DataContract;
using Common.Interfaces;

namespace ClientWPF.ViewModels.StartedGame
{
    public class MapViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<LivingObjectItem> _livingObjects; 
        public ObservableCollection<LivingObjectItem> LivingObjects
        {
            get { return _livingObjects; } 
            set { Set(() => LivingObjects, ref _livingObjects, value); }
        }

        private string _sentence;
        public string Sentence
        {
            get { return _sentence; }
            set { Set(() => Sentence, ref _sentence, value); }
        }

        private bool _endFlag;
        public bool EndFlag
        {
            get { return _endFlag; }
            set { Set(() => EndFlag, ref _endFlag, value); }
        }
        

        private List<LivingObjectItem> OrderList { get; set; }

        #endregion

        #region Methods

        public void Initialize()
        {
            EndFlag = false;
        }

        public void OnPlayerMove(Player player, Position newPosition, ActionType actionType)
        {

            PlayerItem objectToMove = LivingObjects.FirstOrDefault(x => x.X == (player.Position.X) && x.Y == player.Position.Y
                                                                    && x is PlayerItem && player.Id == ((PlayerItem)x).Id ) as PlayerItem;
            if (objectToMove != null)
            {
                objectToMove.X = newPosition.X;
                objectToMove.Y = newPosition.Y;

                switch (actionType)
                {
                    case ActionType.MoveDown:
                        objectToMove.ImageInUse = objectToMove.Textures.Down[1];
                        break;
                    case ActionType.MoveLeft:
                        objectToMove.ImageInUse = objectToMove.Textures.Left[1];
                        break;
                    case ActionType.MoveRight:
                        objectToMove.ImageInUse = objectToMove.Textures.Right[1];
                        break;
                    case ActionType.MoveUp:
                        objectToMove.ImageInUse = objectToMove.Textures.Up[1];
                        break;
                }
            }
        }

        public void OnBombDropped(Bomb newBomb)
        {
            BombItem bomb = MapToBombItem(newBomb) as BombItem;
            LivingObjects.Add(bomb);
        }

        public void OnBombExploded(Bomb bomb, List<LivingObject> impacted)
        {
            BombItem bombItem = LivingObjects.First(x => x is BombItem && ((BombItem) x).Id == bomb.Id) as BombItem;

            if (bombItem != null)
            {
                bombItem.ImageInUse = Textures.Textures.ExplodedBombItem.ImageInUse;
                bombItem.Height = 150;
                bombItem.Width = 150;
                Timer t = new Timer(BombExploded, bombItem, 500, Timeout.Infinite);

                foreach (LivingObject livingObject in impacted)
                {
                    if (livingObject is Wall)
                    {
                        LivingObjectItem wallToRemove = LivingObjects.First(x => x is WallItem && ((WallItem)x).Id == ((Wall)livingObject).Id);
                        LivingObjects.Remove(wallToRemove);
                    }
                        
                    if (livingObject is Player)
                    {
                        LivingObjectItem playerToRemove = LivingObjects.FirstOrDefault(x => x is PlayerItem && ((PlayerItem) x).Id == ((Player) livingObject).Id);
                        LivingObjects.Remove(playerToRemove);
                    }
                }
            }
        }

        private void BombExploded(object bomb)
        {
            ExecuteOnUIThread(() => LivingObjects.Remove(bomb as BombItem));
        }

        private LivingObjectItem MapToBombItem(Bomb bomb)
        {
            BombItem bombitem = new BombItem
            {
                Id = bomb.Id,
                X = bomb.Position.X,
                Y = bomb.Position.Y,
                PlayerId = bomb.PlayerId,
                Power = bomb.Power,
                ImageInUse = Textures.Textures.BombItem.ImageInUse,
                ZIndex = 1,
                Width = 20,
                Height = 25
            };
            return bombitem;
        }

        #endregion
    }

    public class MapViewModelDesignData : MapViewModel
    {
        
    }
}
