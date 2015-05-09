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
    /// Class for rendering the user interface avatars
    /// </summary>
    class Avatar
    {
        private Model.Player _player;
        private Texture2D[] _avatarTextures;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Texture enum for avatar related textures
        /// </summary>
        private enum Texture
        { 
            RED_HP = 0,
            GREEN_HP = 1,
            MANA = 2,
            AVATAR_PLAYER = 3,
            AVATAR_KNIGHT = 4,
            AVATAR_OLD_MAN = 5,
            AVATAR_GOBLIN = 6,
            AVATAR_FIREMAGE = 7,
            AVATAR_BRYNOLF = 8,
            AVATAR_CITY_GUARD = 9
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="player">Player object</param>
        public Avatar(SpriteBatch spriteBatch, Model.Player player)
        {
            this._player = player;
            this._spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Loading all avatar related textures
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        public void LoadContent(ContentManager content)
        {
            _avatarTextures = new Texture2D[10] { content.Load<Texture2D>("Textures/Interface/red_hp"),
                                                  content.Load<Texture2D>("Textures/Interface/green_hp"),
                                                  content.Load<Texture2D>("Textures/Interface/mana"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/test"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/avatar_knight"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/avatar_old_man"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/goblinAvatar"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/fireMageAvatar"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/brynolf"),
                                                  content.Load<Texture2D>("Textures/Interface/Avatars/cityGuardavatar")}; 
        }

        /// <summary>
        /// Method for drawing avatar icons and their health and mana pools
        /// </summary>
        public void DrawTargetAvatars()
        {
            Rectangle pBackgroundRect = new Rectangle(110, 30, 169, 36);

            //Player hp
            int playerHpWidth = CalculateHp(_player);
            Rectangle playerHpRect = new Rectangle(105, 47, playerHpWidth, 16);
            _spriteBatch.Draw(_avatarTextures[Convert.ToInt32(Texture.GREEN_HP)], playerHpRect, Color.White);

            //Player mana
            int playerManaWidth = CalculateMana(_player);
            Rectangle playerManaRect = new Rectangle(105, 63, playerManaWidth, 16);
            _spriteBatch.Draw(_avatarTextures[Convert.ToInt32(Texture.MANA)], playerManaRect, Color.White);

            //Healthbar
            _spriteBatch.Draw(_avatarTextures[Convert.ToInt32(Texture.AVATAR_PLAYER)], new Vector2(0, 0), Color.White);

            if (_player.Target != null)
            {
                Texture2D target;
                Texture2D hp;

                if (_player.Target.GetType() == Model.GameModel.ENEMY_NPC)
                {
                    hp = _avatarTextures[Convert.ToInt32(Texture.RED_HP)];

                    if (_player.Target.Type == Model.Enemy.CLASS_WARRIOR)
                        target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_KNIGHT)];

                    else if (_player.Target.Type == Model.Enemy.CLASS_GOBLIN)
                        target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_GOBLIN)];

                    else if (_player.Target.Type == Model.Enemy.CLASS_MAGE)
                        target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_FIREMAGE)];

                    else if (_player.Target.Type == Model.Enemy.BOSS_A)
                        target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_BRYNOLF)];

                    else
                        target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_GOBLIN)];

                }
                else if (_player.Target.Type == Model.NonPlayerCharacter.CITY_GUARD)
                {
                    target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_CITY_GUARD)];
                    hp = _avatarTextures[Convert.ToInt32(Texture.GREEN_HP)];
                }
                else
                {
                    target = _avatarTextures[Convert.ToInt32(Texture.AVATAR_OLD_MAN)];
                    hp = _avatarTextures[Convert.ToInt32(Texture.GREEN_HP)];
                }

                //Rendering target avatar
                Rectangle eBackgroundRect = new Rectangle(410, 30, 169, 36);

                //Target hp
                int enemyHpWidth = CalculateHp(_player.Target);
                Rectangle enemyHpRect = new Rectangle(407, 47, enemyHpWidth, 16);
                _spriteBatch.Draw(hp, enemyHpRect, Color.White);

                //Target mana
                if (_player.Target.GetType() == Model.GameModel.ENEMY_NPC)
                {
                    Model.Enemy enemy = _player.Target as Model.Enemy;
                    if (enemy.Type == Model.Enemy.CLASS_MAGE || enemy.Type == Model.Enemy.BOSS_A)
                    {
                        int enemyManaWidth = CalculateMana(_player.Target);
                        Rectangle enemyManaRect = new Rectangle(407, 63, enemyManaWidth, 16);
                        _spriteBatch.Draw( _avatarTextures[Convert.ToInt32(Texture.MANA)], enemyManaRect, Color.White);
                    }
                }

                _spriteBatch.Draw(target, new Vector2(300, 0), Color.White);
            }
        }
        
        /// <summary>
        /// Method for calculating of hp-pool in accordance to the hp-graphic
        /// </summary>
        /// <param name="unit">Unit object</param>
        /// <returns></returns>
        private int CalculateHp(Model.Unit unit)
        {
            //Setting the hp width to texture width
            float hpWidth = 169;

            //Checks if the unit does not have full hp
            if (unit.CurrentHp < unit.TotalHp)
            {
                //Calculates remaining hp %
                float percentHpLeft = (unit.CurrentHp / unit.TotalHp);
                //Determents the remaining % of the hp bar
                hpWidth = percentHpLeft * hpWidth;

                //Cuts 0.5 due to integer
                return (int)hpWidth;
            }
            return (int)hpWidth;
        }

        /// <summary>
        /// Method for calculating of mana-pool in accordance to the mana-graphic
        /// See "CalculateHp" for more comments within the method
        /// </summary>
        /// <param name="unit">Unit object</param>
        /// <returns></returns>
        private int CalculateMana(Model.Unit unit)
        {
            float manaWidth = 169;

            if (unit.CurrentMana < unit.TotalMana)
            {
                float percentManaLeft = (unit.CurrentMana / unit.TotalMana);
                manaWidth = percentManaLeft * manaWidth;

                return (int)manaWidth;
            }
            return (int)manaWidth;
        }

        /// <summary>
        /// Returns a texture that is also used in the UIView
        /// </summary>
        public Texture2D ManaTexture
        {
            get { return _avatarTextures[Convert.ToInt32(Texture.MANA)]; }
        }
    }
}
