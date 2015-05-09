using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Object class for the spell "InstantHeal"
    /// </summary>
    class InstantHeal : Spell
    {
        //Properties
        public float Heal { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caster">The unit object casting the spell</param>
        public InstantHeal(Unit caster)
        {
            this.ManaCost = 5;
            this.CastTime = 1f;
            this.FullCastTime = this.CastTime;
            this.Duration = 1;
            this.Range = 0;
            this.CoolDown = 5;
            this.Caster = caster;
            //Heals 1/4 of the total health
            this.Heal = caster.TotalHp / 4;
        }
    }
}
