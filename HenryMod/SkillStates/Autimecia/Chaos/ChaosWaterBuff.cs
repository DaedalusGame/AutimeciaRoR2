using Autimecia.Modules;
using Autimecia.SkillStates.Autimecia.Chaos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class ChaosWaterBuff : BaseChaosBuff
    {
        public override void ApplyBuff()
        {
            if (isAuthority)
            {
                characterBody.AddTimedBuffAuthority(Buffs.waterBuff.buffIndex, 30);
            }
        }
    }
}
