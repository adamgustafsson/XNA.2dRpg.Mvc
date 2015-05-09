using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View.UI
{
    /// <summary>
    /// Class for rendering the actionbar and its components
    /// </summary>
    class ActionBar
    {
        private Camera _camera;
        private Model.QuestSystem _questSystem;
        private Model.Player _player;

        private SpriteBatch _spriteBatch;
        private Texture2D[] _textures;
        private Vector2[] _barPosition;
        private List<Model.Spell> _activeSpells;
        private SpriteFont _spriteFontBig;

        /// <summary>
        /// Texture enum for indexing of actionbar related textures
        /// </summary>
        private enum Texture
        { 
            ICON_INSTANT_HEAL = 0,
            ICON_INSTANT_HEAL_CD = 1,
            ICON_BAG = 2,
            ICON_BAG_OPEN = 3,
            ICON_SMITE = 4,
            ICON_SMITE_CD = 5,
            ICON_WORLD_MAP = 6,
            ICON_WORLD_MAP_SELECTED = 7,
            ICON_CHAR_PANEL = 8,
            ICON_CHAR_PANEL_SELECTED = 9,
            ICON_QUEST_LOG = 10,
            ICON_QUEST_LOG_SELECTED = 11,
            ACTION_BAR = 12
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="activeSpells">List of activ spells</param>
        /// <param name="player">Player object</param>
        /// <param name="questSystem">QuestSystem instance</param>
        /// <param name="camera">Camera instance</param>
        public ActionBar(SpriteBatch spriteBatch, List<Model.Spell> activeSpells, Model.Player player, Model.QuestSystem questSystem, Camera camera)
        {
            this._spriteBatch = spriteBatch;
            this._camera = camera;
            this._player = player;
            this._questSystem = questSystem;
            
            this._barPosition = new Vector2[7];
            this._activeSpells = activeSpells;
        }

        /// <summary>
        /// Loading all actionbar related textures
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        internal void LoadContent(ContentManager content)
        {
            _textures = new Texture2D[13] { content.Load<Texture2D>("Textures/Interface/Icons/instantHeal"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/instantHealCD"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/bagIcon"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/bagIconOpen"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/smite"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/smiteCD"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/worldMap"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/worldMapSelected"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/charPanel2"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/charPanelSelected2"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/qLog"),
                                                content.Load<Texture2D>("Textures/Interface/Icons/qLogSelected"),
                                                content.Load<Texture2D>("Textures/Interface/spellBar2")};

            _spriteFontBig = content.Load<SpriteFont>(@"Fonts\SegoeBig");
        }

        /// <summary>
        /// Method for rendering of all actionbars
        /// </summary>
        internal void DrawActionBar()
        {
            Vector2 position = new Vector2(_camera.GetScreenRectangle.Width / 2 - _textures[Convert.ToInt32(Texture.ACTION_BAR)].Bounds.Width / 2, _camera.GetScreenRectangle.Height - _textures[Convert.ToInt32(Texture.ACTION_BAR)].Bounds.Height * 1.5f);
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ACTION_BAR)], position, Color.White);

            _barPosition[0] = position + new Vector2(40, 51);
            _barPosition[1] = position + new Vector2(88, 51);
            _barPosition[2] = position + new Vector2(150, 51);
            _barPosition[3] = position + new Vector2(189, 51);
            _barPosition[4] = position + new Vector2(239, 51);
            _barPosition[5] = position + new Vector2(288, 51);
            _barPosition[6] = position + new Vector2(334, 49);
           
            //Instant heal
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_INSTANT_HEAL)], _barPosition[1], Color.White);

            if (_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.INSTANT_HEAL && Spell.CoolDown > 0))
            {
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_INSTANT_HEAL_CD)], _barPosition[1], Color.White);
                string cd = ((int)_activeSpells.Find(Spell => Spell.GetType() == Model.SpellSystem.INSTANT_HEAL && Spell.CoolDown > 0).CoolDown).ToString();
                _spriteBatch.DrawString(_spriteFontBig, cd, _barPosition[1] + new Vector2(14, 5), Color.White);
            }

            //Smite
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_SMITE)], _barPosition[0], Color.White);

            if (_activeSpells.Exists(Spell => Spell.GetType() == Model.SpellSystem.SMITE && Spell.CoolDown > 0))
            {
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_SMITE_CD)], _barPosition[0], Color.White);
                string cd = ((int)_activeSpells.Find(Spell => Spell.GetType() == Model.SpellSystem.SMITE && Spell.CoolDown > 0).CoolDown).ToString();
                _spriteBatch.DrawString(_spriteFontBig, cd, _barPosition[0] + new Vector2(14, 5), Color.White);
            }
            
            //Bag
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_BAG)], _barPosition[6], Color.White);

            if (_player.BackPack.IsOpen)
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_BAG_OPEN)], _barPosition[6], Color.White);

            //Map
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_WORLD_MAP)], _barPosition[5], Color.White);

            if (_player.IsLookingAtMap)
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_WORLD_MAP_SELECTED)], _barPosition[5], Color.White);

            //Character panel
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_CHAR_PANEL)], _barPosition[4], Color.White);

            if (_player.CharPanel.IsOpen)
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_CHAR_PANEL_SELECTED)], _barPosition[4], Color.White);

            //Questlog
            _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_QUEST_LOG)], _barPosition[3], Color.White);

            if (_questSystem.IsWatchingQuestLog)
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ICON_QUEST_LOG_SELECTED)], _barPosition[3], Color.White);
        }

        /// <summary>
        /// Returns the rectangel area of the given input actionbar
        /// </summary>
        /// <param name="input">Actionbar input</param>
        /// <returns></returns>
        internal Rectangle GetActionBarArea(InputHandler.Input input)
        {
            int keyArrayIndex = 0;

            switch (input)
            {
                case View.InputHandler.Input.ACTION_BAR_ONE:
                    keyArrayIndex = 0;
                    break;
                case View.InputHandler.Input.ACTION_BAR_TWO:
                    keyArrayIndex = 1;
                    break;
                case View.InputHandler.Input.ACTION_BAR_THREE:
                    keyArrayIndex = 2;
                    break;
                case View.InputHandler.Input.QUEST_LOG:
                    keyArrayIndex = 3;
                    break;
                case View.InputHandler.Input.CHARACTER_PANEL:
                    keyArrayIndex = 4;
                    break;
                case View.InputHandler.Input.WORLD_MAP:
                    keyArrayIndex = 5;
                    break;
                case View.InputHandler.Input.BACKPACK:
                    keyArrayIndex = 6;
                    break;
            }

            return new Rectangle((int)_barPosition[keyArrayIndex].X, (int)_barPosition[keyArrayIndex].Y, 48, 48);
        }

        /// <summary>
        /// Returns an array of the position of all actionbars
        /// </summary>
        public Vector2[] BarPositions
        {
            get { return _barPosition; }
        }
    }
}
