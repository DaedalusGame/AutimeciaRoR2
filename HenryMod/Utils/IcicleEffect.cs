using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class IcicleEffect : MonoBehaviour
    {
        private OrbEffect orbEffect;
        private ChildLocator childLocator;
        private EffectData effectData;

        private Transform[] bones; 

        public void Awake()
        {
            orbEffect = GetComponent<OrbEffect>();
            childLocator = GetComponent<ChildLocator>();
            effectData = GetComponent<EffectData>();

            bones = new[] {
                childLocator.FindChild("Bone0"),
                childLocator.FindChild("Bone1"),
                childLocator.FindChild("Bone2"),
                childLocator.FindChild("Bone3"),
                childLocator.FindChild("Bone4"),
            };
        }

        public void Update()
        {
            float movementSlide = orbEffect.movementCurve.Evaluate(Mathf.Clamp01(orbEffect.age / orbEffect.duration));

            float getSlide(int i)
            {
                return ((float)i / (bones.Length - 1)) * movementSlide;
            }

            for(int i = 0; i < bones.Length; i++)
            {
                var bonePosPrev = Vector3.Lerp(orbEffect.startPosition, orbEffect.lastKnownTargetPosition, getSlide(i-1));
                var bonePos = Vector3.Lerp(orbEffect.startPosition, orbEffect.lastKnownTargetPosition, getSlide(i));
                var bonePosNext = Vector3.Lerp(orbEffect.startPosition, orbEffect.lastKnownTargetPosition, getSlide(i+1));

                var delta = bonePosNext - bonePosPrev;

                var bone = bones[i];
                bone.SetPositionAndRotation(bonePos, Quaternion.LookRotation(delta, Vector3.up));
            }
        }
    }
}
