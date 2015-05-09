using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Object class for Armor items
    /// </summary>
    class Armor : Item
    {
        //Constants
        public const int HEAD_ARMOR = 1;
        public const int CHEST_ARMOR = 2;

        //Properties
        public int ArmorValue { get; set; }
        public int MagicResistValue { get; set; }
        public int HealthValue { get; set; }
        public int ManaValue { get; set; }

        /// <summary>
        /// Constructor used for items loaded directly from the TMX map
        /// </summary>
        /// <param name="armor">Item map object</param>
        /// <param name="id">Item map object id</param>
        public Armor(MapObject armor, int id)
        {
            this.ThisItem = armor;
            this.ArmorValue = Convert.ToInt32(armor.Properties["Armor"].AsInt32);
            this.Type = Convert.ToInt32(armor.Properties["Type"].AsInt32);
            this.ThisItem.Bounds.Width = 48;
            this.ThisItem.Bounds.Height = 48;
            this.CanAddToQuest = true;
            this.WasLooted = false;
            this.ItemId = id;
        }

        /// <summary>
        /// Constructor used for items created in C#
        /// </summary>
        /// <param name="armorValue"></param>
        /// <param name="type">Type of armor</param>
        public Armor(int armorValue, int type)
        {
            this.ArmorValue = armorValue;
            this.Type = type;
            this.CanAddToQuest = true;
            this.WasLooted = false;
            this.ThisItem = new MapObject();
            this.ThisItem.Bounds.Width = 48;
            this.ThisItem.Bounds.Height = 48;
        }
    }
}
