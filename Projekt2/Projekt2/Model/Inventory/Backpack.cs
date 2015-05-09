using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Object class for the backpack
    /// </summary>
    class Backpack
    {
        //Properties
        public Vector2 Position { get; set; }
        public List<Item> BackpackItems { get; set; }
        public bool IsOpen { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Backpack()
        {
            this.IsOpen = false;
            this.BackpackItems = new List<Item>();
            this.Position = new Vector2(855.0f, 322.0f);
        }

        /// <summary>
        /// Method for updating items within the backpack
        /// </summary>
        public void UpdateBackpackItemPositions()
        {
            //TODO: Logic for updating items within the backpack
        }
    }
}
