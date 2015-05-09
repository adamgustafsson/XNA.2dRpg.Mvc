using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Object class for Enemies
    /// </summary>
    class Enemy : Unit
    {
        //Constants
        public const int CLASS_WARRIOR = 0;
        public const int CLASS_ARCHER = 1;
        public const int CLASS_MAGE = 2;
        public const int CLASS_GOBLIN = 3;
        public const int BOSS_A = 4;

        //Properties
        public Point SpawnPosition { get; set; }
        public Vector2 AggroRange { get; set; }
        public int TargetDisLocationX { get; set; }
        public int TargetDisLocationY { get; set; }
        public bool IsActive { get; set; }
        public bool IsEvading { get; set; }
        public Rectangle EnemyZone { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="thisUnit">The enemy map object</param>
        /// <param name="type">The enemy type</param>
        /// <param name="enemyId">The enemy ID</param>
        /// <param name="enemyZone">The enemy zone (mapzone)</param>
        public Enemy(MapObject thisUnit, int type, int enemyId, Rectangle enemyZone)
        {
            this.GlobalCooldown = 0.5f;
            this.EnemyZone = enemyZone;
            this.UnitId = enemyId;
            this.Type = type;
            this.SpawnPosition = thisUnit.Bounds.Location;
            this.ThisUnit = thisUnit;
            this.ThisUnit.Bounds.Width = 64;
            this.ThisUnit.Bounds.Height = 64;
            this.CanAddToQuest = true;

            if(thisUnit.Properties["Type"].AsInt32 == CLASS_WARRIOR)
            {
                this.TotalHp = 100;
                this.AutohitDamage = 3;
                this.MoveSpeed = 2.0f;
            }
            if (thisUnit.Properties["Type"].AsInt32 == CLASS_GOBLIN)
            {
                this.TotalHp = 85;
                this.AutohitDamage = 2;
                this.MoveSpeed = 3.0f;
            }
            if (thisUnit.Properties["Type"].AsInt32 == CLASS_MAGE)
            {
                this.TotalHp = 75;
                this.TotalMana = 20;
                this.CurrentMana = this.TotalMana;
                this.AutohitDamage = 1;
                this.MoveSpeed = 2.0f;
            }
            if (thisUnit.Properties["Type"].AsInt32 == BOSS_A)
            {
                this.TotalHp = 125;
                this.TotalMana = 50;
                this.SpellPower = 5;
                //this.Armor = 5;
                this.CurrentMana = this.TotalMana;
                this.AutohitDamage = 5;
                this.MoveSpeed = 2.0f;
                Model.QuestItem questItem = new Model.QuestItem(QuestItem.ENEMY_HEAD);
                this.BackPack.BackpackItems.Add(questItem);
            }

            this.CurrentHp = this.TotalHp;
        }

        /// <summary>
        /// Checking if an enemy is currently waiting to respawn
        /// </summary>
        /// <returns>True or false</returns>
        public bool WaitingToSpawn()
        {
            if (SpawnTimer > 0)
                return true;
            return false;
        }
    }
}
