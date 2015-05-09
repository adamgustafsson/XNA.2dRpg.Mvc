using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace View
{
    /// <summary>
    /// Main-class for handling all game sprite animations
    /// </summary>
    class AnimationSystem
    {
        private Animation[] _spriteTextures;
        private SpriteBatch _spriteBatch;
        private const float _rotation = 0;
        private const float _depth = 0.5f;
        private const int _framesX = 3;
        private const int _framesY = 4;

        /// <summary>
        /// Texture enum
        /// </summary>
        internal enum Texture
        {
            //Player
            PLAYER = 0,
            CLASS_SCREEN_TEMPLAR = 1,
            CLASS_SCREEN_DESCENDANT = 2,
            CLASS_SCREEN_PROPHET = 3,
            //Enemy
            ENEMY_KNIGHT = 4,
            ENEMY_MAGE = 5,
            ENEMY_GOBLIN = 6,
            BOSS_A = 7,
            //Npc 
            OLD_MAN = 8,
            CITY_GUARD = 9,
            //Items and weapons
            ARMOR_HEAD = 10,    
            ITEM_PURPLE_CHEST = 11,     
            WEAPON_SWORD = 12,
            //Effects
            FIREBALL_TEXTURE = 13,
            SMITE_TEXTURE = 14
        };

        /// <summary>
        /// Constructor - Texture enum must correspond to the texture-array index
        /// </summary>
        /// <param name="spriteBatch"></param>
        public AnimationSystem(SpriteBatch spriteBatch)  
        {
            this._spriteBatch = spriteBatch;
            this._spriteTextures = new Animation[15] {   
                                                    //Player    
                                                    new Animation("player", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, 7, 6),
                                                    new Animation("templar", Vector2.Zero, _rotation, 2.0f, _depth, _framesX, _framesY, 4),
                                                    new Animation("descendant", Vector2.Zero, _rotation, 2.0f, _depth, _framesX, _framesY, 4),
                                                    new Animation("prophet", Vector2.Zero, _rotation, 2.0f, _depth, _framesX, _framesY, 4),
                                                    //Enemies
                                                    new Animation("enemy_knight", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, 6, 6),
                                                    new Animation("enemyfiremage", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, 6, 4),
                                                    new Animation("goblin", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, 6, 5),
                                                    new Animation("enemyboss", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, 7, 5),
                                                    //Npc
                                                    new Animation("npc_old_man", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, _framesY, 0),
                                                    new Animation("cityGuard", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, _framesY, 4),
                                                    //Items
                                                    new Animation("headArmor3", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, _framesY, 5),
                                                    new Animation("PurpleChest", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, _framesY, 1),
                                                    new Animation("swordC", Vector2.Zero, _rotation, 1.5f, _depth, _framesX, _framesY, 6),
                                                    //Effects
                                                    new Animation("fireball2", Vector2.Zero,  _rotation, 1.5f, _depth, 4, 1, 7),
                                                    new Animation("smite", Vector2.Zero,  _rotation, 2.0f, _depth, _framesX, 1, 4)};
        }

        /// <summary>
        /// Loading the animations
        /// </summary>
        /// <param name="content"></param>
        internal void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            foreach(Animation sprite in _spriteTextures)
                sprite.Load(content);
        }

        /// <summary>
        /// Updating and drawing via UpdateFrame and DrawFrame
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="texturePos">Screen position</param>
        /// <param name="color">Color(fading)</param>
        /// <param name="animation">Animation/Object-state</param>
        /// <param name="texture">Texture index</param>
        internal void UpdateAndDraw(float elapsedTime, Color color, Vector2 texturePos, Model.State animation, Texture texture)
        {
            int frameY = -1;
            int frameX = -1;
            bool staticAnimation = false;
            bool verticalAnimation = false;

            switch (animation)
            {
                case Model.State.MOVING_DOWN:
                    frameY = 0;
                    break;
                case Model.State.SMITE:
                    frameY = 0;
                    break;
                case Model.State.MOVING_UP:
                    frameY = 3;
                    break;
                case Model.State.MOVING_RIGHT:
                    frameY = 2;
                    break;
                case Model.State.MOVING_LEFT:
                    frameY = 1;
                    break;
                case Model.State.FACING_CAMERA:
                    frameY = 0;
                    staticAnimation = true;
                    break;
                case Model.State.FACING_LEFT:
                    frameY = 1;
                    staticAnimation = true;
                    break;
                case Model.State.FACING_RIGHT:
                    frameY = 2;
                    staticAnimation = true;
                    break;
                case Model.State.FACING_AWAY:
                    frameY = 3;
                    staticAnimation = true;
                    break;
                case Model.State.IS_DEAD:
                    frameY = 5;
                    staticAnimation = true;
                    break;
                case Model.State.WAS_HEALED:
                    break;
                case Model.State.IS_CASTING_FIREBALL:
                    frameY = 4;
                    break;
                case Model.State.IS_CASTING_HEAL:
                    frameY = 6;
                    break;
                case Model.State.VERTICAL_ANIMATION:
                    frameX = 1;
                    verticalAnimation = true;
                    break;
            }

            if (staticAnimation)
                _spriteTextures[Convert.ToInt32(texture)].StaticTexture(frameY);             
            else if (verticalAnimation)
                _spriteTextures[Convert.ToInt32(texture)].VerticalAnimation(elapsedTime, frameX);
            else
                _spriteTextures[Convert.ToInt32(texture)].HorizontalAnimation(elapsedTime, frameY); 

            _spriteTextures[Convert.ToInt32(texture)].DrawFrame(_spriteBatch, texturePos, color);
        }
    }
}
