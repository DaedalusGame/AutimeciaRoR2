using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class HatBeamStart : BaseHatBeamState
    {
        float duration = 0.1f;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayCrossfade("Base", "TrilbyBeam", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge >= duration)
            {
                outer.SetNextState(new HatBeamFire());
            }
        }
    }
}
