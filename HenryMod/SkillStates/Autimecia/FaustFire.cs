using EntityStates;
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

        public FaustFire()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

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

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }
    }
}
