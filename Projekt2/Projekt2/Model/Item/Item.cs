using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Shared item class.
    /// Inherited by item object classes. 
    /// </summary>
    class Item
    {
        //Properties
        public int ItemId { get; set; }
        public bool WasLooted { get; set; }
        public bool CanAddToQuest { get; set; }
        public float SpawnCounter { get; set; }
        public float SpawnTime { get; set; }
        public int Type { get; set; }
        public MapObject ThisItem { get; set; }
    }
}
