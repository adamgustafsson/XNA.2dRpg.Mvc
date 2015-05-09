using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Object class for the Player
    /// </summary>
    class Player : Unit
    {
        //Cariables
        private Level _level;
        private Random _crittChance;

        //Constants
        public const int TEMPLAR = 0;
        public const int PROPHET = 1;
        public const int DESCENDANT = 2;

        //Properties
        public CharacterPanel CharPanel { get; set; }
        public Item BackpackTarget { get; set; }
        public Item ItemTarget { get; set; }
        public Item CharPanelTarget { get; set; }
        public bool HasHelm { get; set; }
        public Enemy LootTarget { get; set; }

        public bool CanMoveDown { get; set; }
        public bool CanMoveUp { get; set; }
        public bool CanMoveLeft { get; set; }
        public bool CanMoveRight { get; set; }

        public Rectangle PlayerArea { get; set; }
        public Rectangle CollisionArea { get; set; }
        public Rectangle MaxRangeArea { get; set; }

        public Point LastPosition { get; set; }
        public bool IsLookingAtMap { get; set; }
        public bool IsWithinMeleRange { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">Active Level object</param>
        public Player(Level level)
        {
            this._crittChance = new Random();
            this._level = level;

            this.ThisUnit = level.PlayerLayer.MapObjects[0];
            this.ThisUnit.Bounds.Width = 48;
            this.ThisUnit.Bounds.Height = 48;
            this.Type = TEMPLAR;
            this.SpawnTimer = 2f;

            this.IsLookingAtMap = false;
            this.HasHelm = false;
            this.CanMoveRight = true;
            this.CanMoveLeft = true;
            this.CanMoveUp = true;
            this.CanMoveDown = true;

            this.TotalHp = 100;
            this.CurrentHp = this.TotalHp;
            this.TotalMana = 50;
            this.CurrentMana = this.TotalMana;
            this.Armor = 0;
            this.Resist = 0;

            this.CharPanel = new CharacterPanel();
            this.GlobalCooldown = 0;
            this.AutohitDamage = 10;

            Update();
        }

        /// <summary>
        /// Updates the player collisionarea, range and calculates crittchance
        /// </summary>
        public void Update()
        {
            PlayerArea = new Rectangle(this.ThisUnit.Bounds.X, this.ThisUnit.Bounds.Y, this.ThisUnit.Bounds.Width, this.ThisUnit.Bounds.Height);
            CollisionArea = new Rectangle(this.ThisUnit.Bounds.X - 30, this.ThisUnit.Bounds.Y - 30, this.ThisUnit.Bounds.Width + 60, this.ThisUnit.Bounds.Height + 60);
            MaxRangeArea = new Rectangle(this.ThisUnit.Bounds.X - 150, this.ThisUnit.Bounds.Y - 150, this.ThisUnit.Bounds.Width + 300, this.ThisUnit.Bounds.Height + 300);

            if (_crittChance.Next(1, 10) == 1 && this.SwingTime == 50)
                this.AutohitDamage += _crittChance.Next(2, 7);
            else if (this.AutohitDamage != 10 && this.SwingTime == 50)
                this.AutohitDamage = 10;
        }

        /// <summary>
        /// Respawns the player
        /// </summary>
        public void Spawn()
        {
            Point playerPos = ThisUnit.Bounds.Location;
            Point nearestGraveyard = new Point();
            int nearestDiffernce = 0;

            for (int i = 0; i < _level.GraveyardLayer.MapObjects.Count(); i++)
            {
                double p1 = Math.Pow((playerPos.X - _level.GraveyardLayer.MapObjects[i].Bounds.Location.X), 2);
                double p2 = Math.Pow((playerPos.Y - _level.GraveyardLayer.MapObjects[i].Bounds.Location.Y), 2);
                double r = p1 + p2;
                int differnce = (int)Math.Sqrt(r);

                if (nearestGraveyard == new Point())
                {
                    nearestGraveyard = _level.GraveyardLayer.MapObjects[i].Bounds.Location;
                    nearestDiffernce = differnce;
                }
                else if (differnce < nearestDiffernce)
                {
                    nearestGraveyard = _level.GraveyardLayer.MapObjects[i].Bounds.Location;
                    nearestDiffernce = differnce;
                }
            }

            this.ThisUnit.Bounds.Location = nearestGraveyard;
            this.CurrentHp = this.TotalHp;
            this.IsCastingSpell = false;
            this.SpawnTimer = 2;
            //Resets "move to location"
            this.MoveToPosition = Vector2.Zero;
            this.Direction = Vector2.Zero;
        }
    }
}
