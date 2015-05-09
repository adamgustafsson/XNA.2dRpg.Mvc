using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Class for updating the item logic
    /// </summary>
    class ItemSystem
    {
        public List<Item> _items;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemLayer">Item object layer from the TMX map</param>
        public ItemSystem(ObjectLayer itemLayer)
        {
            this._items = new List<Item>();
            LoadItems(itemLayer);
        }

        /// <summary>
        /// Loads all item objects from the TMX map
        /// </summary>
        /// <param name="itemLayer">Item object layer from the TMX map</param>
        public void LoadItems(ObjectLayer itemLayer)
        {
            int itemID = 0;
            foreach (MapObject item in itemLayer.MapObjects)
            {
                itemID++;
                _items.Add(new Armor(item, itemID));
            }
        }

        /// <summary>
        /// Main method for updating the item logic
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="player">Player object</param>
        internal void UpdateItemSystem(float elapsedTime, Player player)
        {
            if (player.IsAlive())
            {
                //Checks if the player targets an item. Puts it in the backpack.
                if (player.ItemTarget != null && player.ItemTarget.WasLooted)
                    AddToBackPack(player, player.ItemTarget);
                //Cheks if the player targets an item in the backpack. Equip/use it.
                if (player.BackpackTarget != null)
                    EquipOrUse(player);
                //Checks if the player targets an equipped item. Puts it back in the backpack.
                if (player.CharPanelTarget != null)
                    UnEquip(player, player.CharPanelTarget);
            }
        }

        /// <summary>
        /// Adding items to the backpack.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="item">Item object</param>
        public void AddToBackPack(Player player, Item item)
        {
            if(item.GetType() == Model.GameModel.ARMOR)
            {
                player.BackPack.BackpackItems.Add(item as Model.Armor);
                _items.Remove(item);
            }
            else if (item.GetType() == Model.GameModel.QUEST_ITEM)
                player.BackPack.BackpackItems.Add(item as Model.QuestItem);
            
            player.ItemTarget = null;
        }

        /// <summary>
        /// Removing items from the backpack.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="item">Item object</param>
        public void RemoveFromBackpack(Player player, Item item)
        {
            player.BackPack.BackpackItems.Remove(item);
        }

        /// <summary>
        /// Method for handling the use of items
        /// </summary>
        /// <param name="player">Player object</param>
        public void EquipOrUse(Player player)
        { 
            if(player.BackpackTarget.GetType() == Model.GameModel.ARMOR)
                EquipArmor(player, player.BackpackTarget as Model.Armor);
        }

        /// <summary>
        /// Method for equipping armor.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="armor">Armor object</param>
        public void EquipArmor(Player player, Armor armor)
        {
            if (player.BackpackTarget.GetType() == Model.GameModel.ARMOR)
            {
                //Controls that the current slot is available
                if (!PlayerHasArmor(player, player.BackpackTarget as Armor))
                {
                    //Increase player armor in accordance to the item, removes the item from the backpack and sets backpack target to null
                    player.Armor += armor.ArmorValue;
                    RemoveFromBackpack(player, armor);
                    player.BackpackTarget = null;
                    player.CharPanel.EquipedItems.Add(armor as Armor);
                } 
            }
        }

        /// <summary>
        /// Method for un-equipping items
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="charPanelTarget">Item object that is being un-equipped</param>
        public void UnEquip(Player player, Item charPanelTarget)
        {
            player.CharPanel.EquipedItems.Remove(charPanelTarget);
            player.BackPack.BackpackItems.Add(charPanelTarget);

            if(charPanelTarget.GetType() == Model.GameModel.ARMOR)
            {
                Armor armor = charPanelTarget as Armor;
                player.Armor -= armor.ArmorValue;

                if(armor.Type == Armor.HEAD_ARMOR)
                    player.HasHelm = false;
            }

            player.CharPanelTarget = null;
        }

        /// <summary>
        /// Checks wheter a given armor piece is already equipped
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="armor">Armor object</param>
        /// <returns>True/false</returns>
        public bool PlayerHasArmor(Player player, Armor armor)
        {
            switch(armor.Type)
            {
                case Model.Armor.HEAD_ARMOR:
                    if (!player.HasHelm)
                    {
                        player.HasHelm = true;
                        return false;
                    }
                    return true;

                default:
                    return true;
            }
        }
    }
}
