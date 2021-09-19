using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosYuurei : BaseSkillState
    {
        protected Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();

            animator = GetModelAnimator();
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            PlayCrossfade("FullBody, Override", "ChaosStart", "Chaos.playbackRate", 1f, 0.05f);

            var state = new ChaosYuureiElement();
            state.activatorSkillSlot = activatorSkillSlot;
            outer.SetNextState(state);
        }
    }
}
