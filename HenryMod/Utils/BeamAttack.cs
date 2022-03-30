using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.Utils
{
    class BeamAttack
    {
        struct RayLine
        {
            Vector3 position;
            Vector3 direction;

            public RayLine(Vector3 position, Vector3 direction)
            {
                this.position = position;
                this.direction = direction;
            }

            public bool Equals(RayLine other, float epsilon)
            {
                return ApproximatelyEquals(position, other.position, epsilon) && ApproximatelyEquals(direction, other.direction, epsilon);
            }

            bool ApproximatelyEquals(Vector3 a, Vector3 b, float epsilon)
            {
                var delta = a - b;

                return delta.sqrMagnitude < epsilon * epsilon;
            }
        }

        public Transform transform;
        public ProjectileDamage projectileDamage;
        public ProjectileController projectileController;
        public float maxDistance;
        public float radius;
        public float multiHitMultiplier;
        public float multiHitDelay;
        public BeamSearch beamSearch;

        public float stopwatch;
        public float rayDist;
        private readonly List<HurtBox> hurtBoxesList = new List<HurtBox>();

        

        Dictionary<HealthComponent, float> lastHit = new Dictionary<HealthComponent, float>();

        List<RayLine> rays = new List<RayLine>();

        public void Init()
        {
            beamSearch = new BeamSearch()
            {
                sphereRadius = radius,
                mask = LayerIndex.entityPrecise.mask,
            };
        }

        public void UpdateRay()
        {
            stopwatch += Time.deltaTime;

            RaycastHit raycastHit;
            Ray ray = new Ray(transform.position, transform.forward);
            rayDist = maxDistance;
            if (Physics.SphereCast(ray, radius, out raycastHit, maxDistance, LayerIndex.world.mask))
            {
                rayDist = raycastHit.distance;
            }
            rayDist = Mathf.Max(rayDist, 0.01f);

            var newRay = new RayLine(transform.position, transform.forward * rayDist);
            if (!rays.LastOrDefault().Equals(newRay, 0.001f))
                rays.Add(newRay);
        }

        public void Fire()
        {
            Ray ray = new Ray(transform.position, transform.forward);

            this.beamSearch.ray = ray;
            this.beamSearch.maxDistance = rayDist;
            this.beamSearch.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.all).FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes(this.hurtBoxesList);

            foreach (var hurtbox in this.hurtBoxesList)
            {
                DealDamage(hurtbox);
            }
            this.hurtBoxesList.Clear();

            rays.Clear();
        }

        private void DealDamage(HurtBox hurtbox)
        {
            DamageInfo damageInfo = new DamageInfo()
            {
                damage = this.projectileDamage.damage,
                crit = this.projectileDamage.crit,
                attacker = this.projectileController.owner,
                inflictor = this.projectileController.gameObject,
                position = hurtbox.transform.position,
                force = this.projectileDamage.force * transform.forward,
                procChainMask = this.projectileController.procChainMask,
                procCoefficient = this.projectileController.procCoefficient,
                damageColorIndex = this.projectileDamage.damageColorIndex,
                damageType = this.projectileDamage.damageType,
            };
            var healthComponent = hurtbox.healthComponent;
            if (healthComponent)
            {
                if (lastHit.ContainsKey(healthComponent))
                {
                    damageInfo.damage *= multiHitMultiplier;
                    damageInfo.procCoefficient *= multiHitMultiplier;
                    if (stopwatch < lastHit[healthComponent] + multiHitDelay)
                        return;
                }

                lastHit[healthComponent] = stopwatch;

                if (healthComponent.gameObject == this.projectileController.owner)
                {
                    return;
                }
                if (FriendlyFireManager.ShouldDirectHitProceed(healthComponent, this.projectileController.teamFilter.teamIndex))
                {
                    if (NetworkServer.active)
                    {
                        damageInfo.ModifyDamageInfo(hurtbox.damageModifier);
                        healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                    }
                }
            }
        }
    }
}
