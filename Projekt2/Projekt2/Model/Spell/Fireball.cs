using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Object class for the spell "Fireball"
    /// </summary>
    class Fireball : Spell
    {
        //Properties
        public Vector2 Direction { get; set; }
        public Unit Target { get; set; }
        public float Damage { get; set; }
        public Rectangle FireBallArea { get; set; }
        public bool WasCasted { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="caster">The unit object casting the spell</param>
        public Fireball(Unit caster)
        {   
            //8 is for moving the spell to the middle of the player when his animation size is 48x48 and the spell is 32x32; 48-32 = 16, 16/2 = 8
            Vector2 position = new Vector2(caster.ThisUnit.Bounds.X + 8, caster.ThisUnit.Bounds.Y + 8);
            this.ManaCost = 10;
            this.Damage = 8;
            this.Position = position;
            this.CastTime = 3f;
            this.FullCastTime = this.CastTime;
            this.Duration = 1;
            this.Range = 0;
            this.CoolDown = 5;
            this.Caster = caster;
            Update(caster.Target);
        }

        /// <summary>
        /// Udate method for the internal fireball logic
        /// </summary>
        /// <param name="target">Target of the casted spell</param>
        public void Update(Unit target)
        {
            FireBallArea = new Rectangle((int)this.Position.X, (int)this.Position.Y, 32, 32);
            Target = target;
        }
    }
}
