using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace View
{
    /// <summary>
    /// Class for rendering of menus and screens
    /// </summary>
    class ScreenView
    {
        //Private variables
        private SpriteBatch _spriteBatch;
        private Stopwatch _stopWatch;
       
        private View.Content _screenContent;
        private View.AnimationSystem _animationSys;
        private View.InputHandler _inputHandler;
        private View.SoundHandler _soundHandler; 

        private bool _didHover;
        private bool _showTemplarSelection = true;
        private bool _showProphetSelection;
        private bool _showDescendantSelection;
 
        //Properties
        internal bool DidPressNewGame { get; set; }
        internal bool DidPressQuit { get; set; }
        internal bool DidPressExit { get; set; }
        internal bool DidChooseClass { get; set; }
        internal bool DidPressOptions { get; set; }
        internal bool DidShowCredits { get; set; }
        internal bool FullScreen { get; set; }
       
        /// <summary>
        /// Enum for storing indexes of screen textures.
        /// </summary>
        public enum Screen
        {
            SCREEN_START = 0,
            SCREEN_ANIMATION_A = 1,
            SCREEN_ANIMATION_B = 2,
            SCREEN_CLASS_SELECT = 3,
            SCREEN_SELECT_TEMPLAR = 4,
            SCREEN_SELECT_PROPHET = 5,
            SCREEN_SELECT_DESCENDANT = 6,
            SCREEN_BORDER_BG = 7,
            SCREEN_TRANSP_BG = 8,
            SCREEN_PAUSE_BG = 9,
            SCREEN_OPTION = 10,
            SCREEN_END_BG = 11,
            SCREEN_CREDITS = 12
        }

        /// <summary>
        /// Enum for storing indexes of button textures.
        /// </summary>
        public enum Button
        {
            BUTTONS_START_MENU = 0,
            BUTTONS_NEWGAME_SELECTED = 1,
            BUTTONS_CREDITS_SELECTED = 2,
            BUTTONS_SELECT = 3,
            BUTTONS_SELECT_GRAY = 4,
            BUTTONS_SELECT_SELECTED = 5,
            BUTTONS_OPTIONS_SELECTED = 6,
            BUTTONS_PAUSE_MENU = 7,
            BUTTONS_QUIT_SELECTED = 8,
            BUTTONS_CHECK_BOX = 9,
            BUTTONS_OK = 10,
            BUTTONS_OK_SELECTED = 11,
            BUTTONS_EXIT_SELECTED = 12
        }
       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="gameModel">GameModel instance</param>
        /// <param name="animationSystem">AnimationSystem instance</param>
        /// <param name="inputHandler">InputHandler instance</param>
        /// <param name="soundHandler">SoundHandler instance</param> 
        public ScreenView(SpriteBatch spriteBatch, View.AnimationSystem animationSystem, View.InputHandler inputHandler, View.SoundHandler soundHandler)
        {
            this._spriteBatch = spriteBatch;
            this._animationSys = animationSystem;
            this._inputHandler = inputHandler;
            this._soundHandler = soundHandler;

            this._screenContent = new Content();
            this._stopWatch = new Stopwatch(); 
            this.FullScreen = false;
        }

        /// <summary>
        /// Loading all view content
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public void LoadContent(ContentManager a_content)
        {
            this._screenContent.LoadScreenContent(a_content);
            this._animationSys.LoadContent(a_content);
        }

        /// <summary>
        /// Method for drawing the start screen
        /// </summary>
        internal void DrawStartScreen()
        {
            _stopWatch.Start();
            PlayStartTheme();
            
            //Startscreen background
            DrawImage(Screen.SCREEN_START, Vector2.Zero, new Point(1280, 720), 1.0f);

            #region Startscreen animation
            int y = 0;
            int x = 10;

            if (_stopWatch.ElapsedMilliseconds > 400)
            {
                y = 59;
                x = 192;
            }
            if (_stopWatch.ElapsedMilliseconds > 800)
            {
                y = 118;
                x = 374;
            }
            if (_stopWatch.ElapsedMilliseconds > 1200)
            {
                _stopWatch.Stop();
                _stopWatch.Reset();
            }

            _spriteBatch.Draw(_screenContent.Screens[Convert.ToInt32(Screen.SCREEN_ANIMATION_A)], new Vector2(23, 260), new Rectangle(0, y, 265, 59), Color.White);
            _spriteBatch.Draw(_screenContent.Screens[Convert.ToInt32(Screen.SCREEN_ANIMATION_B)], new Vector2(154, 352), new Rectangle(x, 0, 170, 247), Color.White);
            #endregion
          
            if (!DidPressOptions && !DidShowCredits)
            {
                //Drawing startmenu
                DrawImage(Screen.SCREEN_BORDER_BG, new Vector2(727, 0), new Point(300, 720), 1.0f);
                DrawButton(Button.BUTTONS_START_MENU, new Vector2(750, 325), new Point(512, 512), 0.5f);

                //Button areas for startmenu
                Rectangle newGameArea = new Rectangle(750, 325, 250, 40);
                Rectangle optionsArea = new Rectangle(750, 404, 250, 40);
                Rectangle creditsArea = new Rectangle(750, 450, 250, 30);
                Rectangle exitArea = new Rectangle(750, 485, 250, 30);

                #region Inputhandling for startmenu
                if (_inputHandler.MouseIsOver(newGameArea))
                {
                    DrawButton(Button.BUTTONS_NEWGAME_SELECTED, new Vector2(750, 319), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(newGameArea))
                    {
                        PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT_B);
                        DidPressNewGame = true;
                    }
                }
                else if (_inputHandler.MouseIsOver(optionsArea))
                {
                    DrawButton(Button.BUTTONS_OPTIONS_SELECTED, new Vector2(750, 396), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(optionsArea))
                    {
                        PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                        DidPressOptions = true;
                    }
                }
                else if (_inputHandler.MouseIsOver(creditsArea))
                {
                    DrawButton(Button.BUTTONS_CREDITS_SELECTED, new Vector2(750, 434), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(creditsArea))
                    {
                        PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                        DidShowCredits = true;
                    }
                }
                else if (_inputHandler.MouseIsOver(exitArea))
                {
                    DrawButton(Button.BUTTONS_EXIT_SELECTED, new Vector2(750, 473), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(exitArea))
                    {
                        DidPressExit = true;
                    }
                }
                else
                    _didHover = false;
                #endregion
            }
            else if (DidPressOptions)
                DrawOptionScreen(new Vector2(727, 0));
            else
                DrawCreditsScreen();
        }

        /// <summary>
        /// Method for drawing the option screen
        /// </summary>
        /// <param name="position">Screen position</param>
        internal void DrawOptionScreen(Vector2 position)
        {
            DrawImage(Screen.SCREEN_OPTION, position, new Point(300, 720), 1.0f);

            #region Graphics

            Rectangle defaultGraphics = new Rectangle((int)position.X + 17, 192, 22, 20);
            Rectangle fullscreenGraphics = new Rectangle((int)position.X + 17, 211, 22, 20);

            if (_inputHandler.DidGetTargetedByLeftClick(defaultGraphics))
                FullScreen = false;
            if (_inputHandler.DidGetTargetedByLeftClick(fullscreenGraphics))
                FullScreen = true;
            if (!FullScreen)
                DrawButton(Button.BUTTONS_CHECK_BOX, new Vector2((int)position.X + 17, 192), new Point(22, 20), 1f);
            else
                DrawButton(Button.BUTTONS_CHECK_BOX, new Vector2((int)position.X + 17, 211), new Point(22, 20), 1f);
            
            #endregion

            #region Sound
           
            Rectangle disableSound = new Rectangle((int)position.X + 17, 263, 22, 20);
            Rectangle disableMusic = new Rectangle((int)position.X + 17, 281, 22, 20);

            if (_inputHandler.DidGetTargetedByLeftClick(disableSound) && !_inputHandler.SoundDisabled)
                _inputHandler.SoundDisabled = true;
            
            else if (_inputHandler.DidGetTargetedByLeftClick(disableSound) && _inputHandler.SoundDisabled)
                _inputHandler.SoundDisabled = false;
            
            if (_inputHandler.DidGetTargetedByLeftClick(disableMusic) && !_inputHandler.MusicDisabled)
                _inputHandler.MusicDisabled = true;
            
            else if (_inputHandler.DidGetTargetedByLeftClick(disableMusic) && _inputHandler.MusicDisabled)
                _inputHandler.MusicDisabled = false;
            
            if (_inputHandler.SoundDisabled)
                DrawButton(Button.BUTTONS_CHECK_BOX, new Vector2((int)position.X + 17, 263), new Point(22, 20), 1f);
            
            if (_inputHandler.MusicDisabled)
                DrawButton(Button.BUTTONS_CHECK_BOX, new Vector2((int)position.X + 17, 281), new Point(22, 20), 1f);

            #endregion

            #region OkButton
            Rectangle okButton = new Rectangle((int)position.X + 21, 550, 256, 30);
            DrawButton(Button.BUTTONS_OK, new Vector2((int)position.X + 21, 550), new Point(512, 83), 0.5f);

            if (_inputHandler.MouseIsOver(okButton))
            {
                DrawButton(Button.BUTTONS_OK_SELECTED, new Vector2((int)position.X + 21, 544), new Point(512, 83), 0.5f);
                PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                if (_inputHandler.DidGetTargetedByLeftClick(okButton))
                {
                    DidPressOptions = false;
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                }
            }
            else
                _didHover = false;
            #endregion
        }

        /// <summary>
        /// Method for drawing the credits screen
        /// </summary>
        internal void DrawCreditsScreen()
        {
            DrawImage(Screen.SCREEN_BORDER_BG, new Vector2(727, 0), new Point(300, 720), 1.0f);
            DrawImage(Screen.SCREEN_CREDITS, Vector2.Zero, new Point(1280, 720), 1.0f);

            #region OkButton
            Rectangle okButton = new Rectangle(750, 560, 256, 30);
            DrawButton(Button.BUTTONS_OK, new Vector2(750, 560), new Point(512, 83), 0.5f);

            if (_inputHandler.MouseIsOver(okButton))
            {
                DrawButton(Button.BUTTONS_OK_SELECTED, new Vector2(750, 554), new Point(512, 83), 0.5f);
                PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                if (_inputHandler.DidGetTargetedByLeftClick(okButton))
                {
                    DidShowCredits = false;
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                }
            }
            else
                _didHover = false;
            #endregion
        }

        /// <summary>
        /// Method for drawing the class selection screen
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void DrawClassSelectionScreen(float elapsedTime)
        {
            PlayStartTheme();
            //Drawing background
            DrawImage(Screen.SCREEN_CLASS_SELECT, Vector2.Zero, new Point(1280, 720), 1f);

            //Positions for animations
            Vector2 templarPosition = new Vector2(300, 170);
            Vector2 prophetPosition = new Vector2(616, 170);
            Vector2 descendantPosition = new Vector2(916, 170);

            //Button areas for select buttons
            Rectangle selectTemplar = new Rectangle(209, 600, 256, 50);
            Rectangle selectProphet = new Rectangle(0,0, 512, 100);
            Rectangle selectDescendant = new Rectangle(0, 0, 512, 100);

            #region Templar
            if (_inputHandler.MouseIsOver(new Rectangle((int)templarPosition.X, (int)templarPosition.Y, 64, 64)) || _showTemplarSelection)
            {
                if (_showTemplarSelection)
                {
                    _spriteBatch.Draw(_screenContent.Screens[Convert.ToInt32(Screen.SCREEN_SELECT_TEMPLAR)], new Vector2(185, 0), new Rectangle(0, 0, 300, 720), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    _animationSys.UpdateAndDraw(elapsedTime, Color.White, templarPosition, Model.State.MOVING_DOWN, AnimationSystem.Texture.CLASS_SCREEN_TEMPLAR);
                    _spriteBatch.Draw(_screenContent.Buttons[Convert.ToInt32(Button.BUTTONS_SELECT)], new Vector2(203, 601), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                   
                    #region SelectButton
                    if (_inputHandler.MouseIsOver(selectTemplar))
                    {
                        _spriteBatch.Draw(_screenContent.Buttons[Convert.ToInt32(Button.BUTTONS_SELECT_SELECTED)], new Vector2(209, 600), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                        if (_inputHandler.DidGetTargetedByLeftClick(selectTemplar))
                        {
                            PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT_B);
                            DidChooseClass = true;
                        }
                    }
                    else
                        _didHover = false; 
                    #endregion
                }
                else
                    _animationSys.UpdateAndDraw(elapsedTime, Color.White, templarPosition, Model.State.FACING_CAMERA, AnimationSystem.Texture.CLASS_SCREEN_TEMPLAR);

                if (_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)templarPosition.X, (int)templarPosition.Y, 64, 64)))
                {
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                    _showTemplarSelection = true;
                    _showProphetSelection = false;
                    _showDescendantSelection = false;
                }
            }
            else if (!_showTemplarSelection)
                _animationSys.UpdateAndDraw(elapsedTime, Color.White, templarPosition, Model.State.FACING_AWAY, AnimationSystem.Texture.CLASS_SCREEN_TEMPLAR);
            #endregion

            #region Prophet
            if (_inputHandler.MouseIsOver(new Rectangle((int)prophetPosition.X, (int)prophetPosition.Y, 64, 64)) || _showProphetSelection)
            {
                if (_showProphetSelection)
                {
                    _spriteBatch.Draw(_screenContent.Screens[Convert.ToInt32(Screen.SCREEN_SELECT_PROPHET)], new Vector2(501, 0), new Rectangle(0, 0, 300, 720), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    _animationSys.UpdateAndDraw(elapsedTime, Color.White, prophetPosition, Model.State.MOVING_DOWN, AnimationSystem.Texture.CLASS_SCREEN_PROPHET);
                    _spriteBatch.Draw(_screenContent.Buttons[Convert.ToInt32(Button.BUTTONS_SELECT_GRAY)], new Vector2(525, 600), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                }
                else
                    _animationSys.UpdateAndDraw(elapsedTime, Color.White, prophetPosition, Model.State.FACING_CAMERA, AnimationSystem.Texture.CLASS_SCREEN_PROPHET);

                if (_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)prophetPosition.X, (int)prophetPosition.Y, 64, 64)))
                {
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                    _showTemplarSelection = false;
                    _showProphetSelection = true;
                    _showDescendantSelection = false;
                }
            }
            else if (!_showProphetSelection)
                _animationSys.UpdateAndDraw(elapsedTime, Color.White, prophetPosition, Model.State.FACING_AWAY, AnimationSystem.Texture.CLASS_SCREEN_PROPHET);
            #endregion

            #region Descendant
            if (_inputHandler.MouseIsOver(new Rectangle((int)descendantPosition.X, (int)descendantPosition.Y, 64, 64)) || _showDescendantSelection)
            {
                if (_showDescendantSelection)
                {
                    _spriteBatch.Draw(_screenContent.Screens[Convert.ToInt32(Screen.SCREEN_SELECT_DESCENDANT)], new Vector2(801, 0), new Rectangle(0, 0, 300, 720), Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
                    _animationSys.UpdateAndDraw(elapsedTime, Color.White, descendantPosition, Model.State.MOVING_DOWN, AnimationSystem.Texture.CLASS_SCREEN_DESCENDANT);
                    _spriteBatch.Draw(_screenContent.Buttons[Convert.ToInt32(Button.BUTTONS_SELECT_GRAY)], new Vector2(825, 600), selectProphet, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                }
                else
                    _animationSys.UpdateAndDraw(elapsedTime, Color.White, descendantPosition, Model.State.FACING_CAMERA, AnimationSystem.Texture.CLASS_SCREEN_DESCENDANT);

                if (_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)descendantPosition.X, (int)descendantPosition.Y, 64, 64)))
                {
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                    _showTemplarSelection = false;
                    _showProphetSelection = false;
                    _showDescendantSelection = true;
                }
            }
            else if (!_showDescendantSelection)
                _animationSys.UpdateAndDraw(elapsedTime, Color.White, descendantPosition, Model.State.FACING_AWAY, AnimationSystem.Texture.CLASS_SCREEN_DESCENDANT);
            #endregion
        }

        /// <summary>
        /// Method for drawing the pause screen
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void DrawPauseScreen(float elapsedTime)
        {
            Rectangle optionsArea = new Rectangle(850, 325, 250, 30);
            Rectangle quitArea = new Rectangle(850, 364, 250, 30);

            DrawImage(Screen.SCREEN_TRANSP_BG, new Vector2(0, 0), new Point(1280, 720),1.0f);

            if (!DidPressOptions)
            {
                DrawImage(Screen.SCREEN_PAUSE_BG, new Vector2(827, 0), new Point(300, 720), 1.0f);
                DrawButton(Button.BUTTONS_PAUSE_MENU, new Vector2(850, 250), new Point(512, 330), 0.5f);

                if (_inputHandler.MouseIsOver(optionsArea))
                {
                    DrawButton(Button.BUTTONS_OPTIONS_SELECTED, new Vector2(850, 321), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(optionsArea))
                    {
                        PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_SELECT);
                        DidPressOptions = true;
                    }
                }
                else if (_inputHandler.MouseIsOver(quitArea))
                {
                    DrawButton(Button.BUTTONS_QUIT_SELECTED, new Vector2(850, 359), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(quitArea))
                        DidPressQuit = true;
                    else
                        DidPressQuit = false;
                }
                else
                    _didHover = false;
            }
            else
                DrawOptionScreen(new Vector2(827, 0));
        }

        /// <summary>
        /// Method for drawing the game over screen
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void DrawGameOverScreen(float elapsedTime)
        {
            if (!DidShowCredits)
            {
                DrawImage(Screen.SCREEN_END_BG, new Vector2(0, 0), new Point(1280, 720), 1.0f);
                DrawImage(Screen.SCREEN_BORDER_BG, new Vector2(727, 0), new Point(300, 720), 1.0f);
                _spriteBatch.Draw(_screenContent.Buttons[Convert.ToInt32(Button.BUTTONS_START_MENU)], new Vector2(750, 440), new Rectangle(0, 230, 512, 250), Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                Rectangle creditsArea = new Rectangle(750, 450, 250, 30);
                Rectangle exitArea = new Rectangle(750, 485, 250, 30);

                #region Inputhandling for game over screen

                if (_inputHandler.MouseIsOver(creditsArea))
                {
                    DrawButton(Button.BUTTONS_CREDITS_SELECTED, new Vector2(750, 434), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(creditsArea))
                        DidShowCredits = true;
                }
                else if (_inputHandler.MouseIsOver(exitArea))
                {
                    DrawButton(Button.BUTTONS_EXIT_SELECTED, new Vector2(750, 473), new Point(512, 100), 0.5f);
                    PlayMenuSound(SoundHandler.Effect.MENU_BUTTON_HOVER);

                    if (_inputHandler.DidGetTargetedByLeftClick(exitArea))
                        DidPressExit = true;
                }
                else
                    _didHover = false;

                #endregion
            }
            else
                DrawCreditsScreen();
        }

        /// <summary>
        /// Method for drawing screen buttons
        /// </summary>
        /// <param name="index">Button index for texture array</param>
        /// <param name="pos">Button position</param>
        /// <param name="size">Button size</param>
        /// <param name="scale">Button scale</param>
        internal void DrawButton(Button index, Vector2 pos, Point size, float scale)
        {
            _spriteBatch.Draw(_screenContent.Buttons[Convert.ToInt32(index)], pos, new Rectangle(0, 0, size.X, size.Y), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Method for drawing screen backgrounds and images
        /// </summary>
        /// <param name="index">Image index for texture array</param>
        /// <param name="pos">Image position</param>
        /// <param name="size">Image size</param>
        /// <param name="scale">Image scale</param>
        internal void DrawImage(Screen index, Vector2 pos, Point size, float scale)
        {
            _spriteBatch.Draw(_screenContent.Screens[Convert.ToInt32(index)], pos, new Rectangle(0, 0, size.X, size.Y), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Method for drawing the mouse cursor
        /// </summary>
        internal void DrawMouse()
        {
            Vector2 mousePosition = new Vector2(_inputHandler.GetMouseState().X, _inputHandler.GetMouseState().Y);

            if (!_inputHandler.MouseIsOverEnemy)
            {
                if (_inputHandler.LeftButtonIsDown())
                    _spriteBatch.Draw(_screenContent.MouseTextures[1], mousePosition, Color.White);
                else
                    _spriteBatch.Draw(_screenContent.MouseTextures[0], mousePosition, Color.White);
            }
        }

        /// <summary>
        /// Method for playing the start theme
        /// </summary>
        internal void PlayStartTheme()
        {
            if (_inputHandler.MusicDisabled)
                _soundHandler.StopTrack();
            else
                _soundHandler.PlaySoundTrack(SoundHandler.Track.THEME);
        }

        /// <summary>
        /// Method for playing menu sounds
        /// </summary>
        internal void PlayMenuSound(SoundHandler.Effect sound)
        {
            if (sound == SoundHandler.Effect.MENU_BUTTON_HOVER && !_didHover)
            {
                _didHover = true;
                _soundHandler.PlaySound(SoundHandler.Effect.MENU_BUTTON_HOVER, 0.2f);
            }
            else if (sound == SoundHandler.Effect.MENU_BUTTON_SELECT)
                _soundHandler.PlaySound(SoundHandler.Effect.MENU_BUTTON_SELECT, 0.2f);
            else if (sound == SoundHandler.Effect.MENU_BUTTON_SELECT_B)
                _soundHandler.PlaySound(SoundHandler.Effect.MENU_BUTTON_SELECT_B, 0.4f);
        }

        internal bool PressedAndReleasedEsc() { return _inputHandler.PressedAndReleasedEsc(); }
    }
}
