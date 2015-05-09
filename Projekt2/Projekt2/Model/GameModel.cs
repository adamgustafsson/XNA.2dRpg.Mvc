using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Model
{
    /// <summary>
    /// Main model class for updating the game engine.
    /// </summary>
    class GameModel
    {
        //Variables
        private Map _currentMap;
        private Level _level;
        private PlayerSystem _playerSystem;
        private EnemySystem _enemySystem;
        private NpcSystem _npcSystem;
        private ItemSystem _itemSystem;
        private QuestSystem _questSystem;

        //Type-references for model classes
        public static Type ENEMY_NPC = typeof(Model.Enemy);
        public static Type FRIENDLY_NPC = typeof(Model.NonPlayerCharacter);      
        public static Type ARMOR = typeof(Model.Armor);
        public static Type PLAYER = typeof(Model.Player);
        public static Type QUEST_ITEM = typeof(Model.QuestItem);
        public static Type QUEST_ITEM2 = typeof(Model.QuestItem);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public GameModel(ContentManager content)
        {
            this._level = new Level(content);
            this._currentMap = _level.CurrentMap();
            this._playerSystem = new PlayerSystem(_level);
            this._enemySystem = new EnemySystem(_level.EnemyNPCLayer, _level, _currentMap);
            this._npcSystem = new NpcSystem(_level.FriendlyNPCLayer);
            this._itemSystem = new ItemSystem(_level.ItemLayer);
            this._questSystem = new QuestSystem(content);
        }

        /// <summary>
        /// Main method for updating the game engine.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void Update(float elapsedTime)
        {
            _itemSystem.UpdateItemSystem(elapsedTime, _playerSystem.Player);

            _playerSystem.Update(elapsedTime, _enemySystem.Enemies);

            _enemySystem.Update(_playerSystem.Player, elapsedTime);

            _questSystem.UpdateActiveQuest(_enemySystem.SpawnList, _npcSystem.NonPlayerCharacters, _playerSystem.Player.BackPack.BackpackItems, _level);

            #region Interaction objectlayer-player

            if (PlayerEnters(_playerSystem.Player))
                _level.ForegroundVisible = false;
            else
                _level.ForegroundVisible = true;

            //Collision handling -player
            if (PlayerAndTileCollide())
            {
                _playerSystem.Player.ThisUnit.Bounds.X = _playerSystem.Player.LastPosition.X;
                _playerSystem.Player.ThisUnit.Bounds.Y = _playerSystem.Player.LastPosition.Y;
                _playerSystem.Player.MoveToPosition = new Vector2(_playerSystem.Player.ThisUnit.Bounds.Center.X, _playerSystem.Player.ThisUnit.Bounds.Center.Y);
                _playerSystem.Player.Direction = new Vector2(_playerSystem.Player.ThisUnit.Bounds.Center.X, _playerSystem.Player.ThisUnit.Bounds.Center.Y);

            }
            else
            {
                if (!(_playerSystem.Player.ThisUnit.Bounds.X == _playerSystem.Player.LastPosition.X
                    && _playerSystem.Player.ThisUnit.Bounds.Y == _playerSystem.Player.LastPosition.Y))
                {
                    _playerSystem.Player.CanMoveDown = true;
                    _playerSystem.Player.CanMoveLeft = true;
                    _playerSystem.Player.CanMoveRight = true;
                    _playerSystem.Player.CanMoveUp = true;
                }

                _playerSystem.Player.LastPosition = _playerSystem.Player.ThisUnit.Bounds.Location;
            } 

            #endregion
        }

        /// <summary>
        /// Collisionhandling - Player and Tiles
        /// </summary>
        /// <returns></returns>
        private bool PlayerAndTileCollide()
        {
            foreach (var obj in _currentMap.GetObjectsInRegion(_level.IndexCollision, _playerSystem.Player.CollisionArea))
            {
                if(obj.Name != "Open")
                {
                    if (obj.Bounds.Intersects(_playerSystem.Player.PlayerArea))
                    {
                        if (_playerSystem.Player.ThisUnit.Bounds.X > _playerSystem.Player.LastPosition.X)
                        {
                            _playerSystem.Player.CanMoveRight = false;
                            _playerSystem.Player.CanMoveLeft = true;
                            _playerSystem.Player.CanMoveDown = true;
                            _playerSystem.Player.CanMoveUp = true;
                        }
                        else if (_playerSystem.Player.ThisUnit.Bounds.X < _playerSystem.Player.LastPosition.X)
                        {
                            _playerSystem.Player.CanMoveRight = true;
                            _playerSystem.Player.CanMoveLeft = false;
                            _playerSystem.Player.CanMoveDown = true;
                            _playerSystem.Player.CanMoveUp = true;
                        }
                        else if (_playerSystem.Player.ThisUnit.Bounds.Y > _playerSystem.Player.LastPosition.Y)
                        {
                            _playerSystem.Player.CanMoveRight = true;
                            _playerSystem.Player.CanMoveLeft = true;
                            _playerSystem.Player.CanMoveDown = false;
                            _playerSystem.Player.CanMoveUp = true;
                        }
                        else if (_playerSystem.Player.ThisUnit.Bounds.Y < _playerSystem.Player.LastPosition.Y)
                        {
                            _playerSystem.Player.CanMoveRight = true;
                            _playerSystem.Player.CanMoveLeft = true;
                            _playerSystem.Player.CanMoveUp = false;
                            _playerSystem.Player.CanMoveDown = true;
                        }
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the player enters a building.
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>True or false</returns>
        private bool PlayerEnters(Player player)
        {
            foreach (var obj in _currentMap.GetObjectsInRegion(_level.IndexInteraction, _playerSystem.Player.CollisionArea))
            {
                if (obj.Bounds.Intersects(player.PlayerArea))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the game is completed
        /// </summary>
        /// <returns>True or false</returns>
        internal bool GameIsOver()
        {
            return _questSystem.AllQuestsCompleted;
        }

        //Readonly properties
        public Map CurrentMap { get { return _currentMap; } }
        public Level Level { get { return _level; } }
        public PlayerSystem PlayerSystem { get { return _playerSystem; } }
        public EnemySystem EnemySystem { get { return _enemySystem; } }
        public NpcSystem NpcSystem { get { return _npcSystem; } }
        public ItemSystem ItemSystem { get { return _itemSystem; } }
        public QuestSystem QuestSystem { get { return _questSystem; } }
    }
}
