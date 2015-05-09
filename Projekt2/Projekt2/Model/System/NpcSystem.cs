using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;

namespace Model
{
    /// <summary>
    /// Class for updating the non-player character logic
    /// </summary>
    class NpcSystem
    {
        private List<Model.NonPlayerCharacter> _nonPlayerCharacters;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="npcLayer">The tmx object layer for npc characters</param>
        public NpcSystem(ObjectLayer npcLayer)
        {
            this._nonPlayerCharacters = new List<Model.NonPlayerCharacter>();
            LoadFriends(npcLayer);
        }

        /// <summary>
        /// Loads npc objects from the tmx map
        /// </summary>
        /// <param name="npcLayer"></param>
        private void LoadFriends(ObjectLayer npcLayer)
        {
            foreach (MapObject friend in npcLayer.MapObjects)
                _nonPlayerCharacters.Add(new NonPlayerCharacter(friend, Convert.ToInt32(friend.Properties["ID"].AsInt32), Convert.ToBoolean(friend.Properties["CanInterract"].AsBoolean)));
        }

        public List<Model.NonPlayerCharacter> NonPlayerCharacters { get { return _nonPlayerCharacters; } }
    }
}
