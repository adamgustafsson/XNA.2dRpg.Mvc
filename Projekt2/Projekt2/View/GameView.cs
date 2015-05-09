using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace View
{
    /// <summary>
    /// Main view class for handling and rendering of graphics and sound
    /// </summary>
    class GameView
    {
        private View.AnimationSystem _animationSystem;
        private View.Dialog _conversation;
        private View.UIView _UIView;
        private View.InputHandler _inputHandler;
        private View.SoundHandler _soundHandler;
        private View.Camera _camera;
        private View.UnitView _unitView;
        private Model.GameModel _gameModel; 

        private string _zoneText = "";
        private float _zoneTextFader = 1;
        private bool _zoneTextWasDrawn = true;
        private bool _showDebug;
        private SpriteFont _spriteFont;
        private SpriteBatch _spriteBatch;
        private Texture2D _fireball;
        private Stopwatch _lootWatch = new Stopwatch();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphicsDevice">GraphicsDevice instance</param>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="gameModel">GameModel instance</param>
        /// <param name="animationSystem">AnimationSystem instance</param>
        /// <param name="inputHandler">InputHandler instance</param>
        /// <param name="soundHandler">SoundHandler instance</param> 
        public GameView(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Model.GameModel gameModel, View.AnimationSystem animationSystem, View.InputHandler inputHandler, View.SoundHandler soundHandler)
        {
            this._gameModel = gameModel;
            this._camera = new Camera(graphicsDevice, gameModel);
            this._spriteBatch = spriteBatch;
            this._soundHandler = soundHandler;
            this._inputHandler = inputHandler;
            this._animationSystem = animationSystem;
            this._conversation = new Dialog(spriteBatch, _gameModel, _camera, _inputHandler);
            this._UIView = new UIView(spriteBatch, _camera, _inputHandler, gameModel, _conversation);
            this._unitView = new UnitView(_gameModel, spriteBatch, _camera, _inputHandler, _animationSystem, _conversation);
        }

        /// <summary>
        /// Loading all view content
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        internal void LoadContent(ContentManager content)
        {
            _animationSystem.LoadContent(content);
            _UIView.LoadContent(content); 
            _conversation.LoadContent(content);
            _unitView.LoadContent(content);
            _fireball = content.Load<Texture2D>("Textures/Spells/fireball");
            _spriteFont = content.Load<SpriteFont>(@"Fonts\Nyala");
        }

        /// <summary>
        /// Main drawing method.
        /// Draws all in game content.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void DrawAndUpdate(float elapsedTime)
        {
            _inputHandler.MouseIsOverLoot = false;
            
            //Get player object
            Model.Player player = _gameModel.PlayerSystem.Player;
         
            //Updating player camera
            _camera.UpdateCamera();
            _spriteBatch.Begin();
            
            //Drawing background layers
            _gameModel.CurrentMap.DrawLayer(_spriteBatch, _gameModel.Level.IndexBackgroundLayerOne, _camera.GetScreenRectangle, 0f);
            _gameModel.CurrentMap.DrawLayer(_spriteBatch, _gameModel.Level.IndexBackgroundLayerTwo, _camera.GetScreenRectangle, 0f);
            _gameModel.CurrentMap.DrawLayer(_spriteBatch, _gameModel.Level.IndexBackgroundLayerThree, _camera.GetScreenRectangle, 0f);
            
            //Drawing items, units and spells
            DrawItems(elapsedTime, player);          
            _unitView.DrawAndUpdateUnits(elapsedTime);
            DrawSpells(_gameModel.PlayerSystem.SpellSystem.ActiveSpells, elapsedTime);
            DrawSpells(_gameModel.EnemySystem._enemySpellSystem.ActiveSpells, elapsedTime);

            //Drawing foreground layers
            if (_gameModel.Level.ForegroundVisible)
                _gameModel.CurrentMap.DrawLayer(_spriteBatch, _gameModel.Level.IndexForeground, _camera.GetScreenRectangle, 0f);

            //Drawing dialog boxes, user interface and zone texts
            DrawDialogBoxes(player);
            _UIView.DrawAndUpdate(elapsedTime);            
            DrawZoneText();

            #region DEBUG - Show objectlayers

            if (_inputHandler.PressedAndReleased(InputHandler.Input.DEBUG_MODE))
                _showDebug = !_showDebug;

            if (_showDebug)
            {
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexCollision, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexInteraction, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexFriendlyNPC, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexEnemyNPC, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexItems, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexPlayer, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexEnemyZone, _camera.GetScreenRectangle, 0f);
                _gameModel.CurrentMap.DrawObjectLayer(_spriteBatch, _gameModel.Level.IndexGraveyard, _camera.GetScreenRectangle, 0f);
            }

            #endregion     

            _spriteBatch.End();

            //Repeat the camera uodate (fix) 
            _camera.UpdateCamera();

            UpdateMusic();
        }

        /// <summary>
        /// Method for updating the game music
        /// </summary>
        private void UpdateMusic()
        {
            bool inTown = false;
            bool inDungeon = false;

            foreach (MapObject obj in _gameModel.Level.ZoneLayer.MapObjects)
            {
                if (obj.Name.ToUpper() == Model.Level.Zones.TOWN.ToString() && obj.Bounds.Intersects(_gameModel.PlayerSystem.Player.ThisUnit.Bounds))
                    inTown = true;
                else if (obj.Name.ToUpper() == Model.Level.Zones.DUNGEON.ToString() && obj.Bounds.Intersects(_gameModel.PlayerSystem.Player.ThisUnit.Bounds))
                    inDungeon = true;
            }

            if (inTown)
                _soundHandler.PlaySoundTrack(View.SoundHandler.Track.TOWN);
            else if (inDungeon)
                _soundHandler.PlaySoundTrack(View.SoundHandler.Track.DUNGEON);
            else
                _soundHandler.PlaySoundTrack(View.SoundHandler.Track.WORLD);

        }

        /// <summary>
        /// Method for updating the zone texts
        /// </summary>
        private void UpdateZoneText()
        {
            bool inZone = false;
            string nameProperty = "Name";

            foreach (MapObject obj in _gameModel.Level.ZoneLayer.MapObjects)
            {
                //If player is in zone
                if (_gameModel.PlayerSystem.Player.ThisUnit.Bounds.Intersects(obj.Bounds))
                {
                    inZone = true;
                    //If player reached a new zone
                    if (obj.Properties[nameProperty].Value != _zoneText)
                    {
                        _zoneTextFader = 1;
                        _zoneText = Convert.ToString(obj.Properties[nameProperty].Value);
                        _zoneTextWasDrawn = false;
                    }
                }
            }
            //If player have left the current zone
            if (!inZone && _zoneTextFader == 1)
                _zoneText = "";
        }

        /// <summary>
        /// Method for drawing dialog boxes
        /// </summary>
        /// <param name="player">Player object</param>
        private void DrawDialogBoxes(Model.Player player)
        {
            //Trigger the drawing dialog boxes
            if (player.Target != null)
            {
                if (player.Target.GetType() == Model.GameModel.FRIENDLY_NPC)
                {
                    Model.NonPlayerCharacter npc = player.Target as Model.NonPlayerCharacter;

                    if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(npc.ThisUnit.Bounds)) &&
                        npc.ThisUnit.Bounds.Intersects(player.CollisionArea) && npc.CanInterract)
                    {
                        _conversation.DrawDialog = true;
                    }
                    else if (!npc.ThisUnit.Bounds.Intersects(player.CollisionArea))
                    {
                        _conversation.DrawDialog = false;
                    }

                    bool isQuestNpc = false;

                    if (npc.UnitId == _gameModel.QuestSystem.ActiveNpc)
                        isQuestNpc = true;

                    if (_conversation.DrawDialog)
                        _conversation.DrawNPCText(player.Target, isQuestNpc);
                }
            }
        }

        /// <summary>
        /// Method for drawing zone texts
        /// </summary>
        private void DrawZoneText()
        {
            UpdateZoneText();

            //If the zone text wasn't already drawn
            if (!_zoneTextWasDrawn)
            {
                _zoneTextFader = _zoneTextFader - 0.005f;
                Color color = new Color(_zoneTextFader, _zoneTextFader, _zoneTextFader, _zoneTextFader);
                Vector2 stringSize = _spriteFont.MeasureString(_zoneText);
                _spriteBatch.DrawString(_spriteFont, _zoneText, new Vector2((_camera.GetScreenRectangle.Width / 2) - stringSize.X/2, _camera.GetScreenRectangle.Height / 3), color);

                if (_zoneTextFader <= 0)
                {
                    _zoneTextFader = 1;
                    _zoneTextWasDrawn = true;
                }
            }
        }

        /// <summary>
        /// Method for drawing of map Items as well as setting the players item targets
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        private void DrawItems(float elapsedTime, Model.Player player)
        {
            List<Model.Item> items = _gameModel.ItemSystem._items;

            foreach (Model.Item item in items)
            {
                if (_inputHandler.MouseIsOver(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                    _inputHandler.MouseIsOverLoot = true;

               //Checks if the player targeted an item
                if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                    player.ItemTarget = item;

                if (player.ItemTarget == item)
                {
                    if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(item.ThisItem.Bounds)) &&
                        player.ItemTarget.ThisItem.Bounds.Intersects(player.CollisionArea) && !_lootWatch.IsRunning)
                    {
                        item.WasLooted = true;
                    }
                    else
                        item.WasLooted = false;
                }
               
                if (item.GetType() == Model.GameModel.ARMOR)
                {
                    Model.Armor Armor = item as Model.Armor;

                    if (Armor.Type == Model.Armor.HEAD_ARMOR)
                    {
                        Vector2 position = _camera.VisualizeCordinates(Armor.ThisItem.Bounds.Location.X, Armor.ThisItem.Bounds.Location.Y);
                        Model.State itemAnimation = 0; 

                        itemAnimation = Model.State.FACING_CAMERA;

                        _animationSystem.UpdateAndDraw(elapsedTime, Color.White, position, itemAnimation, AnimationSystem.Texture.ITEM_PURPLE_CHEST);
                    }
                }
            }
        }

        /// <summary>
        /// Method for drawing of all active spells
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="activeSpells">Active spells from Model.SpellSystem</param>
        private void DrawSpells(List<Model.Spell> activeSpells, float elapsedTime)
        {
            foreach (Model.Spell spell in activeSpells)
            {
                //If spell is active and have been casted
                if(spell.Duration > 0 && spell.CastTime < 0)
                {
                    if (spell.GetType() == Model.SpellSystem.FIRE_BALL)
                    {
                        Vector2 fireballPos = _camera.VisualizeCordinates((int)spell.Position.X, (int)spell.Position.Y);
                        _animationSystem.UpdateAndDraw(elapsedTime, Color.White, fireballPos, Model.State.SMITE, AnimationSystem.Texture.FIREBALL_TEXTURE);
                    }
                }
                if (spell.GetType() == Model.SpellSystem.SMITE && spell.CoolDown > 1.7 && spell.Caster.Target != null)
                {
                    Vector2 smitePos = _camera.VisualizeCordinates(spell.Caster.Target.ThisUnit.Bounds.X, spell.Caster.Target.ThisUnit.Bounds.Y);
                    _animationSystem.UpdateAndDraw(elapsedTime, Color.White, smitePos, Model.State.SMITE, AnimationSystem.Texture.SMITE_TEXTURE);
                }
            }
        }

        /// <summary>
        /// Method for collecting all map objects visible to the camera
        /// </summary>
        /// <param name="currentMap">Current Map</param>
        private List<MapObject> GetObjectsInRegion(Map currentMap)
        {
            List<MapObject> mapObjects = new List<MapObject>();

            foreach (MapObject mapObject in currentMap.GetObjectsInRegion(_camera.GetScreenRectangle))
                mapObjects.Add(mapObject);

            return mapObjects;
        }

        #region InputHandling

        /// <summary>
        /// Checks wether a given key was pressed
        /// </summary>
        /// <param name="key">Keyboard key</param>
        /// <returns>True or false</returns>
        internal bool DidPressKey(InputHandler.Input input)
        {
            return _inputHandler.IsKeyDown(input);
        }

        /// <summary>
        /// Checks wether a given key was pressed and released
        /// </summary>
        /// <param name="key">Keyboard key</param>
        /// <returns>True or false</returns>
        internal bool DidPressAndReleaseKey(InputHandler.Input input)
        {
            return _inputHandler.PressedAndReleased(input);
        }

        /// <summary>
        /// Checks wether the mouse hover over any inteface graphic
        /// </summary>
        /// <returns>True or false</returns>
        internal bool MouseIsOverInterface()
        {
            return _inputHandler.MouseIsOverInterface;
        }

        /// <summary>
        /// Checks wether a given actionbar was pressed
        /// </summary>
        /// <param name="key">Actionbar</param>
        /// <returns>True or false</returns>
        internal bool DidActivateActionBar(InputHandler.Input input)
        {
            Rectangle actionBar = _UIView.GetActionBarArea(input);

            return (_inputHandler.DidGetTargetedByLeftClick(actionBar) || DidPressAndReleaseKey(input));
        }

        /// <summary>
        /// Checks the map position of the mouse.
        /// Requires that the mouse is not hovering any interface.
        /// </summary>
        /// <returns>Mouse logic coordinates</returns>
        internal Vector2 GetMapMousePosition()
        {
            if (_inputHandler.RightButtonIsDown())
            {
                if (!_inputHandler.MouseIsOverInterface)
                    return _camera.LogicalizeCordinates(_inputHandler.GetMouseState().X, _inputHandler.GetMouseState().Y);
            }

            return Vector2.Zero;
        }

        /// <summary>
        /// Checks if the player did un target its target
        /// </summary>
        /// <param name="player">Player object</param>
        /// <returns>True or false</returns>
        internal bool DidUnTarget(Model.Player player)
        {
            return (_inputHandler.DidLeftClick() && !_inputHandler.MouseIsOverObject && !_inputHandler.MouseIsOverInterface) && !_conversation.DrawDialog ||
                    !player.Target.ThisUnit.Bounds.Intersects(_camera.GetScreenRectangle);
        }     
        #endregion
    }
}
