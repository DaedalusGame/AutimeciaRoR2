using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class FaustFire : BaseSkillState
    {
        protected Animator animator;
        public float durationBase = 0.7f;
        public float duration;

        AutimeciaTracker huntressTracker;
        HurtBox initialOrbTarget;

        public float fireThreshold = 0.3f;
        bool fired;

        public FaustFire()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            huntressTracker = GetComponent<AutimeciaTracker>();
            if (huntressTracker && isAuthority)
            {
                initialOrbTarget = huntressTracker.GetTrackingTarget();
            }

            duration = durationBase / attackSpeedStat;

            animator = GetModelAnimator();
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            PlayCrossfade("FullBody, Override", "TipYeet", "Tip.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
        }

        private void FireHat()
        {
            if (initialOrbTarget)
            {
                var orb = new OrbFaust();
                var aimRay = GetAimRay();
                orb.speed = 50f;
                orb.origin = aimRay.origin;
                orb.target = initialOrbTarget;
                orb.damageValue = damageStat * StaticValues.faustDamage;
                orb.isCrit = Util.CheckRoll(critStat, characterBody.master);
                orb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
                orb.attacker = gameObject;
                orb.procCoefficient = 0.0f;
                OrbManager.instance.AddOrb(orb);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(!fired && fixedAge / duration >= fireThreshold)
            {
                FireHat();
                fired = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
    }
}
