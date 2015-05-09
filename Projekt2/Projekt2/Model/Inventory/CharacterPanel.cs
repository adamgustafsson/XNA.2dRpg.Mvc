using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Objectclass for the character panel.
    /// </summary>
    class CharacterPanel
    {
        //Properties
        public Vector2 Position { get; set; }
        public List<Item> EquipedItems { get; set; }
        public bool IsOpen { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterPanel()
        {
            this.EquipedItems = new List<Item>();
            this.Position = new Vector2(250.0f, 150.0f);
            this.IsOpen = false;
        }

        /// <summary>
        /// Method for updating items on the character
        /// </summary>
        public void UpdateCharPanelItemPositions()
        {
            //TODO: Logic for updating items on the character
        }
    }
}
