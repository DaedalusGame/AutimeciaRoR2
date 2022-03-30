using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using RoR2;
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
            AutimeciaPlugin.ModLogger.LogInfo($"Chaos values contain: {String.Join(",", values)}");

            var element = values.GetFirstOfType<ChaosElement>();
            var target = values.GetFirstOfType<ChaosTarget>();

            //Buff
            if (element == ChaosElement.Fire && target == ChaosTarget.Buff)
                this.outer.SetNextState(new ChaosFireSprint());
            else if (element == ChaosElement.Water && target == ChaosTarget.Buff)
                this.outer.SetNextState(new ChaosWaterBuff());
            else if (element == ChaosElement.Wind && target == ChaosTarget.Buff)
                this.outer.SetNextState(new ChaosWindBuff());
            else if (element == ChaosElement.Earth && target == ChaosTarget.Buff)
                this.outer.SetNextState(new ChaosEarthBuff());
            //Projectile
            else if (element == ChaosElement.Fire && target == ChaosTarget.Projectile)
                this.outer.SetNextState(new ChaosFireProjectile());
            else if (element == ChaosElement.Water && target == ChaosTarget.Projectile)
                this.outer.SetNextState(new ChaosWaterProjectile());
            else if (element == ChaosElement.Wind && target == ChaosTarget.Projectile)
                this.outer.SetNextState(new ChaosWindProjectile());
            //Shield
            else if (element == ChaosElement.Fire && target == ChaosTarget.Shield)
                this.outer.SetNextState(new ChaosFireShield());
            else if (element == ChaosElement.Water && target == ChaosTarget.Shield)
                this.outer.SetNextState(new ChaosWaterShield());
            else if (element == ChaosElement.Wind && target == ChaosTarget.Shield)
                this.outer.SetNextState(new ChaosWindShield());
            //Area
            else if (element == ChaosElement.Fire && target == ChaosTarget.Area)
                this.outer.SetNextState(new ChaosFireArea());
            else if (element == ChaosElement.Water && target == ChaosTarget.Area)
                this.outer.SetNextState(new ChaosWaterArea());
            else if (element == ChaosElement.Wind && target == ChaosTarget.Area)
                this.outer.SetNextState(new ChaosWindArea());
            else if (element == ChaosElement.Earth && target == ChaosTarget.Area)
                this.outer.SetNextState(new ChaosEarthArea());
            else
                this.outer.SetNextState(new ChaosWaterShield());
        }

        private void SetOnBody(EntityState state)
        {
            var bodyMachine = EntityStateMachine.FindByCustomName(gameObject, "Body");

            if (bodyMachine)
            {
                bodyMachine.SetNextState(state);
                this.outer.SetNextStateToMain();
            }
            else
                this.outer.SetNextState(state);
        }

        protected override GameObject GetReelPrefab()
        {
            return Projectiles.reelTarget;
        }
    }
}
