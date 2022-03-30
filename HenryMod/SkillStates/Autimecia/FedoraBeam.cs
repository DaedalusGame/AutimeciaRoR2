using EntityStates;
using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Projectile;
using Autimecia.Modules;

namespace Autimecia.SkillStates
{
    class FedoraBeam : BaseSkillState
    {
        private const float fireThreshold = 0.59f;

        protected Animator animator;
        public float durationBase = 0.7f;
        public float duration;
        public bool fired;

        public FedoraBeam()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            duration = durationBase / attackSpeedStat;

            animator = GetModelAnimator();
            StartAimMode(0.5f + duration, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            /*Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }*/

            PlayCrossfade("FullBody, Override", "Cast1", "Cast1.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
        }

        private void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    crit = RollCrit(),
                    damage = damageStat * StaticValues.hatBeamDamageInitial,
                    damageColorIndex = DamageColorIndex.Default,
                    force = 0f,
                    owner = gameObject,
                    position = aimRay.origin,
                    procChainMask = default(ProcChainMask),
                    projectilePrefab = Projectiles.fedoraBeamPrefab,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    target = null
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge / duration >= fireThreshold && !fired)
            {
                FireProjectile();
                fired = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
