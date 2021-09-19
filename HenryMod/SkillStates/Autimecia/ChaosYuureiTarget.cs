using Autimecia.Modules;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosYuureiTarget : BaseChaosReel
    {
        protected override void SetNextState()
        {
            this.outer.SetNextState(new ChaosWaterProjectile());
        }

        protected override GameObject GetReelPrefab()
        {
            return Projectiles.reelTarget;
        }
    }
}
