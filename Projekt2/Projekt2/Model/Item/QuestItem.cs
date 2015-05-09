using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Object class for quest items
    /// </summary>
    class QuestItem : Item
    {
        public const int ENEMY_HEAD = 1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of quest item</param>
        public QuestItem(int type)
        {
            this.Type = type;
            this.WasLooted = false;
            this.CanAddToQuest = true;
            this.ThisItem = new MapObject();
            this.ThisItem.Bounds.Width = 48;
            this.ThisItem.Bounds.Height = 48;
        }
    }
}
