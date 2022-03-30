using Autimecia.Modules;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class OrbFaust : GenericDamageOrb
    {
        public override void Begin()
        {
            duration = distanceToTarget / speed;
            if (Projectiles.orbFaust)
            {
                EffectData effectData = new EffectData
                {
                    scale = scale,
                    origin = origin,
                    genericFloat = duration
                };
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(Projectiles.orbFaust, effectData, true);
            }
        }

        public override void OnArrival()
        {
            base.OnArrival();
            if (this.target)
            {
                HealthComponent healthComponent = this.target.healthComponent;
                if (healthComponent && healthComponent.body)
                {
                    var faust = healthComponent.body.gameObject.AddComponent<FaustComponent>();
                    faust.attacker = attacker;

                    SetStateOnHurt setStateOnHurt = healthComponent.GetComponent<SetStateOnHurt>();
                    if (setStateOnHurt)
                    {
                        setStateOnHurt.SetStun(-1f);
                    }
                }
            }
        }
    }
}
