using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.Utils
{
    class BrokenSkillDef : SkillDef
    {
        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return false;
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return false;
        }
    }
}
