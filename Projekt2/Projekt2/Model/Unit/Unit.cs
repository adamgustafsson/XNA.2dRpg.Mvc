using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Shared unit class.
    /// Inherited by unit object classes. 
    /// </summary>
    class Unit
    {
        //Properties
        public Unit Target { get; set; }
        public MapObject ThisUnit { get; set; }
        public int Type { get; set; }
        public float SpawnTimer { get; set; }
        public float MoveSpeed { get; set; }
        public bool IsCastingSpell { get; set; }
        public bool InCombat { get; set; }
        public bool CanAddToQuest { get; set; }
        public Backpack BackPack { get; set; }
        public State WeaponState { get; set; }
        public State UnitState { get; set; }
        public Vector2 Direction { get; set; }
        public Vector2 MoveToPosition { get; set; }

        //Properties - Attack
        public bool IsAttacking { get; set; }
        public int AutohitDamage { get; set; }
        public float SwingTime { get; set; }
        public float GlobalCooldown { get; set; }

        //Properties - Stats
        public float Armor { get; set; }
        public float Resist { get; set; }
        public float SpellPower { get; set; }
        public int UnitId { get; set; }
        public float TotalHp { get; set; }
        public float CurrentHp { get; set; }
        public float CurrentMana { get; set; }
        public float TotalMana { get; set; }
        public float ManaRegen { get; set; }
        public float HpRegen { get; set; }
             
        /// <summary>
        /// Constructor
        /// </summary>
        public Unit()
        {
            this.BackPack = new Backpack();
            this.IsCastingSpell = false;
            this.ManaRegen = 1;
            this.HpRegen = 1;
        }
        
        /// <summary>
        /// Checks whether the player is alive.
        /// </summary>
        /// <returns>False if the player hp is less or equal to zero</returns>
        internal bool IsAlive()
        {
            if (CurrentHp <= 0)
                return false;

            return true;
        }
    }
}
