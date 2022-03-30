using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    abstract class BaseChaosShield : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 1f;

        public float durationAnim;
        public float durationAnimBase = 0.7f;

        public float fireThreshold = 0.355f;

        public bool fired;

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

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge / durationAnim >= fireThreshold && !fired)
            {
                CreateShield();
                fired = true;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        protected abstract void CreateShield();

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
