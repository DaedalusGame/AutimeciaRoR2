using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class HatBeamFire : BaseHatBeamState
    {
        float startTime = 0.1f;
        float duration = 3.0f;
        float endTime = 0.4f;

        AnimationCurve curveStart = AnimationCurve.EaseInOut(0, 0, 1f, 1);
        AnimationCurve curveEnd = AnimationCurve.EaseInOut(0, 1, 1f, 0);

        bool firing => age >= startTime;

        BeamAttack beamAttack;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayAnimation("Base", "TrilbyFire");

            beamAttack = new BeamAttack() {
                projectileDamage = projectileDamage,
                projectileController = projectileController,
                transform = transform,
                maxDistance = 100,
                radius = 0.5f,
                multiHitDelay = 0.1f,
                multiHitMultiplier = StaticValues.hatBeamDamageDiminish,
            };
            beamAttack.Init();
        }

        public override void Update()
        {
            base.Update();

            if(firing)
                beamAttack.UpdateRay();

            boneEnd.localPosition = Vector3.right * beamAttack.rayDist;

            var texScale = beamAttack.rayDist;
            var texOffset = (age * -10f);
            rendererOuter.material.SetTextureOffset("_MainTex", new Vector2(0, texOffset));
            rendererOuter.material.SetTextureScale("_MainTex", new Vector2(0, texScale));
        }

        protected override float GetBeamScale()
        {
            float appearSlide = Mathf.Clamp01(age / startTime);
            float disappearSlide = Mathf.Clamp01((age - startTime - duration) / endTime);

            return curveStart.Evaluate(appearSlide) * curveEnd.Evaluate(disappearSlide);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (firing)
            {
                beamAttack.Fire();
            }

            if (fixedAge >= startTime + duration + endTime)
                outer.SetNextStateToMain();
        }
    }
}
