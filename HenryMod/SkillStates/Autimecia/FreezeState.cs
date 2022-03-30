using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.SkillStates
{
    class FreezeState : EntityState
    {
        public float duration;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (characterMotor)
                characterMotor.velocity = Vector3.zero;

            if (fixedAge >= duration)
                outer.SetNextStateToMain();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(duration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            duration = reader.ReadSingle();
        }
    }
}
