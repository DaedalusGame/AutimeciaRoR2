using Autimecia.Modules;
using Autimecia.SkillStates.Autimecia.Chaos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class ChaosEarthBuff : BaseChaosBuff
    {
        public override void ApplyBuff()
        {
            if (isAuthority) {
                characterBody.AddTimedBuffAuthority(Buffs.earthBuff.buffIndex, 30);
            }
        }
    }
}
