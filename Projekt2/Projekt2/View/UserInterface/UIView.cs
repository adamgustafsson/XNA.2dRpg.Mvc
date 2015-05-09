using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FuncWorks.XNA.XTiled;
using System.Diagnostics;

namespace View
{
    /// <summary>
    /// Main class for rendering of user interface
    /// </summary>
    class UIView
    {
        private Texture2D[] _textures;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFontSegoeSmall;
        private SpriteFont _healFont;
        private Stopwatch _rightClickWatch;
        private Vector2 _moveToPos;
        private List<Model.Spell> _activeSpells;
        private List<Model.Item> _worldItems;
        private Map _currentMap;

        private float _dmgCounter;
        private float _healingCounter;
        private float _smiteCounter;
        private float _randomNr = 0;
        private float _moveToColor;

        private View.Camera _camera;
        private View.InputHandler _inputHandler;
        private View.Dialog _dialog;
        private Model.QuestSystem _questSystem;
        private Model.Player _player;

        private View.UI.Avatar _avatar;
        private View.UI.ActionBar _actionBar;
        private View.UI.WorldMap _worldMap;
        private View.UI.Common _common;

        /// <summary>
        /// Texture index enum
        /// </summary>
        private enum Texture
        { 
            BACKPACK = 0,
            HEADARMOR = 1,
            CHARACTER_PANEL = 2,
            BAR_BACKGROUND = 3,
            LOOT_BOX = 4,
            CURSOR_NORMAL = 5,
            CURSOR_HOVER_ENEMY = 6,
            CURSOR_SELECT = 7,
            BOSS_HEAD = 8,
            MOVE_TO_CROSS = 9,
            CHAR_PANEL_BG = 10,
            QLOG = 11
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">Instance of SpriteBatch</param>
        /// <param name="camera">Instance of Camera</param>
        /// <param name="inputHandler">Instance of InputHandler</param>
        /// <param name="gameModel">Instance of GameModel</param>
        /// <param name="dialog">Instance of Dialog</param>
        public UIView(SpriteBatch spriteBatch, Camera camera, InputHandler inputHandler, Model.GameModel gameModel, Dialog dialog)
        {
            this._spriteBatch = spriteBatch;
            this._player = gameModel.PlayerSystem.Player;
            this._worldItems = gameModel.ItemSystem._items;
            this._currentMap = gameModel.Level.CurrentMap();
            this._activeSpells = gameModel.PlayerSystem.SpellSystem.ActiveSpells;
            this._rightClickWatch = new Stopwatch();

            this._dialog = dialog;
            this._camera = camera;
            this._inputHandler = inputHandler;
            this._questSystem = gameModel.QuestSystem;

            this._common = new UI.Common(_spriteBatch);
            this._avatar = new UI.Avatar(_spriteBatch, _player);
            this._worldMap = new UI.WorldMap(_spriteBatch, _player, _inputHandler, _camera);
            this._actionBar = new UI.ActionBar(_spriteBatch, _activeSpells, _player, _questSystem, _camera);
        }

        /// <summary>
        /// Loading all content within the UI namespace
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        internal void LoadContent(ContentManager content)
        {
            //Namespace classes
            _common.LoadContent(content);
            _avatar.LoadContent(content);
            _actionBar.LoadContent(content);
            _worldMap.LoadContent(content);

            //Textures
            _textures = new Texture2D[12] {     content.Load<Texture2D>("Textures/Interface/backpack"),
                                                content.Load<Texture2D>("Textures/Interface/headArmor2"),
                                                content.Load<Texture2D>("Textures/Interface/characterpanel"),
                                                content.Load<Texture2D>("Textures/Interface/barbackground"),
                                                content.Load<Texture2D>("Textures/Interface/lootbox"),
                                                content.Load<Texture2D>("Textures/Interface/cursor"),
                                                content.Load<Texture2D>("Textures/Interface/cursorAttack"),
                                                content.Load<Texture2D>("Textures/Interface/cursorSelect"),
                                                content.Load<Texture2D>("Textures/Items/QuestItems/bossHead"),
                                                content.Load<Texture2D>("Textures/Interface/moveToCross"),
                                                content.Load<Texture2D>("Textures/Interface/charPanel"),
                                                content.Load<Texture2D>("Textures/Interface/qlog")};
            //Fonts
            _spriteFontSegoeSmall = content.Load<SpriteFont>(@"Fonts\SegoeSmall");
            _healFont = content.Load<SpriteFont>(@"Fonts\HealFont");
        }

        /// <summary>
        /// Main method for updating and rendering of the game UI
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void DrawAndUpdate(float elapsedTime)
        {
            _inputHandler.MouseIsOverInterface = false;

            _avatar.DrawTargetAvatars();

            _actionBar.DrawActionBar();

            DrawBackpack();

            DrawDamageAndHealing(elapsedTime);

            DrawCharacterPanel();

            _worldMap.DrawWorldMap();

            if (_questSystem.IsWatchingQuestLog)
                DrawQuestLog();
            if(_player.IsCastingSpell)
                DrawCastBar();
            if (_player.LootTarget != null)
                DrawLootBox(_player.LootTarget);

            //Checks if the cursor is hovering the actionbars
            foreach (Vector2 actionBarPos in _actionBar.BarPositions)
            {
                if (_inputHandler.MouseIsOver(new Rectangle((int)actionBarPos.X, (int)actionBarPos.Y, 48, 48)))
                    _inputHandler.MouseIsOverInterface = true;
            }

            DrawMoveToCross();

            DrawMouse();
        }

        /// <summary>
        /// Method for drawing the move-to cross
        /// </summary>
        private void DrawMoveToCross()
        {
            if (_inputHandler.DidRightClick() && !_inputHandler.MouseIsOverInterface)
            {
                _rightClickWatch.Reset();
                _rightClickWatch.Start();
                _moveToPos = _player.MoveToPosition;
                _moveToColor = 1f;
            }
            if (_rightClickWatch.IsRunning && _rightClickWatch.ElapsedMilliseconds < 1000)
            {
                _moveToColor = _moveToColor - 0.015f;
                Color color = new Color(_moveToColor, _moveToColor, _moveToColor, _moveToColor);
                Vector2 pos = _camera.VisualizeCordinates((int)_moveToPos.X - 16, (int)_moveToPos.Y);
                _spriteBatch.Draw(GetTexture(Texture.MOVE_TO_CROSS), pos, color);
            }
            else
            {
                _rightClickWatch.Stop();
                _rightClickWatch.Reset();
            }
        }

        /// <summary>
        /// Method for drawing the mouse cursor
        /// </summary>
        private void DrawMouse()
        {
            Vector2 mousePosition = new Vector2(_inputHandler.GetMouseState().X, _inputHandler.GetMouseState().Y);
            Color mouseColor = Color.White;

            if (_inputHandler.MouseIsOverLoot && !_inputHandler.MouseIsOverEnemy)
                mouseColor = Color.LightGreen;
            if (_inputHandler.MouseIsOverEnemy)
                _spriteBatch.Draw(GetTexture(Texture.CURSOR_HOVER_ENEMY), mousePosition, mouseColor);
            else if (_inputHandler.LeftButtonIsDown())
                _spriteBatch.Draw(GetTexture(Texture.CURSOR_SELECT), mousePosition, mouseColor);
            else
                _spriteBatch.Draw(GetTexture(Texture.CURSOR_NORMAL), mousePosition, mouseColor);
        }

        /// <summary>
        /// Method for drawing the QuestLog
        /// </summary>
        private void DrawQuestLog()
        {
            List<Reader.Objective> progress = _questSystem.ObjectiveList;
            List<Reader.Objective> quest = _questSystem.CurrentQuest.Objectives;
            Vector2 position = new Vector2(405, 150);
            Rectangle textRect = _camera.VisualizeRectangle(new Rectangle((int)position.X + 8, (int)position.Y + 53, 225, 350));
            Rectangle closeCross = GetCloseButton(position.X, position.Y, Texture.QLOG);

            _spriteBatch.Draw(GetTexture(Texture.QLOG), position, Color.White);
            
            if (_questSystem.CurrentQuest.Status != Model.QuestSystem.PRE)
            {
                _spriteBatch.DrawString(_spriteFontSegoeSmall, _dialog.GetLogMessage(textRect), _camera.LogicalizeCordinates(textRect.X, textRect.Y), Color.White);

                int changeRow = 150;
                for (int i = 0; i < progress.Count; i++)
                {
                    _spriteBatch.DrawString(_spriteFontSegoeSmall, progress[i].Amount + "/" + quest[i].Amount + " - " + quest[i].Name, _camera.LogicalizeCordinates(textRect.X, textRect.Y + changeRow), Color.White);
                    changeRow += 18;
                }
            }

            if (_inputHandler.MouseIsOver(new Rectangle((int)position.X, (int)position.Y, GetTexture(Texture.QLOG).Bounds.Width, GetTexture(Texture.QLOG).Bounds.Height)))
                _inputHandler.MouseIsOverInterface = true;
            if (_inputHandler.DidGetTargetedByLeftClick(closeCross))
                _questSystem.IsWatchingQuestLog = false;
        }

        /// <summary>
        /// Method for drawing the lootbox
        /// </summary>
        /// <param name="unit">Unit object</param>
        private void DrawLootBox(Model.Unit unit)
        {
            //Displacement for the lootbox texture
            int displacementX = 10;
            int displacementY = 20;

            //TODO: Code duplication, also used for backpack
            Vector2 unitPos = _camera.VisualizeCordinates(unit.ThisUnit.Bounds.X, unit.ThisUnit.Bounds.Y);
            Rectangle itemRect = new Rectangle(0, 0, 32, 32);
            Rectangle lootBoxOuter = new Rectangle((int)unitPos.X, (int)unitPos.Y + 100,  GetTexture(Texture.LOOT_BOX).Width, GetTexture(Texture.LOOT_BOX).Height);
            Rectangle lootBoxInner = new Rectangle(lootBoxOuter.X + displacementX, lootBoxOuter.Y + displacementY, GetTexture(Texture.LOOT_BOX).Width - displacementX, GetTexture(Texture.LOOT_BOX).Height - displacementY);
            Rectangle closeCross = GetCloseButton(lootBoxOuter.X, lootBoxOuter.Y, Texture.LOOT_BOX);
            _spriteBatch.Draw(GetTexture(Texture.LOOT_BOX), lootBoxOuter, Color.White);

            int x = -5;
            int y = 6;

            foreach (Model.Item item in unit.BackPack.BackpackItems)
            {
                //Bugfix. Doesnt show if the objectlayer items is not drawn
                item.ThisItem.Bounds.X = lootBoxInner.X + _camera.GetScreenRectangle.X + x;
                item.ThisItem.Bounds.Y = lootBoxInner.Y + _camera.GetScreenRectangle.Y + y;

                Vector2 itemPosition = _camera.VisualizeCordinates(item.ThisItem.Bounds.X, item.ThisItem.Bounds.Y);
                
                //If type is Armor
                if(item.GetType() == Model.GameModel.ARMOR)
                {
                    if(item.Type == Model.Armor.HEAD_ARMOR)
                        _spriteBatch.Draw(GetTexture(Texture.HEADARMOR), itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                }
                //If type is QuestItem
                else if (item.GetType() == Model.GameModel.QUEST_ITEM)
                {
                    if (item.Type == Model.QuestItem.ENEMY_HEAD)
                        _spriteBatch.Draw(GetTexture(Texture.BOSS_HEAD), itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                }

                if (_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)itemPosition.X, (int)itemPosition.Y, item.ThisItem.Bounds.Width, item.ThisItem.Bounds.Height)))
                {
                    _player.ItemTarget = item;

                    if(_player.ItemTarget.GetType() != Model.GameModel.QUEST_ITEM)
                        item.WasLooted = true;

                    if(!item.WasLooted)
                    {
                        //Checks if an enemyhead is already in backpack
                        if (!_player.BackPack.BackpackItems.Exists(Item => Item.Type == Model.QuestItem.ENEMY_HEAD && Item.GetType() == Model.GameModel.QUEST_ITEM))
                            item.WasLooted = true;
                        else 
                            _player.ItemTarget = null;
                    }
                }

                x++;
                if (x >= GetTexture(Texture.LOOT_BOX).Width - displacementX * 2)
                {
                    y += 48;
                    x = 0;
                }

                //Checks if the cursor is hovering an item
                if (_inputHandler.MouseIsOver(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                    _common.DrawItemStats(item, itemPosition);
            }

            //Checks if the loot box was closed or got out of range
            if (_inputHandler.DidGetTargetedByLeftClick(closeCross) || !_player.ThisUnit.Bounds.Intersects(unit.ThisUnit.Bounds))
                _player.LootTarget = null;            

            unit.BackPack.BackpackItems.RemoveAll(Item => Item.WasLooted == true);
        }

        /// <summary>
        /// Method for drawing all player dmg output and healing input
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        private void DrawDamageAndHealing(float elapsedTime)
        {
            #region AoutoHitDMG
            _dmgCounter = _dmgCounter + (elapsedTime * 75);

            if (_player.IsWithinMeleRange && _player.Target != null && _player.SwingTime < 50 && _player.SwingTime > 25)
            {
                Vector2 position = _camera.VisualizeCordinates(_player.Target.ThisUnit.Bounds.X + 20, _player.Target.ThisUnit.Bounds.Y);

                Color color = Color.White;
                float scale = 1;

                if (_player.AutohitDamage > 10)
                {
                    color = Color.Yellow;
                    scale = scale + _dmgCounter / 50;
                }

                //Random: randomizing the position of dmg output qhilst scale is for crits
                _spriteBatch.DrawString(_healFont, _player.AutohitDamage.ToString(), position - new Vector2(_randomNr + (scale * 5), _dmgCounter), color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            else
            {
                _dmgCounter = 0;
                Random rdm = new Random();
                _randomNr = rdm.Next(-10, 10);
            } 
            #endregion

            #region Spells
            if (_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.INSTANT_HEAL && Spell.Duration == 0 && Spell.CoolDown > 4))
                DrawSpellOutput(elapsedTime, Model.SpellSystem.INSTANT_HEAL);
            else
                _healingCounter = 0;  

            if (_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.SMITE && Spell.Duration == 0 && Spell.CoolDown > 1))
                DrawSpellOutput(elapsedTime, Model.SpellSystem.SMITE);
            else
                _smiteCounter = 0;
            #endregion
        }

        //Metod för utritning av spells skada/healing
        /// <summary>
        /// Method for drawing spells, dmg and healing
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="spellType">Type of spell</param>
        private void DrawSpellOutput(float elapsedTime, Type spellType)
        {
            Model.Spell spell = _activeSpells.Find(Spell => Spell.GetType() == spellType);
            string output = null;
            float counter = 0;
            Color color = Color.White;
            Vector2 position = Vector2.Zero;

            if (spellType == Model.SpellSystem.SMITE && _player.Target != null)
            {
                Model.Smite smite = spell as Model.Smite;
                output = ((int)smite.Damage).ToString();
                _smiteCounter = _smiteCounter + (elapsedTime * 75);
                counter = _smiteCounter;
                position = _camera.VisualizeCordinates(_player.Target.ThisUnit.Bounds.X, _player.Target.ThisUnit.Bounds.Y);
            }
            else if (spellType == Model.SpellSystem.INSTANT_HEAL)
            {
                Model.InstantHeal heal = spell as Model.InstantHeal;
                output = ((int)heal.Heal).ToString();
                _healingCounter = _healingCounter + (elapsedTime * 75);
                counter = _healingCounter;
                color = Color.LightGreen;
                position = _camera.VisualizeCordinates(_player.ThisUnit.Bounds.X, _player.ThisUnit.Bounds.Y);
            }    
        
            if(output != null)
                 _spriteBatch.DrawString(_healFont, output, position - new Vector2(-7, counter), color);
        }

        /// <summary>
        /// Method for drawing the castbar
        /// </summary>
        private void DrawCastBar()
        {
            foreach (Model.Spell spell in _activeSpells)
            {
                //Caster is player
                if(spell.Caster == _player)
                {
                    //Cast time remains.
                    if(spell.CastTime > 0)
                    {
                    //Setting castbar width
                    float castBarWidth = _player.ThisUnit.Bounds.Width;

                    float casted = spell.CastTime / spell.FullCastTime;
                    casted = 1 - casted;
                    castBarWidth = casted * castBarWidth;
                    Vector2 castBarVector = _camera.VisualizeCordinates(_player.ThisUnit.Bounds.X, (_player.ThisUnit.Bounds.Y + _player.ThisUnit.Bounds.Height));
                    Rectangle castBarRect = new Rectangle((int)castBarVector.X, (int)castBarVector.Y, (int)castBarWidth, 6);
                    _spriteBatch.Draw(_avatar.ManaTexture, castBarRect, Color.White);
                   
                    }
                }
            }
        }

        /// <summary>
        /// Method for drawing of the CharacterPanel
        /// </summary>
        private void DrawCharacterPanel()
        {
            Vector2 pos = new Vector2();
            Vector2 panelPos = new Vector2(_player.CharPanel.Position.X, _player.CharPanel.Position.Y);
            Model.Item statsItem = null;

            Rectangle charPanelRect = new Rectangle(0, 0, GetTexture(Texture.CHAR_PANEL_BG).Width, GetTexture(Texture.CHAR_PANEL_BG).Height);
            Rectangle closeCross = GetCloseButton(_player.CharPanel.Position.X, _player.CharPanel.Position.Y, Texture.CHAR_PANEL_BG);

            if(_player.CharPanel.IsOpen)
            {
                _spriteBatch.Draw(GetTexture(Texture.CHAR_PANEL_BG), panelPos, charPanelRect, Color.White);
                _spriteBatch.DrawString(_common.SpriteFontSegoe, "Health: +" + _player.TotalHp.ToString(), panelPos + new Vector2(80, 70), Color.White);
                _spriteBatch.DrawString(_common.SpriteFontSegoe, "Mana: +" + _player.TotalMana.ToString(), panelPos + new Vector2(80, 85), Color.White);
                _spriteBatch.DrawString(_common.SpriteFontSegoe, "Damage: +10", panelPos + new Vector2(80, 100), Color.White);
                _spriteBatch.DrawString(_common.SpriteFontSegoe, "Crit: 10%", panelPos + new Vector2(80, 115), Color.White);
                _spriteBatch.DrawString(_common.SpriteFontSegoe, "Armor: +" + _player.Armor.ToString(), panelPos + new Vector2(80, 130), Color.White);
                _spriteBatch.DrawString(_common.SpriteFontSegoe, "Resist: +" + _player.Resist.ToString(), panelPos + new Vector2(80, 145), Color.White);

                if (_inputHandler.MouseIsOver(new Rectangle((int)_player.CharPanel.Position.X, (int)_player.CharPanel.Position.Y, GetTexture(Texture.CHAR_PANEL_BG).Bounds.Width, GetTexture(Texture.CHAR_PANEL_BG).Bounds.Height)))
                    _inputHandler.MouseIsOverInterface = true;
                if (_inputHandler.DidGetTargetedByLeftClick(closeCross))
                    _player.CharPanel.IsOpen = false;
            }

            Vector2 position = new Vector2(_player.CharPanel.Position.X +13, _player.CharPanel.Position.Y+39);
            Rectangle itemRect = new Rectangle(0, 0, 32, 32);

            #region Ritar Equippade Items
            foreach (Model.Item item in _player.CharPanel.EquipedItems)
            {
                item.ThisItem.Bounds.X = (int)_player.CharPanel.Position.X + _camera.GetScreenRectangle.X +10;
                item.ThisItem.Bounds.Y = (int)_player.CharPanel.Position.Y + _camera.GetScreenRectangle.Y +25;

                if (_player.CharPanel.IsOpen)
                {
                    if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                        _player.CharPanelTarget = item;

                    if (item.GetType() == Model.GameModel.ARMOR)
                    {
                        Model.Armor Armor = item as Model.Armor;

                        if (Armor.Type == Model.Armor.HEAD_ARMOR)
                            _spriteBatch.Draw(GetTexture(Texture.HEADARMOR), position, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    }
                }
                //Checks if the ccursor is hovering the item
                if (_inputHandler.MouseIsOver(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                {
                    pos = position;
                    statsItem = item;
                }
            } 
            #endregion

            if (statsItem != null && _player.CharPanel.IsOpen)
                _common.DrawItemStats(statsItem, pos);
        }

        /// <summary>
        /// Method for drawing the backpack
        /// </summary>
        private void DrawBackpack()
        {
            //For item stats
            Vector2 pos = new Vector2();
            Model.Item statsItem = null;

            //Backpack rectangle
            Rectangle backpackRect = new Rectangle(0, 0, GetTexture(Texture.BACKPACK).Width, GetTexture(Texture.BACKPACK).Height);
            Rectangle closeCross = GetCloseButton(_player.BackPack.Position.X, _player.BackPack.Position.Y, Texture.BACKPACK);

            if (_player.BackPack.IsOpen)
            {
                _spriteBatch.Draw(GetTexture(Texture.BACKPACK), new Vector2(_player.BackPack.Position.X, _player.BackPack.Position.Y), backpackRect, Color.White);

                if (_inputHandler.MouseIsOver(new Rectangle((int)_player.BackPack.Position.X, (int)_player.BackPack.Position.Y, GetTexture(Texture.BACKPACK).Bounds.Width, GetTexture(Texture.BACKPACK).Bounds.Height)))
                    _inputHandler.MouseIsOverInterface = true;
             
                if (_inputHandler.DidGetTargetedByLeftClick(closeCross))
                    _player.BackPack.IsOpen = false;
            }

            //Rectangle for items within the backpack
            Rectangle itemRect = new Rectangle(0, 0, 32, 32);
            int x = 12;
            int y = 45;

            foreach (Model.Item item in _player.BackPack.BackpackItems)
            {
                //Bugfix. Doesnt show if the objectlayer items is not drawn
                item.ThisItem.Bounds.X = (int)_player.BackPack.Position.X + _camera.GetScreenRectangle.X + x;
                item.ThisItem.Bounds.Y = (int)_player.BackPack.Position.Y + _camera.GetScreenRectangle.Y + y;
                
                Vector2 itemPosition = new Vector2(_player.BackPack.Position.X + x, _player.BackPack.Position.Y + y);
                
                //If backpack is open
                if (_player.BackPack.IsOpen)
                {
                    if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                        _player.BackpackTarget = item;

                    //If type is Armor       
                    if (item.GetType() == Model.GameModel.ARMOR)
                    {
                        if (item.Type == Model.Armor.HEAD_ARMOR)
                            _spriteBatch.Draw(GetTexture(Texture.HEADARMOR), itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    }
                    //If type is Questitem.
                    else if (item.GetType() == Model.GameModel.QUEST_ITEM)
                    {
                        if (item.Type == Model.QuestItem.ENEMY_HEAD)
                            _spriteBatch.Draw(GetTexture(Texture.BOSS_HEAD), itemPosition, itemRect, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    }
                }

                //Itemplacement in backpack
                x += 50;

                if (x >= GetTexture(Texture.BACKPACK).Width)
                {
                    y += 50;
                    x = 0;
                }

                //If cursor is hovering item
                if (_inputHandler.MouseIsOver(_camera.VisualizeRectangle(item.ThisItem.Bounds)))
                {
                    pos = itemPosition;
                    statsItem = item;
                }
            }

            //If player is hovering an item and the backpack is open
            if(statsItem != null && _player.BackPack.IsOpen)
                _common.DrawItemStats(statsItem, pos);
            
            //Removes the item from the backpack and drops it on the ground @ rightclick
            if(_player.BackPack.IsOpen)
            {
                for (int i = 0; i < _player.BackPack.BackpackItems.Count; i++)
                {
                    if (_inputHandler.DidGetTargetedByRightClick(_camera.VisualizeRectangle(_player.BackPack.BackpackItems[i].ThisItem.Bounds)) && _player.BackPack.BackpackItems[i].GetType() != Model.GameModel.QUEST_ITEM)
                    {
                        _player.BackPack.BackpackItems[i].ThisItem.Bounds = _player.ThisUnit.Bounds;
                        _worldItems.Add(_player.BackPack.BackpackItems[i]);
                        _player.BackPack.BackpackItems.Remove(_player.BackPack.BackpackItems[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an rectangle that represents the area correspondent to the close cross for an open window/panel
        /// </summary>
        /// <param name="panelLocationX">Panels x location</param>
        /// <param name="panelLocationY">Panels Y location</param>
        /// <param name="index">Texture array index</param>
        /// <returns></returns>
        private Rectangle GetCloseButton(float panelLocationX, float panelLocationY, Texture index)
        {
            return new Rectangle((int)panelLocationX + GetTexture(index).Width - 25, (int)panelLocationY, 25, 25);
        }

        /// <summary>
        /// Returns the rectangel area of the given input actionbar
        /// </summary>
        /// <param name="input">Actionbar input</param>
        /// <returns>The rectangel area of the given input actionbar</returns>
        internal Rectangle GetActionBarArea(InputHandler.Input input)
        {
            return _actionBar.GetActionBarArea(input);
        }

        /// <summary>
        /// Converts an texture array index from enum to int32
        /// </summary>
        /// <param name="index">Texture array index</param>
        /// <returns>A texture 2D</returns>
        private Texture2D GetTexture(Texture index)
        {
            return _textures[Convert.ToInt32(index)];
        }
    }
}
