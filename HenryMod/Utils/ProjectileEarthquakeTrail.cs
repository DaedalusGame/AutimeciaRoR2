using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class ProjectileEarthquakeTrail : MonoBehaviour
    {
        public GameObject prefab;
        public float radius = 1;
        public float force = 1000;
        public float damageMultiplier = 6f;
        
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;

        float stopwatchFire;
        public float fireRate;

        private void Awake()
        {
            projectileController = gameObject.GetComponent<ProjectileController>();
            projectileDamage = gameObject.GetComponent<ProjectileDamage>();
        }

        private void FixedUpdate()
        {
            stopwatchFire += Time.fixedDeltaTime;

            if(stopwatchFire >= fireRate)
            {
                stopwatchFire -= fireRate;
                FireQuake();
            }
        }

        private void FireQuake()
        {
            var blast = new BlastAttack()
            {
                attacker = projectileController.owner,
                inflictor = gameObject,
                teamIndex = projectileController.teamFilter.teamIndex,
                attackerFiltering = AttackerFiltering.NeverHit,
                position = transform.position,
                radius = radius,
                falloffModel = BlastAttack.FalloffModel.None,
                baseDamage = projectileDamage.damage * damageMultiplier,
                baseForce = 0,
                bonusForce = Vector3.up * force,
                crit = projectileDamage.crit,
                procCoefficient = 1,
                damageType = DamageType.Stun1s,
                losType = BlastAttack.LoSType.None,
                procChainMask = default(ProcChainMask),
            };
            blast.Fire();
            EffectManager.SpawnEffect(prefab, new EffectData()
            {
                origin = transform.position,
                scale = 1,
            }, true);
        }
    }
}
