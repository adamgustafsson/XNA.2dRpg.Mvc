using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Class for loading all TMX maps from content and assigning their layers.
    /// </summary>
    class Level
    {
        /* AssignLayerIndex - This method must be called before AssignObjectLayers (Should be the first call upon loading the next map - IndexLevel++ )
         * AssignObjectLayers - Should be called second
         * LoadMaps - Loads all maps and adds them to the list
         * IndexLevel - The index within the map list that is currently running
         */

        //Public layer indexes
        public int IndexBackgroundLayerOne;
        public int IndexBackgroundLayerTwo;
        public int IndexBackgroundLayerThree;
        public int IndexForeground;
        public int IndexInteraction;
        public int IndexCollision;
        public int IndexFriendlyNPC;
        public int IndexEnemyNPC;
        public int IndexPlayer;
        public int IndexItems;
        public int IndexEnemyZone;
        public int IndexGraveyard;
        public int IndexZones;

        //Variables
        private List<Map> m_mapList;
          
        //Objectlayers
        private ObjectLayer _backgroundLayer;
        private ObjectLayer _foregroundLayer;
        private ObjectLayer _interactionLayer;
        private ObjectLayer _collisionLayer;
        private ObjectLayer _friendlyNPCLayer;
        private ObjectLayer _enemyNPCLayer;
        private ObjectLayer _playerLayer;
        private ObjectLayer _itemLayer;
        private ObjectLayer _enemyZoneLayer;
        private ObjectLayer _graveyardLayer;
        private ObjectLayer _zoneLayer;

        //Properties
        public bool ForegroundVisible { get; set; }
        public int IndexLevel { get; set; }

        //Mapzone-enum
        public enum Zones
        {
            TOWN,
            DUNGEON
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public Level(ContentManager content)
        {
            this.ForegroundVisible = true;
            this.m_mapList = new List<Map>();

            LoadMaps(content);
            AssignObjectLayerIndexes();
            AssignTileLayerIndexes();
            AssignObjectLayers();
        }

        /// <summary>
        /// Returns the current Map object
        /// </summary>
        /// <returns>Map object</returns>
        public Map CurrentMap()
        {
            return m_mapList[IndexLevel];
        }

        /// <summary>
        /// Loads all tmx maps from the content
        /// </summary>
        /// <param name="content">Contentmanager instance</param>
        public void LoadMaps(ContentManager content)
        {
            m_mapList.Add(content.Load<Map>("world"));
        }

        /// <summary>
        /// Collects the indexes from all available objectlayers within the current TMX file
        /// </summary>
        public void AssignObjectLayerIndexes()
        {
            for (int i = 0; i < m_mapList[IndexLevel].ObjectLayers.Count; i++)
            {
                switch (m_mapList[IndexLevel].ObjectLayers[i].Name)
                {
                    case "Interaction":
                        IndexInteraction = i;
                        break;
                    case "Collision":
                        IndexCollision = i;
                        break;
                    case "FriendlyNPC":
                        IndexFriendlyNPC = i;
                        break;
                    case "EnemyNPC":
                        IndexEnemyNPC = i;
                        break;
                    case "Player":
                        IndexPlayer = i;
                        break;
                    case "Items":
                        IndexItems = i;
                        break;
                    case "EnemyZone":
                        IndexEnemyZone = i;
                        break;
                    case "Graveyard":
                        IndexGraveyard = i;
                        break;
                    case "Zones":
                        IndexZones = i;
                        break;
                }
            }
        }

        /// <summary>
        /// Collects the indexes from all available tilelayers within the current TMX file
        /// </summary>
        public void AssignTileLayerIndexes()
        {
            for (int i = 0; i < m_mapList[IndexLevel].TileLayers.Count; i++)
            {
                switch (m_mapList[IndexLevel].TileLayers[i].Name)
                {
                    case "BackgroundLayer1":
                        IndexBackgroundLayerOne = i;
                        break;
                    case "BackgroundLayer2":
                        IndexBackgroundLayerTwo = i;
                        break;
                    case "BackgroundLayer3":
                        IndexBackgroundLayerThree = i;
                        break;
                    case "Foreground":
                        IndexForeground = i;
                        break;
                }
            }
        }

        /// <summary>
        /// Assigning all layers to variables
        /// </summary>
        public void AssignObjectLayers()
        {
            _backgroundLayer = m_mapList[IndexLevel].ObjectLayers[IndexBackgroundLayerOne];
            _foregroundLayer = m_mapList[IndexLevel].ObjectLayers[IndexForeground];
            _interactionLayer = m_mapList[IndexLevel].ObjectLayers[IndexInteraction];
            _collisionLayer = m_mapList[IndexLevel].ObjectLayers[IndexCollision];
            _friendlyNPCLayer = m_mapList[IndexLevel].ObjectLayers[IndexFriendlyNPC];
            _enemyNPCLayer = m_mapList[IndexLevel].ObjectLayers[IndexEnemyNPC];
            _playerLayer = m_mapList[IndexLevel].ObjectLayers[IndexPlayer];
            _itemLayer = m_mapList[IndexLevel].ObjectLayers[IndexItems];
            _enemyZoneLayer = m_mapList[IndexLevel].ObjectLayers[IndexEnemyZone];
            _graveyardLayer = m_mapList[IndexLevel].ObjectLayers[IndexGraveyard];
            _zoneLayer = m_mapList[IndexLevel].ObjectLayers[IndexZones];
        }

        //Readonly getters for objectlayers
        public ObjectLayer ZoneLayer { get { return _zoneLayer; } }
        public ObjectLayer EnemyZoneLayer { get { return _enemyZoneLayer; } }
        public ObjectLayer BackgroundLayer { get { return _backgroundLayer; } }
        public ObjectLayer ForegroundLayer { get { return _foregroundLayer; } }
        public ObjectLayer InteractionLayer { get { return _interactionLayer; } }
        public ObjectLayer CollisionLayer { get { return _collisionLayer; } }
        public ObjectLayer FriendlyNPCLayer { get { return _friendlyNPCLayer; } }
        public ObjectLayer EnemyNPCLayer { get { return _enemyNPCLayer; } }
        public ObjectLayer PlayerLayer { get { return _playerLayer; } }
        public ObjectLayer ItemLayer { get { return _itemLayer; } }
        public ObjectLayer GraveyardLayer { get { return _graveyardLayer; } }
    }
}
