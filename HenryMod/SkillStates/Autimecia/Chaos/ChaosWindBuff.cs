using Autimecia.Modules;
using Autimecia.SkillStates.Autimecia.Chaos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class ChaosWindBuff : BaseChaosBuff
    {
        public override void ApplyBuff()
        {
            if (isAuthority)
            {
                characterBody.AddTimedBuffAuthority(Buffs.windBuff.buffIndex, 30);
            }
        }
    }
}
