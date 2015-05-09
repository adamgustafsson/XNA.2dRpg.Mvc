using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    /// <summary>
    /// Class for handling the ScreenView content 
    /// </summary>
    class Content
    {
        private SpriteFont _segoeKeycaps;
        private Texture2D[] _screenTextures;
        private Texture2D[] _screenButtons;
        private Texture2D[] _mouseTextures;

        /// <summary>
        /// Loading all ScreenView content
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public void LoadScreenContent(ContentManager a_content)
        {
            this._screenTextures = new Texture2D[13] {a_content.Load<Texture2D>("Textures/Screens/Images/BG2"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/startanimation1"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/test9"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/bg3"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/selectTemplar"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/selectProphet"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/selectDescendant"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/borderBg"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/transpBg"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/pauseBg"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/optionScreen"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/endBG"),
                                           a_content.Load<Texture2D>("Textures/Screens/Images/credits")};

            this._screenButtons = new Texture2D[13] {a_content.Load<Texture2D>("Textures/Screens/Buttons/menyFinal"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/newGameSelect"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/creditsSelect"),                                          
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/select"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/selectGray"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/selectSelected"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/optionsSelect"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/pauseMeny"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/quitSelected"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/checkBox"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/ok"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/okSelected"),
                                           a_content.Load<Texture2D>("Textures/Screens/Buttons/exitSelected")};

            this._mouseTextures = new Texture2D[2] { a_content.Load<Texture2D>("Textures/Interface/cursor"),
                                                      a_content.Load<Texture2D>("Textures/Interface/cursorSelect")};

            this._segoeKeycaps = a_content.Load<SpriteFont>("Fonts/Segoe");
        }

        //Readonly properties
        public Texture2D[] Screens { get { return _screenTextures; } }
        public Texture2D[] Buttons { get { return _screenButtons; } }
        public Texture2D[] MouseTextures { get { return _mouseTextures; } } 
    }
}
