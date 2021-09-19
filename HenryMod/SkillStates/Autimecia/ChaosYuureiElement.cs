using Autimecia.Modules;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosYuureiElement : BaseChaosReel
    {
        protected override void SetNextState()
        {
            var state = new ChaosYuureiTarget();
            state.values = values;
            state.activatorSkillSlot = activatorSkillSlot;
            this.outer.SetNextState(state);
        }

        protected override GameObject GetReelPrefab()
        {
            return Projectiles.reelElement;
        }
    }
}
