using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Object class for the spell "Smite"
    /// </summary>
    class Smite : Spell
    {
        //Properties
        public Unit Target { get; set; }
        public int Damage { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caster">The unit object casting the spell</param>
        public Smite(Unit caster)
        {
            this.Damage = 15;
            this.ManaCost = 10;
            this.CastTime = 0;
            this.FullCastTime = this.CastTime;
            this.Duration = 1;
            this.Range = 0;
            this.CoolDown = 2;
            this.Caster = caster;
            this.Target = this.Caster.Target;
        }
    }
}
