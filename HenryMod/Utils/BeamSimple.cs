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
    class BeamSimple : MonoBehaviour
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

        public float stopwatch;
        public float startTime;
        public float endTime;
        public AnimationCurve startCurve = AnimationCurve.Constant(0, float.PositiveInfinity, 0);
        public AnimationCurve endCurve = AnimationCurve.Constant(0, float.PositiveInfinity, 1);

        public bool fired;
        public float maxDistance;
        public float radius;
        public float multiHitMultiplier;
        public float multiHitDelay;
        public BeamSearch beamSearch;
        private readonly List<HurtBox> hurtBoxesList = new List<HurtBox>();
        public ProjectileDamage projectileDamage;
        public ProjectileController projectileController;
        public float rayDist;

        public Transform boneStart;
        public Transform boneEnd;
        public Renderer rendererOuter;

        Dictionary<HealthComponent, float> lastHit = new Dictionary<HealthComponent, float>();

        List<RayLine> rays = new List<RayLine>();

        private void Awake()
        {
            beamSearch = new BeamSearch()
            {
                sphereRadius = radius,
                mask = LayerIndex.entityPrecise.mask,
            };
            projectileController = gameObject.GetComponentInParent<ProjectileController>();
            projectileDamage = gameObject.GetComponentInParent<ProjectileDamage>();
            var locator = gameObject.GetComponent<ChildLocator>();
            boneStart = locator.FindChild("BeamStart");
            boneEnd = locator.FindChild("BeamEnd");
            rendererOuter = locator.FindChildGameObject("Outer").GetComponent<Renderer>();
        }

        protected void OnEnable()
        {
        }

        public void Update()
        {
            RaycastHit raycastHit;
            Ray ray = new Ray(transform.position, transform.forward);
            rayDist = maxDistance;
            if (Physics.SphereCast(ray, radius, out raycastHit, maxDistance, LayerIndex.world.mask))
            {
                rayDist = raycastHit.distance;
            }
            rayDist = Mathf.Max(rayDist, 0.01f);

            if(fired)
            {
                var newRay = new RayLine(transform.position, transform.forward * rayDist);
                if(!rays.LastOrDefault().Equals(newRay, 0.001f))
                    rays.Add(newRay);
            }

            var scale = startCurve.Evaluate(stopwatch - startTime) * endCurve.Evaluate(stopwatch - endTime);
            boneStart.localScale = Vector3.one * scale;
            boneEnd.localScale = Vector3.one * scale;
            boneEnd.localPosition = Vector3.right * rayDist;
            var texScale = rayDist;
            var texOffset = (stopwatch * -10f);
            rendererOuter.material.SetTextureOffset("_MainTex", new Vector2(0, texOffset));
            rendererOuter.material.SetTextureScale("_MainTex", new Vector2(0, texScale));

            stopwatch += Time.deltaTime;
        }

        public void Fire(AnimationCurve curve)
        {
            startTime = stopwatch;
            startCurve = curve;
            fired = true;
        }

        public void Finish(AnimationCurve curve, float delay = 0f)
        {
            endTime = stopwatch + delay;
            endCurve = curve;
        }

        public void FixedUpdate()
        {
            if (fired)
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
                force = this.projectileDamage.force * base.transform.forward,
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
