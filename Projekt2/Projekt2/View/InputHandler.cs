using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace View
{
    /// <summary>
    /// Class for handling all user input
    /// </summary>
    class InputHandler
    {
        /// <summary>
        /// Input enum countaining all available user input choices
        /// </summary>
        public enum Input
        {
            UP = Keys.W,
            DOWN = Keys.S, 
            LEFT = Keys.A,
            RIGHT = Keys.D,
            ACTION_BAR_ONE = Keys.D1,
            ACTION_BAR_TWO = Keys.D2,
            ACTION_BAR_THREE = Keys.D3,
            ACTION_BAR_FOUR = Keys.D4,
            BACKPACK = Keys.B,
            CHARACTER_PANEL = Keys.C,
            QUEST_LOG = Keys.L,
            WORLD_MAP = Keys.M,
            DEBUG_MODE = Keys.V
        }

        //Keyboard/Mouse states
        private KeyboardState _kbs;
        private KeyboardState _prevKbs;
        private MouseState _mouseState;
        private MouseState _prevMoseState;

        //Mouse
        private bool _isMouseOverEnemy;
        private bool _isMouseOverNPC = false;
        private bool _isMouseOverInterface;
        private bool _isMouseOverLoot;

        //Menu inputs
        private bool _soundDisabled;
        private bool _musicDisabled;

        /// <summary>
        /// Sets a new keyboard state while saving the previous state
        /// </summary>
        internal void SetKeyboardState()
        {
            _prevKbs = _kbs;
            _kbs = Keyboard.GetState();
        }
      
        /// <summary>
        /// Sets a new mouse state while saving the previous state
        /// </summary>
        internal void SetMouseState()
        {
            _prevMoseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        /// <summary>
        /// Returns the current mouse state
        /// </summary>
        internal MouseState GetMouseState()
        {
            return _mouseState;
        }

        /// <summary>
        /// Checks if a given input key was pressed
        /// <param name="input">User input key</param> 
        /// </summary>
        internal bool IsKeyDown(Input input)
        {
            return _kbs.IsKeyDown((Keys)input);
        }

        /// <summary>
        /// Checks if a given input key was pressed and released
        /// <param name="input">User input key</param> 
        /// </summary>
        internal bool PressedAndReleased(Input input)
        {
            return (_kbs.IsKeyUp((Keys)input) && _prevKbs.IsKeyDown((Keys)input));
        }

        /// <summary>
        /// Checks if a given target was targeted by a mouse left click
        /// </summary>
        internal bool DidGetTargetedByLeftClick(Rectangle target)
        {
            return (DidLeftClick() && target.Intersects(new Rectangle(_mouseState.X, _mouseState.Y, 1, 1)));
        }

        /// <summary>
        /// Checks if a given target was targeted by a mouse right click
        /// </summary>
        internal bool DidGetTargetedByRightClick(Rectangle target)
        {
            return (DidRightClick() && target.Intersects(new Rectangle(_mouseState.X, _mouseState.Y, 1, 1)));
        }

        /// <summary>
        /// Checks if the right mouse button was clicked
        /// </summary>
        internal bool DidRightClick()
        {
            return (_prevMoseState.RightButton == ButtonState.Pressed && _mouseState.RightButton == ButtonState.Released);
        }

        /// <summary>
        /// Checks if the left mouse button was clicked
        /// </summary>
        internal bool DidLeftClick()
        {
            return (_prevMoseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released);
        }

        /// <summary>
        /// Checks if the mouse is hovering over a given rectangle area
        /// <param name="area">Rectangle area to check</param>
        /// </summary>
        internal bool MouseIsOver(Rectangle area)
        {
            return (area.Intersects(new Rectangle(_mouseState.X, _mouseState.Y, 1, 1)));
        }

        /// <summary>
        /// Checks if the mouse is hovering over an enemy unit
        /// </summary>
        internal bool MouseIsOverEnemy
        {
            get { return _isMouseOverEnemy; }
            set { _isMouseOverEnemy = value; }
        }

        /// <summary>
        /// Checks if the mouse is hovering over an enemy, npc or loot
        /// </summary>
        internal bool MouseIsOverObject
        {
            get { return _isMouseOverEnemy || _isMouseOverNPC || _isMouseOverLoot; }
        }

        /// <summary>
        /// Checks if the mouse is hovering over any inteface graphics
        /// </summary>
        internal bool MouseIsOverInterface
        {
            get { return _isMouseOverInterface; }
            set { _isMouseOverInterface = value; }
        }

        /// <summary>
        /// Checks if the mouse is hovering over loot
        /// </summary>
        internal bool MouseIsOverLoot
        {
            get { return _isMouseOverLoot; }
            set { _isMouseOverLoot = value; }
        }

        /// <summary>
        /// Checks if the right mouse button is pressed down 
        /// </summary>
        internal bool RightButtonIsDown()
        {
            return _mouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the left mouse button is pressed down 
        /// </summary>
        internal bool LeftButtonIsDown()
        {
            return _mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Checks if the user pressed and released the escape button
        /// </summary>
        internal bool PressedAndReleasedEsc()
        {
            return _kbs.IsKeyUp(Keys.Escape) && _prevKbs.IsKeyDown(Keys.Escape);
        }

        /// <summary>
        /// Checks wether or not the music is disabled 
        /// </summary>
        internal bool MusicDisabled
        {
            get { return _musicDisabled; }
            set { _musicDisabled = value; }
        }

        /// <summary>
        /// Checks wether or not the sound is disabled 
        /// </summary>
        internal bool SoundDisabled
        {
            get { return _soundDisabled; }
            set { _soundDisabled = value; }
        }
    }
}
