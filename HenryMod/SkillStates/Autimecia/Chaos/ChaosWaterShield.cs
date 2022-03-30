using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosWaterShield : BaseChaosShield
    {
        protected override void CreateShield()
        {
            GameObject shield = UnityEngine.Object.Instantiate(Projectiles.shieldWater, transform);
            var shieldController = shield.GetComponent<ShieldController>();
            shieldController.networkOwner = gameObject;
        }
    }
}
