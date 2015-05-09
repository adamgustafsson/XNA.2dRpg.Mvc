using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Shared class for Unit logic, inherited by all UnitSystems
    /// </summary>
    class UnitSystem
    {
        /// <summary>
        /// Method for checking if an unit reached a specific location
        /// </summary>
        /// <param name="player">Unit rectangle</param>
        /// <param name="moveTo">Move to location</param>
        /// <param name="accuracy"></param>
        /// <returns>True or false</returns>
        public bool ArrivedToPosition(Rectangle player, Vector2 moveTo, int accuracy)
        {
            Vector2 difference = new Vector2(player.Center.X - (int)moveTo.X, player.Center.Y - (int)moveTo.Y);
            if ((difference.X > -accuracy && difference.X < accuracy) && (difference.Y > -accuracy && difference.Y < accuracy))
                return true;
            return false;
        }

        /// <summary>
        /// Regenerates a given units mana
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="unit">Unit object</param>
        public void RegenerateMana(float elapsedTime, Unit unit)
        {
            if (unit.IsAlive())
            {
                unit.ManaRegen -= elapsedTime;

                //Checks if mana regeneration is nessesary
                if (unit.ManaRegen < 0 && unit.CurrentMana != unit.TotalMana)
                {
                    //TODO: Check type of unit if different mana regen is required
                    unit.CurrentMana += 1.5f;
                    
                    if (unit.CurrentMana > unit.TotalMana)
                        unit.CurrentMana = unit.TotalMana;

                    unit.ManaRegen = 1;
                } 
            }
        }

        /// <summary>
        /// Regenerates a given units HP
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="unit">Unit object</param>
        public void RegenerateHp(float elapsedTime, Unit unit)
        {
            if (!unit.IsAttacking && unit.IsAlive())
            {
                unit.HpRegen -= elapsedTime;
                if (unit.HpRegen < 0 && unit.CurrentHp != unit.TotalHp)
                {
                    //TODO: Check type of unit if different mana regen is required
                    //At this point only the player regenerates HP
                    unit.CurrentHp += 0.5f;
                    if (unit.CurrentHp > unit.TotalHp)
                        unit.CurrentHp = unit.TotalHp;

                    unit.HpRegen = 1;
                }
            }
        }

        /// <summary>
        /// Method for decreasing a units global cd.
        /// </summary>
        /// <param name="unit">Unit object</param>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        public void DecreaseGlobalCD(Unit unit, float elapsedTime)
        {
            //Reducing global cd
            if (unit.GlobalCooldown > 0)
            {
                unit.GlobalCooldown -= elapsedTime;
                if (unit.GlobalCooldown < 0)
                    unit.GlobalCooldown = 0;

            }
        }
    }
}
