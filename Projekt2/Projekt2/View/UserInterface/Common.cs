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
    /// Class cointaning shared methods and properties used by several UI classes and methods
    /// </summary>
    class Common
    {
        private SpriteFont _spriteFontSegoe;
        private SpriteBatch _spriteBatch;
        private Texture2D[] _textures;

        /// <summary>
        /// Texture enum
        /// </summary>
        private enum Texture
        {
            ITEM_STATS_BACKGROUND = 0,
            ITEM_STATS_BG_SMALL = 1
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch"></param>
        public Common(SpriteBatch spriteBatch)
        {
            this._spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Loading all class related content
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public void LoadContent(ContentManager content)
        {
            _textures = new Texture2D[2] { content.Load<Texture2D>("Textures/Interface/itemStatsBackG"),
                                           content.Load<Texture2D>("Textures/Interface/itemStatsBGsmall")};

            _spriteFontSegoe = content.Load<SpriteFont>(@"Fonts\Segoe");
        }


        /// <summary>
        /// Method for rendering item information when hovering over the item
        /// </summary>
        /// <param name="item">The item whos stats will be shown</param>
        /// <param name="itemPosition">The items position</param>
        internal void DrawItemStats(Model.Item item, Vector2 itemPosition)
        {
            if (item.GetType() == Model.GameModel.QUEST_ITEM)
            {
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ITEM_STATS_BG_SMALL)], new Vector2(itemPosition.X + item.ThisItem.Bounds.Width - 10, itemPosition.Y), Color.White);
                _spriteBatch.DrawString(_spriteFontSegoe, "Quest Item", new Vector2(itemPosition.X + item.ThisItem.Bounds.Width, itemPosition.Y + 8), Color.White);
            }
            else
            {
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.ITEM_STATS_BACKGROUND)], new Vector2(itemPosition.X + item.ThisItem.Bounds.Width - 10, itemPosition.Y), Color.White);

                if (item.GetType() == Model.GameModel.ARMOR)
                {
                    Model.Armor armor = item as Model.Armor;
                    
                    if (armor.Type == Model.Armor.HEAD_ARMOR)
                        _spriteBatch.DrawString(_spriteFontSegoe, "Slot: Head", new Vector2(itemPosition.X + item.ThisItem.Bounds.Width, itemPosition.Y + 4), Color.White);

                    _spriteBatch.DrawString(_spriteFontSegoe, "Armor: +" + armor.ArmorValue.ToString(), new Vector2(itemPosition.X + item.ThisItem.Bounds.Width, itemPosition.Y + 19), Color.White);
                    _spriteBatch.DrawString(_spriteFontSegoe, "Resist: +" + armor.MagicResistValue.ToString(), new Vector2(itemPosition.X + item.ThisItem.Bounds.Width, itemPosition.Y + 34), Color.White);
                    _spriteBatch.DrawString(_spriteFontSegoe, "Health: +" + armor.HealthValue.ToString(), new Vector2(itemPosition.X + item.ThisItem.Bounds.Width, itemPosition.Y + 49), Color.White);
                    _spriteBatch.DrawString(_spriteFontSegoe, "Mana: +" + armor.ManaValue.ToString(), new Vector2(itemPosition.X + item.ThisItem.Bounds.Width, itemPosition.Y + 64), Color.White);
                }
            }
        }

        /// <summary>
        /// Font getter (Seoge)
        /// </summary>
        public SpriteFont SpriteFontSegoe
        {
            get { return _spriteFontSegoe; }
        }
    }
}
