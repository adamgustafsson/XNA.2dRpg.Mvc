using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuncWorks.XNA.XTiled;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Model
{
    /// <summary>
    /// Class for updating all spell logic
    /// </summary>
    class SpellSystem
    {
        private List<Spell> _activeSpells;
        private float _globalCd = 0.2f;
        
        //Type-references
        public static Type INSTANT_HEAL = typeof(Model.InstantHeal);
        public static Type FIRE_BALL = typeof(Model.Fireball);
        public static Type SMITE = typeof(Model.Smite);

        /// <summary>
        /// Constructor
        /// </summary>
        public SpellSystem()
        { 
            _activeSpells = new List<Spell>();
        }

        /// <summary>
        /// Updates the spell logic
        /// </summary>
        /// <param name="elapsedTime">Elapsed time in milleseconds</param>
        internal void Update(float elapsedTime)
        {
            //Clearing the spell-list from spells where:
            _activeSpells.RemoveAll(Spell => (Spell.Duration == 0 && Spell.CoolDown <= 0) ||                      //Duration and Cooldown is zero
                                              (!Spell.Caster.IsAlive()) ||                                        //Caster is dead
                                              (Spell.Caster.Target != null && (Spell.GetType() == FIRE_BALL && !Spell.Caster.Target.IsAlive())) ||
                                              (Spell.GetType() == FIRE_BALL && Spell.Caster.Target == null));     //Fireball target is dead or null

            //Uppdating active Instant heals
            foreach (Spell spell in _activeSpells)
            { 
                #region InstanHeal
                if(spell.GetType() == SpellSystem.INSTANT_HEAL)
                {
                    InstantHeal instantHeal = spell as Model.InstantHeal;

                    //If cast time is done and the spell is not yet started
                    if (instantHeal.CastTime <= 0 && instantHeal.Duration != 0)
                    {
                        //Cast heal
                        instantHeal.Caster.IsCastingSpell = false;
                        CastInstantHeal(instantHeal);
                    }
                    //Else if cast time exists: reduce it
                    else if (spell.CastTime > 0)
                    {
                        if(spell.Caster.GetType() == GameModel.ENEMY_NPC)
                            spell.Caster.UnitState = Model.State.IS_CASTING_HEAL;

                        instantHeal.CastTime -= elapsedTime;
                    }
                    //Else, reduce spell cd     
                    else
                        instantHeal.CoolDown -= elapsedTime;
                } 
                #endregion
                
                #region FireBall
                else if(spell.GetType() == SpellSystem.FIRE_BALL)
                {
                    Fireball fireBall = spell as Model.Fireball;
                    //Updating fireball target.
                    fireBall.Update(spell.Caster.Target);

                    //If casttimem is done whilst the spell have not yet hit the target
                    if (fireBall.CastTime <= 0 && spell.Duration != 0)
                    {
                        //Set status to "was casted"
                        if(!fireBall.WasCasted)
                        {
                            CastFireBall(fireBall);
                            fireBall.WasCasted = true;
                        }

                        //If the spell hit its target
                        if (fireBall.Target.ThisUnit.Bounds.Intersects(fireBall.FireBallArea) && fireBall.Duration > 0)
                        {
                            //Do dmg
                            fireBall.Caster.Target.CurrentHp -= (int)fireBall.Damage + fireBall.Caster.SpellPower;
                            //Declare spell hit
                            spell.Duration = 0;
                        }

                        //Updating spell
                        fireBall.Direction = new Vector2(fireBall.Target.ThisUnit.Bounds.X, fireBall.Target.ThisUnit.Bounds.Y) - fireBall.Position;
                        Vector2 newCordinates = new Vector2();
                        newCordinates = fireBall.Direction;
                        newCordinates.Normalize();
                        fireBall.Position += newCordinates * 5;
                        fireBall.Caster.IsCastingSpell = false;
                    }
                    //If cast time exists: reduce it
                    if (spell.CastTime > 0)
                    {
                        //Whilst player is casting: Facing camera
                        spell.Caster.UnitState = Model.State.IS_CASTING_FIREBALL;
                        fireBall.CastTime -= elapsedTime;
                    }
                    //Else, reduce cd  
                    else
                        fireBall.CoolDown -= elapsedTime;
                }
                #endregion

                #region Smite

                else if (spell.GetType() == SpellSystem.SMITE)
                {
                    Smite smite = spell as Model.Smite;

                    if (smite.CastTime <= 0 && spell.Duration != 0)
                        CastSmite(smite);

                    if (spell.CastTime > 0)
                    {
                        if (spell.Caster.GetType() == GameModel.PLAYER)
                            spell.Caster.UnitState = State.FACING_CAMERA;

                        smite.CastTime -= elapsedTime;
                    }  
                    else
                        smite.CoolDown -= elapsedTime;
                }
                #endregion
            } 
        }

        /// <summary>
        /// Methoid for casting InstantHeal
        /// </summary>
        /// <param name="instantHeal">InstantHeal object</param>
        public void CastInstantHeal(InstantHeal instantHeal)
        {
            //Setting caster global cd
            instantHeal.Caster.GlobalCooldown = _globalCd;

            //Heals
            instantHeal.Caster.CurrentHp += (int)instantHeal.Heal + instantHeal.Caster.SpellPower;

            //Reduce caster mana
            instantHeal.Caster.CurrentMana -= instantHeal.ManaCost;

            //Makes sure the HP doesnt exceed the maximum HP
            if (instantHeal.Caster.CurrentHp > instantHeal.Caster.TotalHp)
                instantHeal.Caster.CurrentHp = instantHeal.Caster.TotalHp;

            //Declares that the spell update is finished
            instantHeal.Duration = 0;
        }

        /// <summary>
        /// Methoid for casting FireBall
        /// </summary>
        /// <param name="fireBall">Fireball object</param>
        public void CastFireBall(Fireball fireBall)
        {
            //Setting casters global cd
            fireBall.Caster.GlobalCooldown = _globalCd;
            //Reducing caster mana
            fireBall.Caster.CurrentMana -= fireBall.ManaCost;
        }

        /// <summary>
        /// Methoid for casting Smite
        /// </summary>
        /// <param name="smite">Smite object</param>
        public void CastSmite(Smite smite)
        {
            smite.Caster.GlobalCooldown = _globalCd;

            //Smite dmg
            if(smite.Target != null)
            {
                //Makes sure the target hit by Smite begins to attack
                smite.Target.Target = smite.Caster;
                smite.Target.IsAttacking = true;
                smite.Target.CurrentHp -= smite.Damage;
                //Reduce caster mana
                smite.Caster.CurrentMana -= smite.ManaCost;
            }

            //Declares that the current spell waas updated
            smite.Duration = 0;
            smite.Caster.IsCastingSpell = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellType"></param>
        /// <param name="caster"></param>
        internal void AddSpell(Type spellType, Unit caster)
        {
            //Validates that the caster is able to cast the given spell
            if (CanCastSpell(spellType, caster))
            {
                //Creates a new spell
                Spell spellToAdd = (Model.Spell)Activator.CreateInstance(spellType, caster);

                //Validates that the caster have enough mana to cast the spell
                if(spellToAdd.Caster.CurrentMana >= spellToAdd.ManaCost)
                {
                    caster.IsCastingSpell = true;
                    _activeSpells.Add(spellToAdd);
                }
            }
        }

        /// <summary>
        ///  Checks if a given caster is able to cast a given spell
        /// </summary>
        /// <param name="spellType">Type of spell</param>
        /// <param name="caster">Unit object</param>
        /// <returns>True or false</returns>
        internal bool CanCastSpell(Type spellType, Unit caster)
        {
            //If caster have no current global cd
            if (caster.GlobalCooldown <= 0)
            {
                //If theres NO such spell whos: CasterID already have a spell of the same sort with remaining cd.
                return (!_activeSpells.Exists(Spell => Spell.Caster.UnitId == caster.UnitId && Spell.CoolDown > 0 && Spell.GetType() == spellType));
            }
            return false;
        }

        /// <summary>
        /// Getter for active spells
        /// </summary>
        internal List<Spell> ActiveSpells
        {
            get { return _activeSpells; }
        }
    }
}
