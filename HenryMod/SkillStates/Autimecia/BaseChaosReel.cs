using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    abstract class BaseChaosReel : BaseSkillState
    {
        protected Animator animator;
        public float durationMin;
        public float durationMax;
        public float chargeMin = 0.7f;
        public float chargeMax = 5f;
        public ReelComponent reel;

        bool keyDownLast;
        bool selectTriggered;

        public List<object> values = new List<object>();

        protected abstract GameObject GetReelPrefab();

        protected abstract void SetNextState();

        public override void OnEnter()
        {
            base.OnEnter();

            durationMax = chargeMax * attackSpeedStat;
            durationMin = chargeMin;

            animator = GetModelAnimator();
            StartAimMode(2f + durationMax, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            reel = GameObject.Instantiate(GetReelPrefab(), modelLocator.modelTransform).GetComponent<ReelComponent>();
            reel.spinRate = 160f / attackSpeedStat;
            reel.spinOffset = UnityEngine.Random.Range(0, 360);
            reel.StartReel(AnimationCurve.EaseInOut(0, 0, 0.3f, 1f));
            keyDownLast = IsKeyDownAuthority();
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
            if (reel)
            {
                Destroy(reel.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            var keyDown = IsKeyDownAuthority();
            if (isAuthority && !selectTriggered && ((keyDown && !keyDownLast && fixedAge >= durationMin) || fixedAge >= durationMax))
            {
                values.Add(reel.Select());
                selectTriggered = true;
            }
            keyDownLast = keyDown;

            if (reel.done)
            {
                SetNextState();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
