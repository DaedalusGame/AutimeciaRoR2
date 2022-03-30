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
    class ChaosWaterProjectile : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 1f;

        public float durationAnim;
        public float durationAnimBase = 0.7f;

        public float fireThreshold = 0.77f;

        public float damageCoefficient = 5f;
        public float force = 0;
        public float maxSpread = 50;

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

            if (fixedAge / durationAnim >= fireThreshold && !fired)
            {
                List<Collider> salvoColliders = new List<Collider>();
                ProjectileSalvoHelper.StartSalvo();
                for (int i = 0; i < 5; i++)
                {
                    var aimRay = GetAimRay();
                    aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, maxSpread, 1f, 0.1f, 0f, 0f);
                    FireProjectile(aimRay);
                }
                foreach(var projectileController in ProjectileSalvoHelper.EndSalvo())
                {
                    var projectileIgnoreSalvo = projectileController.GetComponent<ProjectileIgnoreSalvo>();
                    if(projectileIgnoreSalvo)
                        projectileIgnoreSalvo.salvoColliders = salvoColliders;
                }
                fired = true;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireProjectile(Ray aimRay)
        {
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(Projectiles.chaosBubble, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
