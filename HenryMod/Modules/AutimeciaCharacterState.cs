using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.Modules
{
    class AutimeciaCharacterState : GenericCharacterMain
    {
        public AutimeciaCharacterState()
        {
        }

        public override void ProcessJump()
        {
            var jumps = 0;
            if(this.hasCharacterMotor)
                jumps = characterMotor.jumpCount;
            base.ProcessJump();
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                if(characterMotor.jumpCount != jumps)
                {
                    PlayAnimation("Wings", "Jump");
                }
            }
        }
    }
}
