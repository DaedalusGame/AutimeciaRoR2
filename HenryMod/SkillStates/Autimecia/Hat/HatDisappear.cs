using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class HatDisappear : BaseHatState
    {
        float duration = 0.5f;

        public override void OnEnter()
        {
            base.OnEnter();

            GetModelAnimator().SetBool("disappear", true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
