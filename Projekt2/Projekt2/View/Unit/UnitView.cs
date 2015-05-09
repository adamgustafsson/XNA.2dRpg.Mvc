using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace View
{
    /// <summary>
    /// Class for rendering units
    /// </summary>
    class UnitView
    {
        private Model.Player _player;
        private List<Model.Enemy> _enemies;
        private List<Model.Enemy> _deadEnemies;
        private List<Model.NonPlayerCharacter> _nonPlayerCharacters;
        private Model.QuestSystem _questSystem;

        private View.InputHandler _inputHandler;
        private View.AnimationSystem _animationSystem;
        private View.Dialog _conversation;

        private float _swordTime;
        private SpriteBatch _spriteBatch;
        private Camera _camera;
        private Texture2D[] _textures;

        /// <summary>
        /// Texture index enum
        /// </summary>
        private enum Texture
        { 
            RED_CIRCLE = 0,
            GREEN_CIRCLE = 1,
            INTERACT = 2,
            INTERACT_Q = 3,
            INTERACT_Q_COMPLETE = 4 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameModel">GameModel instance</param>
        /// <param name="spriteBatch">SpriteBatch instance</param>
        /// <param name="camera">Camera instance</param>
        /// <param name="inputHandler">InputHandler instance</param>
        /// <param name="animationSystem">AnimationSystem instance</param>
        /// <param name="dialog">Dialog instance</param>
        public UnitView(Model.GameModel gameModel, SpriteBatch spriteBatch, Camera camera, InputHandler inputHandler, AnimationSystem animationSystem, View.Dialog dialog)
        {
            this._player = gameModel.PlayerSystem.Player;
            this._enemies = gameModel.EnemySystem.Enemies;
            this._deadEnemies = gameModel.EnemySystem.SpawnList;
            this._nonPlayerCharacters = gameModel.NpcSystem.NonPlayerCharacters;
            this._questSystem = gameModel.QuestSystem;
            this._camera = camera;
            this._spriteBatch = spriteBatch;
            this._inputHandler = inputHandler;
            this._animationSystem = animationSystem;
            this._conversation = dialog;

        }

        /// <summary>
        /// Loading all unit related content
        /// </summary>
        /// <param name="content">ContentManager instance</param>
        internal void LoadContent(ContentManager content)
        {
            _textures = new Texture2D[5] {  content.Load<Texture2D>("Textures/Graphic/red_circle"),
                                             content.Load<Texture2D>("Textures/Graphic/green_circle"),
                                             content.Load<Texture2D>("Textures/Graphic/interact"),
                                             content.Load<Texture2D>("Textures/Graphic/interactQ"),
                                             content.Load<Texture2D>("Textures/Graphic/interactQcomplete")};
        }
        
        /// <summary>
        /// Main method for updating and drawin Units
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void DrawAndUpdateUnits(float elapsedTime)
        {
            _swordTime = elapsedTime;
            DrawEnemies(elapsedTime);
            DrawNPC(elapsedTime);
            DrawPlayer(elapsedTime);
        }

        /// <summary>
        /// Method for drawing the player
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        private  void DrawPlayer(float elapsedTime)
        {
            if (_player.IsAlive())
                DrawWeapon(elapsedTime, _player.UnitState, _player.WeaponState, _player);
            else
                _player.UnitState = Model.State.IS_DEAD;
      
            //Drawing player
            Vector2 playerPosition = _camera.VisualizeCordinates(_player.ThisUnit.Bounds.X, _player.ThisUnit.Bounds.Y);
            _animationSystem.UpdateAndDraw(elapsedTime, Color.White, playerPosition, _player.UnitState, AnimationSystem.Texture.PLAYER);

            //Armor
            if (_player.HasHelm)
                DrawArmor(elapsedTime, _player, _player.UnitState, AnimationSystem.Texture.ARMOR_HEAD);
        }

        /// <summary>
        /// Method for drawing wepons
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="unitState">Unit state</param>
        /// <param name="weaponAnimation">Wepon animation</param>
        /// <param name="unit">Unit object</param>
        private void DrawWeapon(float elapsedTime, Model.State unitState, Model.State weaponAnimation, Model.Unit unit)
        {
            //Drawing wepons
            if (unit.IsAttacking && !unit.IsCastingSpell)// && m_player.IsAlive())//Player is alive: bugfix, not needed
            {
                if (weaponAnimation > 0)
                {
                    #region Displacement

                    int displacementX = 0;
                    int displacementY = 0;

                    if (weaponAnimation == Model.State.FACING_CAMERA ||
                        weaponAnimation == Model.State.MOVING_DOWN)
                    {
                        displacementX = -11;
                        displacementY = 25;
                    
                        if (unit.GetType() == Model.GameModel.ENEMY_NPC)
                            displacementX = -5;
                    }
                    else if (weaponAnimation == Model.State.MOVING_UP)
                    {
                        displacementX = +11;
                        displacementY = -10;
                    }
                    else if (weaponAnimation == Model.State.MOVING_LEFT)
                    {
                        displacementX = -20;
                        displacementY = +5;

                        if (unit.GetType() == Model.GameModel.ENEMY_NPC)
                        {
                            displacementX = -10;
                            displacementY = +12;
                        }
                    }
                    else
                    {
                        displacementX = +20;
                        displacementY = +5;
                    
                        if (unit.GetType() == Model.GameModel.ENEMY_NPC)
                        {
                            displacementX = +22;
                            displacementY = +15;
                        }
                    }
                    #endregion

                    Vector2 swordPosition = _camera.VisualizeCordinates(unit.ThisUnit.Bounds.X + displacementX, unit.ThisUnit.Bounds.Y + displacementY);
                    _animationSystem.UpdateAndDraw(_swordTime, Color.White, swordPosition, weaponAnimation, AnimationSystem.Texture.WEAPON_SWORD);
                }
            }
            _swordTime = 0;
        }

        /// <summary>
        /// Method for drawing player armor
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="player">Player object</param>
        /// <param name="unitState">Unit state</param>
        /// <param name="armorTextureIndex">Texture index for the armor</param>
        private void DrawArmor(float elapsedTime, Model.Player player, Model.State unitState, AnimationSystem.Texture armorTextureIndex)
        {
            Vector2 armorPlacement = Vector2.Zero;

            switch (armorTextureIndex)
            {
                case AnimationSystem.Texture.ARMOR_HEAD:
                    armorPlacement = _camera.VisualizeCordinates(player.ThisUnit.Bounds.X, player.ThisUnit.Bounds.Y);
                    break;
            }

            Model.State armorAnimation;

            if (unitState == Model.State.MOVING_DOWN)
                armorAnimation = Model.State.FACING_CAMERA;

            else if (unitState == Model.State.MOVING_UP)
                armorAnimation = Model.State.FACING_AWAY;

            else if (unitState == Model.State.MOVING_LEFT)
                armorAnimation = Model.State.FACING_LEFT;

            else if (unitState == Model.State.MOVING_RIGHT)
                armorAnimation = Model.State.FACING_RIGHT;

            else if (unitState == Model.State.IS_CASTING_HEAL)
                armorAnimation = Model.State.FACING_CAMERA;

            else
                armorAnimation = unitState;

            _animationSystem.UpdateAndDraw(elapsedTime, Color.White, armorPlacement, armorAnimation, AnimationSystem.Texture.ARMOR_HEAD);
        }

        /// <summary>
        /// Method for drawing NPCs
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        private void DrawNPC(float elapsedTime)
        {
            foreach (Model.NonPlayerCharacter friend in _nonPlayerCharacters)
            {
                //Checks wheter the NPC occurs within the camera screen
                if (friend.ThisUnit.Bounds.Intersects(_camera.GetScreenRectangle))
                {
                    friend.CanAddToQuest = true;

                    //Setting the NPC as target on click
                    if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(friend.ThisUnit.Bounds)) ||
                        _inputHandler.DidGetTargetedByRightClick(_camera.VisualizeRectangle(friend.ThisUnit.Bounds)))
                    {
                        _player.Target = friend;
                    }

                    Vector2 interactPosition = _camera.VisualizeCordinates(friend.ThisUnit.Bounds.X + 10, friend.ThisUnit.Bounds.Y - 24);
                    int bubble = -1;

                    //Drawing dialog box if the NPC ID excists in the XML file for Dialogs
                    if (_conversation.DialogueList.Exists(Message => Message.id == friend.UnitId))
                    {
                        friend.CanInterract = true;
                        bubble = Convert.ToInt32(Texture.INTERACT);
                    }
                    else if ((_questSystem.QuestList.Exists(Quest => _questSystem.ActiveNpc == friend.UnitId && Quest.Id == _questSystem.CurrentQuest.Id)))
                    {
                        friend.CanInterract = true;

                        if (_questSystem.CurrentQuest.Status == Model.QuestSystem.END)
                            bubble = Convert.ToInt32(Texture.INTERACT_Q_COMPLETE);
                        else if (_questSystem.CurrentQuest.Status == Model.QuestSystem.PRE)
                            bubble = Convert.ToInt32(Texture.INTERACT_Q);
                        else
                            bubble = Convert.ToInt32(Texture.INTERACT);
                    }
                    else
                        friend.CanInterract = false;

                    if (friend.CanInterract)
                        _spriteBatch.Draw(_textures[bubble], new Rectangle((int)interactPosition.X, (int)interactPosition.Y, 30, 30), Color.White);

                    //Drawing NPC
                    Vector2 npcPosition = _camera.VisualizeCordinates(friend.ThisUnit.Bounds.X + 8, friend.ThisUnit.Bounds.Y + 8);
                    AnimationSystem.Texture animation = 0;

                    if (friend.Type == Model.NonPlayerCharacter.OLD_MAN)
                        animation = AnimationSystem.Texture.OLD_MAN;
                    else
                        animation = AnimationSystem.Texture.CITY_GUARD;
                  
                    //Drawing target circle
                    DrawTargetCircle(_player, friend);
                    _animationSystem.UpdateAndDraw(elapsedTime, Color.White, npcPosition, friend.UnitState, animation);
                }
                else
                    friend.CanAddToQuest = false;
            }

        }

        /// <summary>
        /// Method for drawing enemies
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        private void DrawEnemies(float elapsedTime)
        {
            float mageTime = elapsedTime;
            float warriorTime = elapsedTime;
            float swordTime = elapsedTime;
            float goblinTime = elapsedTime;
            float bossATime = elapsedTime;

            _inputHandler.MouseIsOverEnemy = false;

            //Drawing dead enemies
            foreach (Model.Enemy deadEnemy in _deadEnemies)
            {
                Vector2 enemyPosition = _camera.VisualizeCordinates(deadEnemy.ThisUnit.Bounds.X + 8, deadEnemy.ThisUnit.Bounds.Y + 8);

                if (_inputHandler.MouseIsOver(new Rectangle((int)enemyPosition.X - 8, (int)enemyPosition.Y - 8, deadEnemy.ThisUnit.Bounds.Width, deadEnemy.ThisUnit.Bounds.Height))
                    && !_inputHandler.MouseIsOverInterface)
                {
                    _inputHandler.MouseIsOverLoot= true;
                }

                if (deadEnemy.Type == Model.Enemy.BOSS_A)
                    _animationSystem.UpdateAndDraw(bossATime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.Texture.BOSS_A);

                else if (deadEnemy.Type == Model.Enemy.CLASS_GOBLIN)
                    _animationSystem.UpdateAndDraw(goblinTime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.Texture.ENEMY_GOBLIN);

                else if (deadEnemy.Type == Model.Enemy.CLASS_MAGE)
                    _animationSystem.UpdateAndDraw(mageTime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.Texture.ENEMY_MAGE);

                else if (deadEnemy.Type == Model.Enemy.CLASS_WARRIOR)
                    _animationSystem.UpdateAndDraw(warriorTime, Color.White, enemyPosition, deadEnemy.UnitState, AnimationSystem.Texture.ENEMY_KNIGHT);

                if (_inputHandler.DidGetTargetedByLeftClick(new Rectangle((int)enemyPosition.X, (int)enemyPosition.Y, deadEnemy.ThisUnit.Bounds.Width, deadEnemy.ThisUnit.Bounds.Height)) & !_inputHandler.MouseIsOverInterface)
                {
                    //Checks if the player is in loot range
                    if (_player.ThisUnit.Bounds.Intersects(deadEnemy.ThisUnit.Bounds))
                        _player.LootTarget = deadEnemy;
                }
            }

            //m_enemies.OrderByDescending(Enemy => Enemy.ThisUnit.Bounds.Y);
            //Drawing all enemies
            foreach (Model.Enemy enemy in _enemies)
            {
                Vector2 enemyPosition = _camera.VisualizeCordinates(enemy.ThisUnit.Bounds.X + 8, enemy.ThisUnit.Bounds.Y + 8);
               
                //If mouse is hovering an enemiy
                if(_inputHandler.MouseIsOver(new Rectangle((int)enemyPosition.X - 8, (int)enemyPosition.Y - 8, enemy.ThisUnit.Bounds.Width, enemy.ThisUnit.Bounds.Height))
                    && !_inputHandler.MouseIsOverInterface)
                {
                    _inputHandler.MouseIsOverEnemy = true;
                }

                if(_player.Target == enemy)
                    DrawTargetCircle(_player, enemy);

                if (enemy.IsAttacking && (enemy.Type == Model.Enemy.CLASS_WARRIOR)) //|| enemy.Type == Model.Enemy.BOSS_A))
                {
                    DrawWeapon(swordTime, enemy.UnitState, enemy.WeaponState, enemy);
                    //BUGFIX.
                    swordTime = 0;
                }
                if (enemy.ThisUnit.Bounds.Intersects(_camera.GetScreenRectangle))
                {
                    //If the enemy occurs on the screen it is active
                    enemy.IsActive = true;

                    //If the enemy is targeted
                    if (_inputHandler.DidGetTargetedByLeftClick(_camera.VisualizeRectangle(enemy.ThisUnit.Bounds)) && !_inputHandler.MouseIsOverInterface)
                    {
                        _player.Target = enemy;
                        _player.IsAttacking = true;
                    }

                    //Drawing enemies that is alive
                    if (enemy.IsAlive())
                    {
                        if (enemy.Type == Model.Enemy.CLASS_WARRIOR)
                        {
                            _animationSystem.UpdateAndDraw(warriorTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.ENEMY_KNIGHT);

                            if (enemy.IsAttacking || enemy.IsEvading)
                                warriorTime = 0;
                        }
                        else if (enemy.Type == Model.Enemy.CLASS_MAGE)
                        {
                            _animationSystem.UpdateAndDraw(mageTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.ENEMY_MAGE);

                            if (enemy.IsAttacking || enemy.IsEvading)
                                mageTime = 0;
                        }
                        else if (enemy.Type == Model.Enemy.CLASS_GOBLIN)
                        {
                            _animationSystem.UpdateAndDraw(goblinTime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.ENEMY_GOBLIN);

                            if (enemy.IsAttacking || enemy.IsEvading)
                                goblinTime = 0;
                        }
                        else if (enemy.Type == Model.Enemy.BOSS_A)
                        {
                            _animationSystem.UpdateAndDraw(bossATime, Color.White, enemyPosition, enemy.UnitState, AnimationSystem.Texture.BOSS_A);

                            if (enemy.IsAttacking || enemy.IsEvading)
                                bossATime = 0;
                        }
                    }
                }
                else
                    enemy.IsActive = false;
            }
        }

        /// <summary>
        /// Method for drawing the unit target circle
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="unit">Unit object</param>
        private void DrawTargetCircle(Model.Player player, Model.Unit unit)
        {
            Vector2 position = _camera.VisualizeCordinates(unit.ThisUnit.Bounds.X + 1, unit.ThisUnit.Bounds.Y + 28);

            if (player.Target == unit)
            {
                //Enemies
                if (unit.GetType() == Model.GameModel.ENEMY_NPC)
                    _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.RED_CIRCLE)], position, Color.White);
                //NPCs
                if (unit.GetType() == Model.GameModel.FRIENDLY_NPC)
                    _spriteBatch.Draw(_textures[Convert.ToInt32(Texture.GREEN_CIRCLE)], position, Color.White);
            }
        }
    }
}
