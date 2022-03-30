using Autimecia.SkillStates;
using Autimecia.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace Autimecia.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void RegisterStates()
        {
            entityStates.Add(typeof(BaseMeleeAttack));
            entityStates.Add(typeof(SlashCombo));

            entityStates.Add(typeof(Shoot));

            entityStates.Add(typeof(Roll));

            entityStates.Add(typeof(ThrowBomb));

            entityStates.Add(typeof(FedoraBeam));
            entityStates.Add(typeof(FedoraSpeen));

            entityStates.Add(typeof(FaustCharge));
            entityStates.Add(typeof(FaustFire));

            entityStates.Add(typeof(ChaosYuurei));
            entityStates.Add(typeof(ChaosYuureiElement));
            entityStates.Add(typeof(ChaosYuureiTarget));

            entityStates.Add(typeof(ChaosFireProjectile));
            entityStates.Add(typeof(ChaosFireShield));
            entityStates.Add(typeof(ChaosFireSprint));
            entityStates.Add(typeof(ChaosFireArea));
            entityStates.Add(typeof(ChaosWaterProjectile));
            entityStates.Add(typeof(ChaosWaterShield));
            entityStates.Add(typeof(ChaosWaterBuff));
            entityStates.Add(typeof(ChaosWaterArea));
            entityStates.Add(typeof(ChaosWindArea));
            entityStates.Add(typeof(ChaosWindProjectile));
            entityStates.Add(typeof(ChaosWindShield));
            entityStates.Add(typeof(ChaosWindBuff));
            entityStates.Add(typeof(ChaosEarthArea));
            entityStates.Add(typeof(ChaosEarthBuff));

            entityStates.Add(typeof(FreezeState));
        }
    }
}