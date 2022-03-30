using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosEarthArea : BaseSkillState
    {
        static int waveProjectileCount = 10;

        protected Animator animator;
        public float duration;
        public float durationBase = 3f;

        public float durationAnim;
        public float durationAnimBase = 3f;

        public float fireThreshold = 0.355f;
        bool fired;

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

            if (fixedAge / durationAnim >= fireThreshold && !fired)
            {
                FireRingAuthority();
                fired = true;
                this.SetBodyState(new FreezeState() { duration = duration - fixedAge });
            }

            if (fired && characterMotor)
                characterMotor.velocity = Vector3.zero;

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireRingAuthority()
        {
            float num = 360f / waveProjectileCount;
            Vector3 point = Vector3.ProjectOnPlane(inputBank.aimDirection, Vector3.up);
            Vector3 footPosition = characterBody.footPosition;
            for (int i = 0; i < waveProjectileCount; i++)
            {
                Vector3 forward = Quaternion.AngleAxis(num * i, Vector3.up) * point;
                if (base.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(Projectiles.chaosEarthWave, footPosition, Util.QuaternionSafeLookRotation(forward), base.gameObject, base.characterBody.damage, 0, Util.CheckRoll(base.characterBody.crit, base.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
