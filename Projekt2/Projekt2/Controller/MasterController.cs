using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework.Media;

namespace Controller
{
    /// <summary>
    /// Main controller class.
    /// </summary>
    public class MasterController : Microsoft.Xna.Framework.Game
    {
        //Variables.
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;
        private PerformanceUtility.PerformanceUtility _performanceTool;

        private GameController _gameController;
        private ScreenController _screenController;

        private View.InputHandler _inputHandler;
        private View.AnimationSystem _animationSystem;
        private View.SoundHandler _soundHandler;
        private Model.GameModel _gameModel; 

        /// <summary>
        /// Constructor.
        /// </summary>
        public MasterController()
        {
            this._graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this._graphics.PreferredBackBufferHeight = 720;
            this._graphics.PreferredBackBufferWidth = 1280;
        }

        /// <summary>
        /// Initializer method.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Loading game engine, controller and view.
        /// Loading all needed class instances.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _performanceTool = new PerformanceUtility.PerformanceUtility(_graphics, this);
            _performanceTool.LoadContent(Content, _spriteBatch);
            _gameModel = new Model.GameModel(Content);
            
            //Classes needed for all controller classes (GameController and ScreenController)
            _inputHandler = new View.InputHandler();
            _animationSystem = new View.AnimationSystem(_spriteBatch);
            _soundHandler = new View.SoundHandler(_inputHandler);
            _soundHandler.LoadContent(Content);

            //Controllers
            _screenController = new ScreenController(_gameModel, _spriteBatch, _animationSystem, _inputHandler, _soundHandler);
            _gameController = new GameController(GraphicsDevice, _spriteBatch, _gameModel, _animationSystem, _inputHandler, _soundHandler);

            _gameController.LoadContent(Content);
            _screenController.LoadScreenContent(Content);

            //Initializing renderingobject for TMX files (map-files).
            Map.InitObjectDrawing(GraphicsDevice);
        }

        /// <summary>
        /// Method for unloading content and un-initializing objects.
        /// </summary>
        protected override void UnloadContent()
        {
            _soundHandler = null;
            MediaPlayer.Stop();
            this.Content.Unload();
        }

        /// <summary>
        /// Updating screens and game engine through the controllers.
        /// </summary>
        /// <param name="gameTime">GameTime object</param>
        protected override void Update(GameTime gameTime)
        {
            //Starting performance watch for update in debug mode
            _performanceTool.RecordUpdateStart(gameTime);

            //Updating keyboard & mousestate
            _inputHandler.SetKeyboardState();
            _inputHandler.SetMouseState();
            
            //Updating ScreenController
            _screenController.UpdateScreenSimulation((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (_screenController.DoQuit)
            {
                UnloadContent();
                LoadContent();
            }

            if (_screenController.DoExit)
                this.Exit();

            //If the user selects full screen.
            if (_screenController.FullScreen != _graphics.IsFullScreen && !_graphics.IsFullScreen)
            {
                _graphics.PreferMultiSampling = false;
                _graphics.IsFullScreen = true;
                _graphics.ApplyChanges();
            }
            else if (_screenController.FullScreen != _graphics.IsFullScreen && _graphics.IsFullScreen)
            {
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
            }
            //Updating the game enging via the GameController if no screen is active.
            if (!_screenController.IsShowingExternalScreen() && !_screenController.IsShowingPauseScreen)
                _gameController.UpdateSimulation((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);

            //Ending performance watch for update in debug mode
            _performanceTool.RecordUpdateEnd(gameTime);
        }

        /// <summary>
        /// Method for drawing the game graphics through the Game and Screen view (via Controllers).
        /// </summary>
        /// <param name="gameTime">GameTime object</param>
        protected override void Draw(GameTime gameTime)
        {
            //Starting performance watch for draw in debug mode
            _performanceTool.RecordDrawStart(gameTime);

            GraphicsDevice.Clear(Color.Black);

            if (_screenController.IsShowingExternalScreen())
                _screenController.DrawScreens((float)gameTime.ElapsedGameTime.TotalSeconds);
            //Drawing the game graphics via the GameController if no screen is active.
            else
            {
                _gameController.Draw((float)gameTime.ElapsedGameTime.TotalSeconds);

                //Pause screen is drawn over while the game engine is "paused".
                if(_screenController.IsShowingPauseScreen)
                    _screenController.DrawScreens((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            base.Draw(gameTime);

            //Ending performance watch for draw in debug mode
            _performanceTool.RecordDrawEnd(gameTime);
        }
    }
}