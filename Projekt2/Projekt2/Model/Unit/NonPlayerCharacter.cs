using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Object class for friendly non-player characters
    /// </summary>
    class NonPlayerCharacter : Unit
    {
        //Constant types
        public const int OLD_MAN = 0;
        public const int CITY_GUARD = 1;
        public const int FEMALE_CITIZEN = 2;

        //Properties
        public bool CanInterract { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="thisUnit">Map object unit</param>
        /// <param name="NpcId">The id of the unit</param>
        /// <param name="canInterract">True if the npc should be interactable</param>
        public NonPlayerCharacter(MapObject thisUnit, int NpcId, bool canInterract)
        {
            this.UnitId = NpcId;
            this.ThisUnit = thisUnit;
            this.TotalHp = 10;
            this.CurrentHp = this.TotalHp;
            this.CanInterract = canInterract;
            this.ThisUnit.Bounds.Width = 64;
            this.ThisUnit.Bounds.Height = 64;

            if (thisUnit.Properties["Type"].AsInt32 == OLD_MAN)
            {
                this.Type = OLD_MAN;
                this.UnitState = State.FACING_CAMERA;
            }
            else if (thisUnit.Properties["Type"].AsInt32 == CITY_GUARD)
            {
                this.Type = CITY_GUARD;
                this.UnitState = State.FACING_LEFT;
            }
            else if (thisUnit.Properties["Type"].AsInt32 == FEMALE_CITIZEN)
                this.Type = FEMALE_CITIZEN;
        }
    }
}
