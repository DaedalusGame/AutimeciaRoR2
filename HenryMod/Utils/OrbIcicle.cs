using Autimecia.Modules;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.Utils
{
    class OrbIcicle : GenericDamageOrb
    {
        public float freezeTime;
        public float healMultiplier;

        public override void Begin()
        {
            /*duration = distanceToTarget / speed;
            if (Projectiles.orbIcicle)
            {
                EffectData effectData = new EffectData
                {
                    scale = scale,
                    origin = origin,
                    genericFloat = duration
                };
                effectData.SetHurtBoxReference(target);
                EffectManager.SpawnEffect(Projectiles.orbIcicle, effectData, true);
            }*/
        }

        public override void OnArrival()
        {
            base.OnArrival();

            if (target)
            {

                var healthComponent = target.healthComponent;
                if (healthComponent)
                {
                    var setStateOnHurt = healthComponent.GetComponent<SetStateOnHurt>();
                    if (setStateOnHurt && setStateOnHurt.canBeFrozen)
                    {
                        setStateOnHurt.SetFrozen(freezeTime);
                    }

                    var attackerHealth = attacker.GetComponent<HealthComponent>();

                    AutimeciaPlugin.ModLogger.LogInfo($"{attacker} {attackerHealth}");

                    if (attackerHealth && healthComponent.body && attackerHealth.body)
                    {
                        HealOrb healOrb = new HealOrb();
                        healOrb.origin = healthComponent.body.corePosition;
                        healOrb.target = attackerHealth.body.mainHurtBox;
                        healOrb.healValue = healMultiplier * attackerHealth.fullHealth;
                        healOrb.overrideDuration = 1.0f;
                        OrbManager.instance.AddOrb(healOrb);
                    }
                }
            }
        }
    }
}
