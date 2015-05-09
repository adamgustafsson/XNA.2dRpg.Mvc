using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Class for updating enemy logic
    /// </summary>
    class EnemySystem : UnitSystem
    {
        //Variables
        public SpellSystem _enemySpellSystem;
        private Random _random = new Random(); //Random object used for randomizing a target point on the player object to avoid clustering
        private int _enemyId; //Counter for enemie Id:s used for loading _enemies from the map

        private List<Model.Enemy> _enemies; //Alive _enemies
        private List<Model.Enemy> _spawnList; //Dead _enemies

        //Readonly properties
        public List<Model.Enemy> Enemies { get { return _enemies; } }
        public List<Model.Enemy> SpawnList { get { return _spawnList; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enemyLayer">The TMX object layer "EnemyLayer"</param>
        /// <param name="level">Level object</param>
        /// <param name="currentMap">Current Map object</param>
        public EnemySystem(ObjectLayer enemyLayer, Level level, Map currentMap)
        {
            this._enemies = new List<Model.Enemy>();
            this._spawnList = new List<Enemy>();
            this._enemySpellSystem = new SpellSystem();
            this._enemyId = 0;
            
            Load_enemies(enemyLayer, level, currentMap);
        }

        /// <summary>
        /// Method for loading _enemies from the TMX map
        /// </summary>
        /// <param name="enemyLayer">The TMX object layer "EnemyLayer"</param>
        /// <param name="level">Level object</param>
        /// <param name="currentMap">Current Map object</param>
        private void Load_enemies(ObjectLayer enemyLayer, Level level, Map currentMap)
        {
            foreach (MapObject enemy in enemyLayer.MapObjects)
            {
                _enemyId++;
                //Assigning the enemy an enemyZone from the TMX map
                foreach (var enemyZone in level.EnemyZoneLayer.MapObjects)
                {
                    if(enemy.Bounds.Intersects(enemyZone.Bounds))
                        _enemies.Add(new Enemy(enemy, Convert.ToInt32(enemy.Properties["Type"].AsInt32), _enemyId, enemyZone.Bounds));
                }
            }
        }

        /// <summary>
        /// Main method for updating the _enemies logic
        /// </summary>
        /// <param name="player">The player object</param>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        public void Update(Player player, float elapsedTime)
        {
            _enemySpellSystem.Update(elapsedTime);
            //Checks the list with dead _enemies and handles their respawn
            Enemy_spawnList(player);
            //Set to false here and true whithin the loop if an enemy is attacking the player
            player.InCombat = false;

            foreach (Model.Enemy enemy in _enemies)
            {          
                if (enemy.Target != null)
                {
                    //If the enemy target is dead; stop attacking and evade to it's starting position
                    if (!enemy.Target.IsAlive())
                    {
                        enemy.IsEvading = true;
                        enemy.IsAttacking = false;
                        enemy.Target = null;
                        enemy.IsCastingSpell = false;
                        MoveToTarget(enemy, Vector2.Zero);
                    }
                }

                DecreaseGlobalCD(enemy, elapsedTime);

                if (enemy.Type == Enemy.CLASS_MAGE || enemy.Type == Enemy.BOSS_A)
                    RegenerateMana(elapsedTime, enemy);

                if (enemy.IsActive && enemy.ThisUnit.Bounds.Intersects(enemy.EnemyZone) && !enemy.IsEvading)
                {
                    if (enemy.IsAttacking)
                        EnemyAttack(enemy, player);
                    else
                        CheckEnemyAggro(enemy, player);
                }
                else
                {
                    enemy.IsEvading = true;
                    enemy.IsAttacking = false;
                }

                if (enemy.IsEvading)
                    EnemyEvade(enemy);
                
                //Adds the enemy to the _spawnList if he got killed
                AddTo_spawnList(enemy);
            }

            _enemies.RemoveAll(Enemy => !Enemy.IsAlive());
            _enemies.OrderByDescending(Enemy => Enemy.ThisUnit.Bounds.Y);
        }

        /// <summary>
        /// Checks wheter the player is within the enemy aggro range
        /// </summary>
        /// <param name="enemy">Enemy object</param>
        /// <param name="player">Player object</param>
        private void CheckEnemyAggro(Enemy enemy, Player player)
        {
            enemy.UnitState = Model.State.FACING_CAMERA;

            int aggroRange = SetAggroRange(enemy);

            if (player.ThisUnit.Bounds.Center.X > enemy.ThisUnit.Bounds.Center.X - aggroRange && player.ThisUnit.Bounds.Center.X < enemy.ThisUnit.Bounds.Center.X + aggroRange)
            {
                if ((player.ThisUnit.Bounds.Center.Y > enemy.ThisUnit.Bounds.Center.Y - aggroRange && player.ThisUnit.Bounds.Center.Y < enemy.ThisUnit.Bounds.Center.Y + aggroRange) && player.IsAlive())
                {
                    enemy.TargetDisLocationX = _random.Next(0, 49);
                    enemy.TargetDisLocationY = _random.Next(0, 49);
                    enemy.Target = player;
                    enemy.IsAttacking = true;
                }
            }
        }

        /// <summary>
        /// Method for generating all enemy attacks
        /// </summary>
        /// <param name="enemy">Enemy object</param>
        /// <param name="player">Player object</param>
        private void EnemyAttack(Enemy enemy, Player player)
        {
            player.InCombat = true;

            //If the enemy is a mage and is able to cast a fireball spell
            if (enemy.Type == Model.Enemy.CLASS_MAGE && !enemy.IsCastingSpell)
            {
                if (_enemySpellSystem.CanCastSpell(SpellSystem.FIRE_BALL, enemy))
                    _enemySpellSystem.AddSpell(SpellSystem.FIRE_BALL, enemy);
            }
            //If the enemy is of boss type A and is able to cast a fireball or healing spell
            if (enemy.Type == Model.Enemy.BOSS_A && !enemy.IsCastingSpell)
            {
                //Casting healing spell if hp is below half
                if (enemy.CurrentHp < enemy.TotalHp / 2 && _enemySpellSystem.CanCastSpell(SpellSystem.INSTANT_HEAL, enemy) && enemy.Type == Model.Enemy.BOSS_A)
                    _enemySpellSystem.AddSpell(SpellSystem.INSTANT_HEAL, enemy);
                //Else; casting fireball spell
                else if (_enemySpellSystem.CanCastSpell(SpellSystem.FIRE_BALL, enemy))
                    _enemySpellSystem.AddSpell(SpellSystem.FIRE_BALL, enemy);
            }
            //If the enemy is not casting it is able to move
            if (!enemy.IsCastingSpell)
            {
                MoveToTarget(enemy, new Vector2(enemy.Target.ThisUnit.Bounds.X, enemy.Target.ThisUnit.Bounds.Y));
                //Autohiting
                if (enemy.ThisUnit.Bounds.Intersects(player.PlayerArea) && enemy.IsAlive())
                {
                    if (enemy.SwingTime < 0)
                    {
                        //Reduce dmg in accordance to player armor stats
                        player.CurrentHp -= (enemy.AutohitDamage - player.Armor);

                        //Make sure that the hp does not go over hp maximum
                        if (player.CurrentHp > player.TotalHp)
                            player.CurrentHp = player.TotalHp;

                        enemy.SwingTime = 20;
                    }
                    else
                        enemy.SwingTime -= 1;
                }
            }
        }

        /// <summary>
        /// Method for handling the enemy evade logic
        /// </summary>
        /// <param name="enemy">An enemy object</param>
        private void EnemyEvade(Enemy enemy)
        {
            //Moves back to spawnlocation if it is not attacking
            if (enemy.ThisUnit.Bounds.Location != enemy.SpawnPosition && !enemy.IsAttacking)
            {
                //Evading
                enemy.CurrentHp = enemy.TotalHp;
                MoveToTarget(enemy, new Vector2(enemy.SpawnPosition.X, enemy.SpawnPosition.Y));
            }
            else if (!enemy.IsAttacking)
                enemy.IsEvading = false;
        }

        /// <summary>
        /// Sets an spawntimer and add the enemy to the spawn list
        /// </summary>
        /// <param name="enemy">An enemy object</param>
        private void AddTo_spawnList(Enemy enemy)
        {
            //TODO: Use elapsed time?
            if (!enemy.IsAlive())
            {
                if (enemy.Type == Model.Enemy.CLASS_GOBLIN)
                    enemy.SpawnTimer = 2000;
                else
                    enemy.SpawnTimer = 10000;

                _spawnList.Add(enemy);
                enemy.UnitState = State.IS_DEAD;
            }
        }

        /// <summary>
        /// Sets the aggro range to a given enemy
        /// </summary>
        /// <param name="enemy">An enemy object</param>
        /// <returns>The aggro range as integer</returns>
        private int SetAggroRange(Enemy enemy)
        {
            if (enemy.Type == Enemy.CLASS_WARRIOR)
                return 200;

            else if (enemy.Type == Enemy.CLASS_MAGE)
                return 300;

            else if (enemy.Type == Enemy.CLASS_GOBLIN)
                return 200;
            //Default
            else
                return 200;
        }
        
        /// <summary>
        /// Handles the enemy movements
        /// </summary>
        /// <param name="enemy">An enemy object</param>
        /// <param name="target">Move to target location</param>
        private void MoveToTarget(Enemy enemy, Vector2 target)
        {
            Vector2 moveTo = target;
            Vector2 newCords = Vector2.Zero;

            float xSpeed = 0;
            float ySpeed = 0;

            if (enemy.IsAttacking)
            {
                moveTo.X = enemy.Target.ThisUnit.Bounds.X + enemy.TargetDisLocationX;
                moveTo.Y = enemy.Target.ThisUnit.Bounds.Y + enemy.TargetDisLocationY;
            }
            if (enemy.IsEvading)
            {
                moveTo.X = enemy.SpawnPosition.X;
                moveTo.Y = enemy.SpawnPosition.Y;
            }

            enemy.Direction = new Vector2(moveTo.X - enemy.ThisUnit.Bounds.Center.X, moveTo.Y - enemy.ThisUnit.Bounds.Center.Y);

            xSpeed = Math.Abs(enemy.Direction.X);
            ySpeed = Math.Abs(enemy.Direction.Y);

            if (!ArrivedToPosition(enemy.ThisUnit.Bounds, moveTo, 5) &&  (enemy.Target != null && !enemy.ThisUnit.Bounds.Intersects(enemy.Target.ThisUnit.Bounds) || enemy.IsEvading))
            {
                newCords = enemy.Direction;
                newCords.Normalize();
                newCords.X = newCords.X * enemy.MoveSpeed;
                newCords.Y = newCords.Y * enemy.MoveSpeed;
                xSpeed = Math.Abs(enemy.Direction.X);
                ySpeed = Math.Abs(enemy.Direction.Y);

                enemy.ThisUnit.Bounds.X += (int)newCords.X;
                enemy.ThisUnit.Bounds.Y += (int)newCords.Y;

                if (xSpeed > ySpeed)
                {
                    if (enemy.Direction.X > 0f)
                    {
                        enemy.UnitState = Model.State.MOVING_RIGHT;
                        enemy.WeaponState = enemy.UnitState;
                    }
                    else
                    {
                        enemy.UnitState = Model.State.MOVING_LEFT;
                        enemy.WeaponState = enemy.UnitState;
                    }
                }
                else
                {
                    if (enemy.Direction.Y > 0f)
                    {
                        enemy.UnitState = Model.State.MOVING_DOWN;
                        enemy.WeaponState = enemy.UnitState;
                    }
                    else
                    {
                        enemy.UnitState = Model.State.MOVING_UP;
                        enemy.WeaponState = enemy.UnitState;
                    }
                }

            }
            else
            {
                if (xSpeed > ySpeed)
                {
                    if (enemy.Direction.X > 0f)
                    {
                        enemy.UnitState = Model.State.FACING_RIGHT;
                        enemy.WeaponState = Model.State.MOVING_RIGHT;
                    }
                    else
                    {
                        enemy.UnitState = Model.State.FACING_LEFT;
                        enemy.WeaponState = Model.State.MOVING_LEFT;
                    }
                }
                else
                {
                    if (enemy.Direction.Y > 0f)
                    {
                        enemy.UnitState = Model.State.FACING_CAMERA;
                        enemy.WeaponState = Model.State.MOVING_DOWN;
                    }
                    else
                    {
                        enemy.UnitState = Model.State.FACING_AWAY;
                        enemy.WeaponState = Model.State.MOVING_UP;
                    }
                }
                enemy.IsEvading = false;
            }

        }

        /// <summary>
        /// Checks the list with dead _enemies and handles their respawn
        /// </summary>
        /// <param name="player">Player object</param>
        private void Enemy_spawnList(Player player)
        {
            if (_spawnList.Count > 0)
            {
                for (int i = 0; i < _spawnList.Count; i++)
                {
                    if (_spawnList[i].WaitingToSpawn())
                        _spawnList[i].SpawnTimer --;
                    else
                    {
                        //If enemy was dead and spawned; it is not lootable
                        if (_spawnList[i] == player.LootTarget)
                            player.LootTarget = null;

                        //Respawning enemy
                        MapObject mapObj = _spawnList[i].ThisUnit;
                        Rectangle spawnPos = new Rectangle(_spawnList[i].SpawnPosition.X, _spawnList[i].SpawnPosition.Y, _spawnList[i].ThisUnit.Bounds.Width, _spawnList[i].ThisUnit.Bounds.Height);
                        mapObj.Bounds = spawnPos;
                        Enemy enemy = new Enemy(mapObj, _spawnList[i].Type, _spawnList[i].UnitId, _spawnList[i].EnemyZone);
                        _enemies.Add(enemy);
                        _spawnList.Remove(_spawnList[i]);
                    }
                }
            }
        }
    }
}
