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
    class ChaosFireProjectile : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 2f;

        public float durationAnim;
        public float durationAnimBase = 1f;

        public float fireThreshold = 0.77f;

        public int projectiles;
        public int projectilesLast;
        public float fireRateBase = 0.02f;
        public float fireRate;

        public float damageCoefficient = 0.8f;
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

            for(int i = projectilesLast; i < projectiles; i++)
            {
                float angleOffset = (i % 3 - 1) * 35f;
                FireProjectile(new Ray(aimRay.origin, Quaternion.AngleAxis(angleOffset, Vector3.up) * aimRay.direction));
            }
        }

        private void FireProjectile(Ray aimRay)
        {
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(Projectiles.chaosFireball, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
