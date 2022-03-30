using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class HatBeamThrow : BaseHatBeamState
    {
        float duration = 0.15f;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge >= duration)
            {
                outer.SetNextState(new HatBeamStart());
            }
        }
    }
}
