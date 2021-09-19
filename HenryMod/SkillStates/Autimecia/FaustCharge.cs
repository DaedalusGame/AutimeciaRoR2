using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class FaustCharge : BaseSkillState
    {
        protected Animator animator;
        public float durationMin;
        public float durationMax;
        public float chargeMin = 0.7f;
        public float chargeMax = 5f;

        public FaustCharge()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            durationMax = chargeMax / attackSpeedStat;
            durationMin = chargeMin / attackSpeedStat;

            animator = GetModelAnimator();
            StartAimMode(2f + durationMax, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            /*Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }*/

            PlayCrossfade("FullBody, Override", "Tip", "Tip.playbackRate", durationMin, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority && ((!IsKeyDownAuthority() && fixedAge >= durationMin) || fixedAge >= durationMax))
            {
                var nextState = new FaustFire();
                this.outer.SetNextState(nextState);
            }
        }
    }
}
