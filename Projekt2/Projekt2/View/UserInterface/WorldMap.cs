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
    /// Class for rendering the world maps
    /// </summary>
    class WorldMap
    {
        private View.InputHandler _inputHandler;
        private View.Camera _camera;
        private Model.Player _player;
        private Texture2D[] _textures;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Texture enum for world map related textures
        /// </summary>
        private enum Texture
        {
            PLAYER_POSITION = 0,
            WORLD_MAP = 1,
            WORLD_MAP_BG = 2
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="player">Player object</param>
        /// <param name="inputHandler">InputHandler instance</param>
        /// <param name="camera">Camera instance</param>
        public WorldMap(SpriteBatch spriteBatch, Model.Player player, InputHandler inputHandler, Camera camera)
        {
            this._player = player;
            this._spriteBatch = spriteBatch;
            this._inputHandler = inputHandler;
            this._camera = camera;
        }

        /// <summary>
        /// Loading all world map related textures
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public void LoadContent(ContentManager content)
        {
            _textures = new Texture2D[3] { content.Load<Texture2D>("WorldMaps/playerPos"), 
                                           content.Load<Texture2D>("WorldMaps/theworldFG"),
                                           content.Load<Texture2D>("WorldMaps/theworldBG")};
        }

        /// <summary>
        /// Method for rendering of the world map and the player position
        /// </summary>
        internal void DrawWorldMap()
        {
            if (_player.IsLookingAtMap)
            {
                Vector2 position = new Vector2(_camera.GetScreenRectangle.Width / 2 - _textures[Convert.ToInt32(Texture.WORLD_MAP_BG)].Bounds.Width / 2, _camera.GetScreenRectangle.Height - (_textures[Convert.ToInt32(Texture.WORLD_MAP_BG)].Bounds.Height * 1.25f));
                Rectangle mapRect = new Rectangle((int)position.X + 15, (int)position.Y + 30, 500, 500);
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.WORLD_MAP_BG)], position, Color.White);
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.WORLD_MAP)], mapRect, Color.White);
                _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.PLAYER_POSITION)], new Vector2(mapRect.X + _player.ThisUnit.Bounds.X / 20, mapRect.Y + _player.ThisUnit.Bounds.Y / 20), Color.White);

                //Fixa med anrop till GeatCloseRec
                Rectangle closeCross = new Rectangle((int)position.X + _textures[Convert.ToInt32(Texture.WORLD_MAP_BG)].Width - 25, (int)position.Y, 25, 25);

                if (_inputHandler.MouseIsOver(new Rectangle((int)position.X, (int)position.Y, _textures[Convert.ToInt32(Texture.WORLD_MAP_BG)].Bounds.Width, _textures[Convert.ToInt32(Texture.WORLD_MAP_BG)].Bounds.Height)))
                    _inputHandler.MouseIsOverInterface = true;
                if (_inputHandler.DidGetTargetedByLeftClick(closeCross))
                {
                    _player.IsLookingAtMap = false;
                }
            }
        }
    }
}
