using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace View
{
    /// <summary>
    /// Class for drawing all game animations
    /// </summary>
    public class Animation
    {
        private Texture2D _myTexture;
        private Vector2 _origin;
        private string _name;
        private int _framecountX, _framecountY, _previousFrameX, _frameX, _frameY;
        private float _rotation, _scale, _depth, _totalElapsed, _timePerFrame;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="textureName">The texture file name</param>
        /// <param name="origin">Default: 0</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="scale">Default: 1</param>
        /// <param name="depth">Depth</param>
        /// <param name="frameX">Amount of x frames</param>
        /// <param name="frameY">Amount of y frames</param>
        /// <param name="framesPerSec">Animation speed</param>
        public Animation(string textureName, Vector2 origin, float rotation, float scale, float depth, int frameX, int frameY, int framesPerSec)
        {
            this._name = textureName;
            this._origin = origin;
            this._rotation = rotation;
            this._scale = scale;
            this._depth = depth;
            this._framecountX = frameX;
            this._framecountY = frameY;
            this._timePerFrame = (float)0.5 / framesPerSec;
            this._previousFrameX = 1;
            this._frameX = 1;
            this._frameY = 0;
            this._totalElapsed = 0;
        }

        /// <summary>
        /// Loading the specific sprite
        /// </summary>
        /// <param name="content">ContentManager</param>
        public void Load(ContentManager content)
        {
            _myTexture = content.Load<Texture2D>("Sprites/" + _name);
        }

        /// <summary>
        /// Animates a sprite horizontally
        /// </summary>
        /// <param name="elapsed">Elapsed time - milleseconds</param>
        /// <param name="frameY">Starting vertical frame</param>
        public void HorizontalAnimation(float elapsed, int frameY)
        {
            _frameY = frameY;
            _frameX = _previousFrameX;
            _totalElapsed += elapsed;

            if (_totalElapsed > _timePerFrame)
            {
                _frameX++;                 

                if (_frameX == 3)          
                    _frameX = 0;          

                _previousFrameX = _frameX;
                _totalElapsed -= _timePerFrame;
            }
        }

        /// <summary>
        /// Animates a sprite vertically
        /// </summary>
        /// <param name="elapsed">Elapsed time - milleseconds</param>
        /// <param name="frameX">Starting horizontal frame</param>
        public void VerticalAnimation(float elapsed, int frameX)
        {
            _frameX = frameX;
            _totalElapsed += elapsed;

            if (_totalElapsed > _timePerFrame)
            {
                _frameY++;                

                if (_frameY == 4)         
                    _frameY = 3;           

                _totalElapsed -= _timePerFrame;
            }
        }

        /// <summary>
        /// Draws a static frame
        /// </summary>
        /// <param name="elapsed">Elapsed time - milleseconds</param>
        /// <param name="frameY">Vertical frame</param>
        public void StaticTexture(int frameY)
        {
            _frameY = frameY;
            _frameX = 1;
        }

        /// <summary>
        /// Draws a frame 
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="screenPos">Position</param>
        /// <param name="color">Color</param> 
        public void DrawFrame(SpriteBatch batch, Vector2 screenPos, Color color)
        {
            DrawFrame(batch, _frameX, _frameY, screenPos, color);
        }

        /// <summary>
        /// Draws a frame 
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="frameX">X position</param>
        /// <param name="frameY">Y position</param> 
        /// <param name="screenPos">Position</param> 
        /// <param name="color">Color</param> 
        public void DrawFrame(SpriteBatch batch, int frameX, int frameY, Vector2 screenPos, Color color)
        {
            int FrameWidth = _myTexture.Width / _framecountX;
            int FrameHeight = _myTexture.Height / _framecountY;

            Rectangle sourcerect = new Rectangle(FrameWidth * frameX, FrameHeight * frameY, FrameWidth, FrameHeight);

            batch.Draw(_myTexture, screenPos, sourcerect, color, _rotation, _origin, _scale, SpriteEffects.None, _depth);
        }

        /// <summary>
        /// Resets the current frame
        /// </summary>
        public void ResetFrames()
        {
            _frameX = 0;
            _totalElapsed = 0f;
        }
    }
}
