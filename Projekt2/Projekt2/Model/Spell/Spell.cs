using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Shared spell class.
    /// Inherited by spell object classes. 
    /// </summary>
    class Spell
    {
        //Shared properties
        public float FullCastTime {get; set; }
        public Vector2 Position { get; set; }
        public Unit Caster { get; set; }
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float Range { get; set; }
        public float ManaCost { get; set; }
        public float CastTime { get; set; }
    }
}
