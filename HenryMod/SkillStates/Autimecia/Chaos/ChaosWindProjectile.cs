using Autimecia.Modules;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosWindProjectile : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 5f;

        public float durationAnim;
        public float durationAnimBase = 0.7f;

        public float fireThreshold = 0.77f;

        public int projectiles;
        public int projectilesLast;
        public float fireRateBase = 0.5f;
        public float fireRate;

        public float damageCoefficient = 5f;
        public float force = 0;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = durationBase / attackSpeedStat;
            durationAnim = durationAnimBase / attackSpeedStat;
            fireRate = fireRateBase / attackSpeedStat;

            animator = GetModelAnimator();
            StartAimMode(2f + duration, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            PlayCrossfade("FullBody, Override", "ChaosCast2", "Chaos.playbackRate", durationAnim, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge / durationAnim >= fireThreshold)
            {
                projectiles = Mathf.FloorToInt((fixedAge - (durationAnim * fireThreshold)) / fireRate);
                FireProjectileGroup();
                projectilesLast = projectiles;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireProjectileGroup()
        {
            var aimRay = GetAimRay();

            for (int i = projectilesLast; i < projectiles; i++)
            {
                FireProjectile(aimRay);
            }
        }

        private void FireProjectile(Ray aimRay)
        {
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(Projectiles.chaosWind, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
