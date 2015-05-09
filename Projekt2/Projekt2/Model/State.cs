using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Enum containing all Unit/Object states
    /// </summary>
    public enum State
    {
        FACING_CAMERA = 1,
        FACING_LEFT = 2,
        FACING_RIGHT = 3,
        FACING_AWAY = 4,
        MOVING_LEFT = 5,
        MOVING_RIGHT = 6,
        MOVING_UP = 7,
        MOVING_DOWN = 8,
        IS_DEAD = 9,
        WAS_HEALED = 10,
        IS_CASTING_FIREBALL = 11,
        IS_CASTING_HEAL = 12,
        VERTICAL_ANIMATION = 13,
        SMITE = 14,
        FIREBALL = 15
    };
}
