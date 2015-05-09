using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// Class for updating the Player logic
    /// </summary>
    class PlayerSystem : UnitSystem
    {
        //Variables
        private Player _player;
        private SpellSystem _spellSystem;

        //Readonly properties
        public Player Player { get { return _player; } }
        public SpellSystem SpellSystem { get { return _spellSystem; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">Level object</param>
        public PlayerSystem(Level level)
        {
            this._player = new Player(level);
            this._spellSystem = new SpellSystem();
        }

        /// <summary>
        /// Main method ffor updating the player logic
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        /// <param name="enemies">List of active enemies</param>
        public void Update(float elapsedTime, List<Model.Enemy> enemies)
        {
            _player.Update();

            RegenerateMana(elapsedTime, _player);

            RegenerateHp(elapsedTime, _player);

            _spellSystem.Update(elapsedTime);

            DecreaseGlobalCD(_player, elapsedTime);

            //Untarget the players target if it is dead or evading
            if(_player.Target != null && _player.Target.GetType() == GameModel.ENEMY_NPC)
            {
                Enemy enemy = _player.Target as Enemy;
                if(!_player.Target.IsAlive() || enemy.IsEvading)
                {
                    _player.Target = null;
                    _player.IsAttacking = false;
                }
            }
            if (_player.IsAttacking)
            {
                _player.InCombat = true;
                //Checks if the player is in range of the enemy hitbox
                if (_player.Target.ThisUnit.Bounds.Intersects(_player.PlayerArea))
                {
                    _player.IsWithinMeleRange = true;
                    //Hit cooldown has ended
                    if (_player.SwingTime <= 0)
                    {
                        //If enemy is alive; reduce its hp
                        if (_player.Target.IsAlive() && _player.IsAlive())
                            _player.Target.CurrentHp -= (_player.AutohitDamage - _player.Target.Armor);
                        
                        //Setting new cooldown
                        _player.SwingTime = 50;
                    }
                    else if (!_player.IsCastingSpell)
                        _player.SwingTime -= 1;
                }
                else
                    _player.IsWithinMeleRange = false;
            }
            else 
                _player.InCombat = false;

            //If player is not alive
            if(!_player.IsAlive())
            {
                _player.Target = null;
                _player.IsAttacking = false;

                if (_player.SpawnTimer < 0)
                    _player.Spawn();
                else 
                    _player.SpawnTimer -= elapsedTime;
            }
        }
    }
}
