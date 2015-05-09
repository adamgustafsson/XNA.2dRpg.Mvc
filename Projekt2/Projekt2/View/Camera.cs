using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FuncWorks.XNA.XTiled;

namespace View
{
    /// <summary>
    /// Class for updating the game camera position
    /// </summary>
    class Camera
    {
        private Rectangle _mapView;
        private Viewport _viewPort;
        private Map _currentMap;
        private Model.Player _player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphicsDevice">GraphicsDevice instance</param>
        /// <param name="gameModel">Game model instance</param>
        public Camera(GraphicsDevice graphicsDevice, Model.GameModel gameModel)
        {
            this._viewPort = graphicsDevice.Viewport;
            this._mapView = graphicsDevice.Viewport.Bounds;
            this._currentMap = gameModel.CurrentMap;
            this._player = gameModel.PlayerSystem.Player;
        }

        /// <summary>
        /// Updating camera rules
        /// </summary>
        internal void UpdateCamera()
        {
            Rectangle delta = _viewPort.Bounds;

            if (delta.X != _player.ThisUnit.Bounds.Center.X && _player.ThisUnit.Bounds.Center.X > _viewPort.Width / 2 && _player.ThisUnit.Bounds.Center.X < (_currentMap.Bounds.Width - _viewPort.Width / 2))
            {
                delta.X = _player.ThisUnit.Bounds.Center.X - _viewPort.Width / 2;
            }
            if (delta.Y != _player.ThisUnit.Bounds.Center.Y && _player.ThisUnit.Bounds.Center.Y > _viewPort.Height / 2 && _player.ThisUnit.Bounds.Center.Y < (_currentMap.Bounds.Height - _viewPort.Height / 2))
            {
                delta.Y = _player.ThisUnit.Bounds.Center.Y - _viewPort.Height / 2;
            }
            if (_currentMap.Bounds.Contains(delta))
            {
                _mapView = delta;
            }
        }

        /// <summary>
        /// Method for visualization of logic map-coordinates
        /// </summary>
        /// <param name="xCordinate">Logic x-coordinate</param>
        /// <param name="yCordinate">Logic y-coordinate</param> 
        internal Vector2 VisualizeCordinates(int xCordinate, int yCordinate)
        {
            return new Vector2(xCordinate - _mapView.X, yCordinate - _mapView.Y);
        }

        /// <summary>
        /// Method for logicalization of visual coordinates
        /// </summary>
        /// <param name="xCordinate">Visual x-coordinate</param>
        /// <param name="yCordinate">Visual y-coordinate</param> 
        internal Vector2 LogicalizeCordinates(int xCordinate, int yCordinate)
        {
            return new Vector2(xCordinate + _mapView.X, yCordinate + _mapView.Y);
        }

        /// <summary>
        /// Method for visualization of logic map-rectangles
        /// </summary>
        /// <param name="mapObject">Rectangular map-object</param>
        internal Rectangle VisualizeRectangle(Rectangle mapObject)
        {
            Vector2 cords = VisualizeCordinates(mapObject.X, mapObject.Y);
            return new Rectangle((int)cords.X, (int)cords.Y, mapObject.Width, mapObject.Height);
        }

        /// <summary>
        /// Returns the cameras current location
        /// </summary>
        /// <return>Current camera location</return>
        public Rectangle GetScreenRectangle
        {
            get { return _mapView; }
        }
    }
}
