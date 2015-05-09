using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Reader;

namespace View
{
    /// <summary>
    /// Class for handling and drawing of dialog boxes and ints interaction buttons
    /// </summary>
    class Dialog
    {
        private List<Dialogue> _dialogueList;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Rectangle _textRect;
        private Rectangle _speakerRect;
        private Camera _camera;
        private bool _drawDialog;
        private Texture2D _dialogueWindow;
        private Model.QuestSystem _questSystem;
        private View.InputHandler _inputHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="gameModel">Game model instance</param>
        /// <param name="camera">Camera instance</param>
        /// <param name="inputHandler">InputHandler instance</param>
        public Dialog(SpriteBatch spriteBatch, Model.GameModel gameModel, Camera camera, View.InputHandler inputHandler)
        {
            this._spriteBatch = spriteBatch;
            this._dialogueList = new List<Dialogue>();
            this._questSystem = gameModel.QuestSystem;
            this._camera = camera;
            this._inputHandler = inputHandler;
        }

        /// <summary>
        /// Loading te associated xml, font and texture files
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public void LoadContent(ContentManager content)
        {
            _dialogueWindow = content.Load<Texture2D>("Textures/Conversation/bgprat");
            _dialogueList = content.Load<List<Dialogue>>("XML/dialogue");
            _spriteFont = content.Load<SpriteFont>(@"Fonts\Segoe");
        }

        /// <summary>
        /// Method for rendering of conversation text
        /// </summary>
        /// <param name="targetNpc">The source npc unit</param>
        /// <param name="isQuestDialog">Type of converation</param>
        public void DrawNPCText(Model.Unit targetNpc, bool isQuestDialog)
        {
            //Text rectangle
            _textRect = _camera.VisualizeRectangle(new Rectangle(targetNpc.ThisUnit.Bounds.X, targetNpc.ThisUnit.Bounds.Y, 300, 50));

            //Vizualizing the map-objects rectangle
            _speakerRect = _camera.VisualizeRectangle(new Rectangle(targetNpc.ThisUnit.Bounds.X, targetNpc.ThisUnit.Bounds.Y, targetNpc.ThisUnit.Bounds.Width, targetNpc.ThisUnit.Bounds.Height));

            string message = null;
            int state = 0;

            //Quest dialog
            if (isQuestDialog)
                message = GetQuestMessage(_textRect);
            //Standard dialog    
            else
            {
                //If the game have passed the 3rd quest the dialog-state is 1, else 0
                if (_questSystem.CurrentQuest.Id > 2)
                    state = 1;

                message = GetMessage(targetNpc.UnitId, state);
            }

            //Moving the text box verticaly if it overlaps the speaker
            if (_textRect.Intersects(_speakerRect))
            {
                int overlap = _textRect.Bottom - _speakerRect.Top;
                _textRect.Y -= overlap;
            }

            //Drawing textbox and text
            if (_drawDialog)
            {
                _spriteBatch.Draw(_dialogueWindow, _textRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(_spriteFont, message, new Vector2(_textRect.X + 12, _textRect.Y + 12), Color.White);

                if (isQuestDialog)
                    DrawQuestButtons();
            }
        }

        /// <summary>
        /// Method for rendering of interaction buttons
        /// </summary>
        private void DrawQuestButtons()
        {
            //Button properties
            int buttonWidth = (int)(270f * 0.27f);
            int buttonHeight = (int)(100f * 0.26f); //Reduce dialog box size
            int textMargin = 7;
            Color colorOne = Color.White;
            Color colorTwo = Color.White;
            string buttonOneText;
            string buttonTwoText;

            //Button rectangles
            Rectangle buttonOne = new Rectangle(_textRect.Right - (buttonWidth * 2), _textRect.Bottom, buttonWidth, buttonHeight);
            Rectangle buttonTwo = new Rectangle(_textRect.Right - buttonWidth, _textRect.Bottom, buttonWidth, buttonHeight);

            if (_questSystem.QuestStatus == Model.QuestSystem.PRE)
            {
                buttonOneText = "Accept";
                buttonTwoText = "Decline";
                _spriteBatch.Draw(_dialogueWindow, new Vector2(_textRect.Right - buttonWidth, _textRect.Bottom), new Rectangle(0, 0, 270, 100), Color.White, 0f, Vector2.Zero, 0.26f, SpriteEffects.None, 0f);
                _spriteBatch.Draw(_dialogueWindow, new Vector2(_textRect.Right - (buttonWidth * 2), _textRect.Bottom), new Rectangle(0, 0, 270, 100), Color.White, 0f, Vector2.Zero, 0.26f, SpriteEffects.None, 0f);

                if (_inputHandler.MouseIsOver(buttonOne))
                {
                    colorOne = Color.Green;
                    if (_inputHandler.DidGetTargetedByLeftClick(buttonOne))
                        _questSystem.QuestStatus = Model.QuestSystem.MID;
                }

                if (_inputHandler.MouseIsOver(buttonTwo))
                {
                    colorTwo = Color.Red;
                    if (_inputHandler.DidGetTargetedByLeftClick(buttonTwo))
                        _drawDialog = false;
                }

                _spriteBatch.DrawString(_spriteFont, buttonOneText, new Vector2(buttonOne.X + 7, buttonOne.Y + 4), colorOne);

            }
            else
            {
                if ((_questSystem.QuestStatus == Model.QuestSystem.MID))
                    buttonTwoText = "Okey";
                else
                    buttonTwoText = "Complete";
                    textMargin = 4;

                if (_inputHandler.MouseIsOver(buttonTwo))
                {
                    colorTwo = Color.Green;
                    if (_inputHandler.DidGetTargetedByLeftClick(buttonTwo))
                    {
                        _drawDialog = false;
                        
                        if (buttonTwoText == "Complete")
                            _questSystem.ActivateNextQuest();
                    }

                }

                _spriteBatch.Draw(_dialogueWindow, new Vector2(_textRect.Right - buttonWidth, _textRect.Bottom), new Rectangle(0, 0, 270, 100), Color.White, 0f, Vector2.Zero, 0.26f, SpriteEffects.None, 0f);
            }

            _spriteBatch.DrawString(_spriteFont, buttonTwoText, new Vector2(buttonTwo.X + textMargin, buttonTwo.Y + 4), colorTwo);
        }

        /// <summary>
        /// Method for adapting a string according to given rectangle size
        /// </summary>
        /// <param name="message">String message</param>
        /// <param name="rectangle">Target rectangle</param>
        public string ConstrainText(String message, Rectangle rectangle)
        {
            bool filled = false;
            string line = "";
            string returnString = "";
            string[] wordArray = message.Split(' ');

            //Each word in string
            foreach (string word in wordArray)
            {
                //If next word exceeds the limit rectangle width
                if (_spriteFont.MeasureString(line + word).X > rectangle.Width - 20)
                {
                    //If a new line does not exceed the rectangles height
                    if (_spriteFont.MeasureString(returnString + line + "\n").Y < rectangle.Height)
                    {
                        returnString += line + "\n";
                        line = "";
                        //Space beneeth the last line
                        rectangle.Height += 18;
                    }
                    //If the new line exceeds the rectangle height
                    else if (!filled)
                    {
                        filled = true;
                        returnString += line;
                        line = "";
                    }
                }
                line += word + " ";
            }
            _textRect = rectangle;
            return returnString + line;
        }

        /// <summary>
        /// Method for collecting dialog messages from XML file
        /// </summary>
        /// <param name="id">Message id</param>
        /// <param name="stateIndex">Message state</param>
        public string GetMessage(int id, int stateIndex)
        {
            string message = null;

            foreach (Reader.Dialogue dialogue in _dialogueList)
            {
                if (dialogue.id == id)
                {
                    message = dialogue.message[stateIndex].msg;
                    if (dialogue.message[stateIndex].choices != null)
                    {
                        foreach (string choice in dialogue.message[stateIndex].choices)
                        {
                            message += choice;
                        }
                    }
                }
            }

            return ConstrainText(message, _textRect);
        }

        /// <summary>
        /// Method for collecting quest messages from XML file
        /// </summary>
        /// <param name="rectangle">Message box to fill</param>
        public string GetQuestMessage(Rectangle rectangle)
        {
            return ConstrainText(_questSystem.CurrentMessage, rectangle);
        }

        /// <summary>
        /// Method for collecting quest log messages from XML file
        /// </summary>
        /// <param name="rectangle">Message box to fill</param>
        public string GetLogMessage(Rectangle rectangle)
        {
            return ConstrainText(_questSystem.CurrentQuest.MidMessage, rectangle);
        }

        #region Get/Set
        public bool DrawDialog
        {
            get { return _drawDialog; }
            set { _drawDialog = value; }
        }
        public Rectangle TextRect
        {
            get { return _textRect; }
            set { _textRect = value; }
        }

        public List<Dialogue> DialogueList
        {
            get { return _dialogueList; }
            set { _dialogueList = value; }
        }     
        #endregion
    }
}
