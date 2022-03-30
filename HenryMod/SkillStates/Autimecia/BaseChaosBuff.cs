using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates.Autimecia.Chaos
{
    abstract class BaseChaosBuff : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 0.4f;

        public float durationAnim;
        public float durationAnimBase = 0.3f;

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
                ApplyBuff();
                fired = true;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public abstract void ApplyBuff();

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
