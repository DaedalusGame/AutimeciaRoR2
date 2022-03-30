using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosWindArea : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 3f;

        public float durationAnim;
        public float durationAnimBase = 3f;

        public float fireThreshold = 0.355f;
        int wavesFired; 

        public override void OnEnter()
        {
            base.OnEnter();

            duration = durationBase / attackSpeedStat;
            durationAnim = durationAnimBase / attackSpeedStat;

            animator = GetModelAnimator();
            StartAimMode(2f + duration, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            PlayCrossfade("FullBody, Override", "ChaosCast3", "Chaos.playbackRate", durationAnim, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge / durationAnim >= fireThreshold && wavesFired <= 0)
            {
                EffectManager.SpawnEffect(Projectiles.effectWindArea, new EffectData()
                {
                    rootObject = gameObject,
                    origin = gameObject.transform.position,
                    scale = 3,
                }, true);
                wavesFired++;
                FireWave(4f, 10f, 4000);
                this.SetBodyState(new FreezeState() { duration = duration - fixedAge });
            }

            if(wavesFired > 0 && characterMotor) 
                characterMotor.velocity = Vector3.zero;

            if (fixedAge >= durationAnim * fireThreshold + 0.4f && wavesFired <= 1)
            {
                FireWave(14f, 15f, 6000);
                wavesFired++;
            }

            if (fixedAge >= durationAnim * fireThreshold + 1.2f && wavesFired <= 2)
            {
                FireWave(20f, 23f, 7000);
                wavesFired++;
            }

            if (fixedAge >= durationAnim * fireThreshold + 1.7f && wavesFired <= 3)
            {
                FireWave(30f, 7f, 5000);
                wavesFired++;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireWave(float radius, float damageMultiplier, float force)
        {
            var blastAttack = new BlastAttack()
            {
                position = characterBody.corePosition,
                radius = radius,
                baseForce = force,
                falloffModel = BlastAttack.FalloffModel.None,
                attackerFiltering = AttackerFiltering.NeverHit,
                attacker = gameObject,
                inflictor = gameObject,
                procCoefficient = 1.0f,
                losType = BlastAttack.LoSType.None,
                crit = Util.CheckRoll(critStat, characterBody.master),
                teamIndex = teamComponent.teamIndex,
                baseDamage = damageStat * damageMultiplier,
            };
            blastAttack.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
