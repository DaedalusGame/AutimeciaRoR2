using Autimecia.Modules.ProjectileComponents;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class BaseHatBeamState : BaseHatState
    {
        protected ProjectileFedoraLaser projectileFedoraLaser;

        protected Transform boneStart;
        protected Transform boneEnd;
        protected Renderer rendererOuter;
        protected GameObject beam;

        public override void OnEnter()
        {
            base.OnEnter();

            projectileFedoraLaser = GetComponent<ProjectileFedoraLaser>();

            beam = modelLocator.modelBaseTransform.Find("Beam").gameObject;
            var locator = beam.GetComponent<ChildLocator>();
            boneStart = locator.FindChild("BeamStart");
            boneEnd = locator.FindChild("BeamEnd");
            rendererOuter = locator.FindChildGameObject("Outer").GetComponent<Renderer>();
        }

        public override void Update()
        {
            base.Update();

            var scale = GetBeamScale();
            boneStart.localScale = Vector3.one * scale;
            boneEnd.localScale = Vector3.one * scale;
        }

        protected virtual float GetBeamScale()
        {
            return 0;
        }
    }
}
