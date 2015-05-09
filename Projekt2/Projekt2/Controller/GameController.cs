using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Controller
{
    /// <summary>
    /// Main controller class for the game
    /// </summary>
    class GameController
    {
        //Variebles
        private Model.GameModel _gameModel;
        private View.GameView _gameView;

        /// <summary>
        /// Cinstructor
        /// </summary>
        /// <param name="graphicsDevice">GraphickDevice instance</param>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="gameModel">GameModel instance</param>
        /// <param name="animationSystem">AnimationSystem instance</param>
        /// <param name="inputHandler">InputHandler instance</param>
        /// <param name="soundHandler">SoundHandler instance</param>
        public GameController(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Model.GameModel gameModel,
            View.AnimationSystem animationSystem, View.InputHandler inputHandler, View.SoundHandler soundHandler)
        {
            this._gameModel = gameModel;
            this._gameView = new View.GameView(graphicsDevice, spriteBatch, _gameModel, animationSystem, inputHandler, soundHandler);
        }

        /// <summary>
        /// Method for lodaing the game content via the GameView
        /// </summary>
        /// <param name="content">ContentManagerInstance</param>
        internal void LoadContent(ContentManager content)
        {      
            _gameView.LoadContent(content);
        }

        /// <summary>
        /// Updating the player movement in relation to the user input.
        /// Updating the GameModel logic.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void UpdateSimulation(float elapsedTime)
        {
            Model.Player player = _gameModel.PlayerSystem.Player;
            Vector2 moveToPosition = _gameView.GetMapMousePosition();

            #region MouseMoveInteraktion

            float xSpeed = 0;
            float ySpeed = 0;

            Rectangle mouseRect = new Rectangle((int)moveToPosition.X, (int)moveToPosition.Y, 1, 1);
            if (!player.IsCastingSpell && player.IsAlive())
            {
                //Holding down right mouse button.
                if (moveToPosition != Vector2.Zero)
                {
                    player.MoveToPosition = moveToPosition;
                    player.Direction = new Vector2(moveToPosition.X - player.ThisUnit.Bounds.Center.X, moveToPosition.Y - player.ThisUnit.Bounds.Center.Y);
                }
                //Click to move
                else if (player.MoveToPosition != Vector2.Zero)
                    player.Direction = new Vector2(player.MoveToPosition.X - player.ThisUnit.Bounds.Center.X, player.MoveToPosition.Y - player.ThisUnit.Bounds.Center.Y);

                Vector2 newCords = new Vector2();
                Vector2 facing = new Vector2();

                //If player is moving.
                if (player.Direction != Vector2.Zero && !_gameModel.PlayerSystem.ArrivedToPosition(player.ThisUnit.Bounds, player.MoveToPosition, 5))
                {
                    newCords = player.Direction;
                    newCords.Normalize();
                    newCords.X = newCords.X * 4;
                    newCords.Y = newCords.Y * 4;

                    player.ThisUnit.Bounds.X += (int)newCords.X;
                    player.ThisUnit.Bounds.Y += (int)newCords.Y;
                }
                //If player targeted an unit - set the player state accordingly.
                if (player.Target != null && player.Target.ThisUnit.Bounds.Intersects(player.MaxRangeArea)) 
                {
                    facing.X = player.Target.ThisUnit.Bounds.Center.X - player.ThisUnit.Bounds.Center.X;
                    facing.Y = player.Target.ThisUnit.Bounds.Center.Y - player.ThisUnit.Bounds.Center.Y;
                }
                else
                    facing = newCords;

                xSpeed = Math.Abs(facing.X);
                ySpeed = Math.Abs(facing.Y);

                if (facing == new Vector2())
                {
                    player.UnitState = Model.State.FACING_CAMERA;
                    player.WeaponState = Model.State.MOVING_DOWN;
                }
                if (player.Target != null && player.Target.ThisUnit.Bounds.Intersects(player.MaxRangeArea))
                {
                    if (xSpeed > ySpeed)
                    {
                        if (facing.X > 0f)
                        {
                            player.UnitState = Model.State.FACING_RIGHT;
                            player.WeaponState = Model.State.MOVING_RIGHT;
                        }
                        else
                        {
                            player.UnitState = Model.State.FACING_LEFT;
                            player.WeaponState = Model.State.MOVING_LEFT;
                        }
                    }
                    else
                    {
                        if (facing.Y > 0f)
                        {
                            player.UnitState = Model.State.FACING_CAMERA;
                            player.WeaponState = Model.State.MOVING_DOWN;
                        }
                        else
                        {
                            player.UnitState = Model.State.FACING_AWAY;
                            player.WeaponState = Model.State.MOVING_UP;
                        }
                    }
                }
                if (newCords != new Vector2() && !_gameModel.PlayerSystem.ArrivedToPosition(player.ThisUnit.Bounds, player.MoveToPosition, 5))
                {
                    xSpeed = Math.Abs(newCords.X);
                    ySpeed = Math.Abs(newCords.Y);
                    if (xSpeed > ySpeed)
                    {
                        if (facing.X > 0f)
                        {
                            player.UnitState = Model.State.MOVING_RIGHT;
                            player.WeaponState = player.UnitState;
                        }
                        else
                        {
                            player.UnitState = Model.State.MOVING_LEFT;
                            player.WeaponState = player.UnitState;
                        }
                    }
                    else
                    {
                        if (facing.Y > 0f)
                        {
                            player.UnitState = Model.State.MOVING_DOWN;
                            player.WeaponState = player.UnitState;
                        }
                        else
                        {
                            player.UnitState = Model.State.MOVING_UP;
                            player.WeaponState = player.UnitState;
                        }
                    }
                }
            }
            else if(player.IsCastingSpell)
                player.UnitState = Model.State.IS_CASTING_HEAL;
            else
                player.UnitState = Model.State.FACING_CAMERA;
            #endregion

            #region ActionBarInteraktion

            if (_gameView.DidActivateActionBar(View.InputHandler.Input.ACTION_BAR_TWO))
                _gameModel.PlayerSystem.SpellSystem.AddSpell(Model.SpellSystem.INSTANT_HEAL, player);

            if (_gameView.DidActivateActionBar(View.InputHandler.Input.ACTION_BAR_ONE) && _gameModel.PlayerSystem.Player.Target != null)
            {
                if (_gameModel.PlayerSystem.Player.Target.GetType() == Model.GameModel.ENEMY_NPC)
                    _gameModel.PlayerSystem.SpellSystem.AddSpell(Model.SpellSystem.SMITE, player);
            }

            if (_gameView.DidActivateActionBar(View.InputHandler.Input.BACKPACK))
            {
                if (!player.BackPack.IsOpen)
                    player.BackPack.IsOpen = true;
                else
                    player.BackPack.IsOpen = false;
            }

            if (_gameView.DidActivateActionBar(View.InputHandler.Input.QUEST_LOG))
            {
                if (!_gameModel.QuestSystem.IsWatchingQuestLog)
                    _gameModel.QuestSystem.IsWatchingQuestLog = true;
                else
                    _gameModel.QuestSystem.IsWatchingQuestLog = false;
            }

            #endregion

            //Opening/closing worldmap.
            if (_gameView.DidActivateActionBar(View.InputHandler.Input.WORLD_MAP))
            {
                if (!player.IsLookingAtMap)
                    player.IsLookingAtMap = true;
                else
                    player.IsLookingAtMap = false;
            }

            //Opening/closing character panel.
            if (_gameView.DidActivateActionBar(View.InputHandler.Input.CHARACTER_PANEL))
            {
                if (!player.CharPanel.IsOpen)
                    player.CharPanel.IsOpen = true;
                else
                    player.CharPanel.IsOpen = false;
            }

            //If player un-targets an enemy - stop attacking. 
            if (player.Target != null)
            {
                if (_gameView.DidUnTarget(player))
                {
                    player.IsAttacking = false;
                    player.Target = null;
                }
            }

            #region DEBUG / CHEAT
            //Immortal.
            //if (m_gameView.DidPressAndReleaseKey('R'))
            //{
            //    if (m_gameModel.mPlayerSystem.m_player.Armor < 90)
            //    {
            //        m_gameModel.mPlayerSystem.m_player.Armor = 100;
            //    }
            //}
            ////Full mana and hp.
            //if (m_gameView.DidPressAndReleaseKey('F'))
            //{
            //    m_gameModel.mPlayerSystem.m_player.CurrentHp = m_gameModel.mPlayerSystem.m_player.TotalHp;
            //    m_gameModel.mPlayerSystem.m_player.CurrentMana = m_gameModel.mPlayerSystem.m_player.TotalMana;
            //}
            ////Obstacles - on/off.
            //if (m_gameView.DidPressAndReleaseKey('T'))
            //{
            //    if (m_gameModel.mQuestSystem.CurrentQuestIndex != 2)
            //    {
            //        m_gameModel.mQuestSystem.CurrentQuestIndex = 2;
            //        m_gameModel.mQuestSystem.CurrentQuest.Status = Model.QuestSystem.END;
            //    }
            //}

            //if (m_gameView.DidPressAndReleaseKey('G'))
            //{
            //    foreach (Model.Enemy e in m_gameModel.mEnemySystem.m_enemies)
            //    {
            //        if (e.Type == Model.Enemy.BOSS_A)
            //        {
            //            m_gameModel.mPlayerSystem.m_player.ThisUnit.Bounds.Location = e.ThisUnit.Bounds.Location;
            //        }
            //    }
            //}

            //if (m_gameView.DidPressAndReleaseKey('H'))
            //{
            //    foreach (Model.Friend f in m_gameModel.m_friendSystem.m_friends)
            //    {
            //        if (f.Type == Model.Friend.CITY_GUARD)
            //        {
            //            m_gameModel.mPlayerSystem.m_player.ThisUnit.Bounds.Location = f.ThisUnit.Bounds.Location;
            //        }
            //    }
            //}
            #endregion

            #region KeyBoardMovement (Not currently used)
            if (_gameView.DidPressKey(View.InputHandler.Input.DOWN) && player.CanMoveDown)
            {
                player.ThisUnit.Bounds.Y += Convert.ToInt32(elapsedTime * 200);
                player.UnitState = Model.State.MOVING_DOWN;
            }
            if (_gameView.DidPressKey(View.InputHandler.Input.UP) && player.CanMoveUp)
            {
                player.ThisUnit.Bounds.Y -= Convert.ToInt32(elapsedTime * 200);
                player.UnitState = Model.State.MOVING_UP;
            }
            if (_gameView.DidPressKey(View.InputHandler.Input.RIGHT) && player.CanMoveRight)
            {
                player.ThisUnit.Bounds.X += Convert.ToInt32(elapsedTime * 200);
                player.UnitState = Model.State.MOVING_RIGHT;

            }
            if (_gameView.DidPressKey(View.InputHandler.Input.LEFT) && player.CanMoveLeft)
            {
                player.ThisUnit.Bounds.X -= Convert.ToInt32(elapsedTime * 200);
                player.UnitState = Model.State.MOVING_LEFT;
            }
            #endregion

            //Updating game logic.
            _gameModel.Update(elapsedTime);
        }

        /// <summary>
        /// Drawing the game graphics via the GameView.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds.</param>
        internal void Draw(float elapsedTime)
        {
            //Uppdaterar och ritar grafik
            _gameView.DrawAndUpdate(elapsedTime);
        }
    }
}
