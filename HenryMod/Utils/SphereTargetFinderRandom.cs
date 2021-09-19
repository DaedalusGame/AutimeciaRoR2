using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Autimecia.Utils
{
    [RequireComponent(typeof(TeamFilter))]
    [RequireComponent(typeof(ProjectileTargetComponent))]
    public class ProjectileSphereTargetFinderRandom : MonoBehaviour
    {
        // Token: 0x060029BA RID: 10682 RVA: 0x000A9E78 File Offset: 0x000A8078
        private void Start()
        {
            if (!NetworkServer.active)
            {
                base.enabled = false;
                return;
            }
            this.transform = base.transform;
            this.teamFilter = base.GetComponent<TeamFilter>();
            this.targetComponent = base.GetComponent<ProjectileTargetComponent>();
            this.sphereSearch = new SphereSearch();
            this.searchTimer = 0f;
        }

        // Token: 0x060029BB RID: 10683 RVA: 0x000A9ED0 File Offset: 0x000A80D0
        private void FixedUpdate()
        {
            this.searchTimer -= Time.fixedDeltaTime;
            if (this.searchTimer <= 0f)
            {
                this.searchTimer += this.targetSearchInterval;
                if (this.allowTargetLoss && this.targetComponent.target != null && this.lastFoundTransform == this.targetComponent.target && !this.PassesFilters(this.lastFoundHurtBox))
                {
                    this.SetTarget(null);
                }
                if (!this.onlySearchIfNoTarget || this.targetComponent.target == null)
                {
                    this.SearchForTarget();
                }
                this.hasTarget = (this.targetComponent.target != null);
                if (this.hadTargetLastUpdate != this.hasTarget)
                {
                    if (this.hasTarget)
                    {
                        UnityEvent unityEvent = this.onNewTargetFound;
                        if (unityEvent != null)
                        {
                            unityEvent.Invoke();
                        }
                    }
                    else
                    {
                        UnityEvent unityEvent2 = this.onTargetLost;
                        if (unityEvent2 != null)
                        {
                            unityEvent2.Invoke();
                        }
                    }
                }
                this.hadTargetLastUpdate = this.hasTarget;
            }
        }

        // Token: 0x060029BC RID: 10684 RVA: 0x000A9FD0 File Offset: 0x000A81D0
        private bool PassesFilters(HurtBox result)
        {
            CharacterBody body = result.healthComponent.body;
            return body && (!this.ignoreAir || !body.isFlying) && (!body.isFlying || float.IsInfinity(this.flierAltitudeTolerance) || this.flierAltitudeTolerance >= Mathf.Abs(result.transform.position.y - this.transform.position.y));
        }

        // Token: 0x060029BD RID: 10685 RVA: 0x000AA04C File Offset: 0x000A824C
        private void SearchForTarget()
        {
            this.sphereSearch.origin = this.transform.position;
            this.sphereSearch.radius = this.lookRange;
            this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
            this.sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
            this.sphereSearch.RefreshCandidates();
            this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(this.teamFilter.teamIndex));
            this.sphereSearch.OrderCandidatesByDistance();
            this.sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            this.sphereSearch.GetHurtBoxes(ProjectileSphereTargetFinder.foundHurtBoxes);
            List<HurtBox> possibleTargets = new List<HurtBox>();
            if (ProjectileSphereTargetFinder.foundHurtBoxes.Count > 0)
            {
                int i = 0;
                int count = ProjectileSphereTargetFinder.foundHurtBoxes.Count;
                while (i < count)
                {
                    if (this.PassesFilters(ProjectileSphereTargetFinder.foundHurtBoxes[i]))
                    {
                        possibleTargets.Add(ProjectileSphereTargetFinder.foundHurtBoxes[i]);
                    }
                    i++;
                }
                ProjectileSphereTargetFinder.foundHurtBoxes.Clear();
            }
            Util.ShuffleList(possibleTargets);
            if(possibleTargets.Any())
                this.SetTarget(possibleTargets.First());
        }

        // Token: 0x060029BE RID: 10686 RVA: 0x000AA14C File Offset: 0x000A834C
        private void SetTarget(HurtBox hurtBox)
        {
            this.lastFoundHurtBox = hurtBox;
            this.lastFoundTransform = ((hurtBox != null) ? hurtBox.transform : null);
            this.targetComponent.target = this.lastFoundTransform;
        }

        // Token: 0x060029BF RID: 10687 RVA: 0x000AA178 File Offset: 0x000A8378
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 position = base.transform.position;
            Gizmos.DrawWireSphere(position, this.lookRange);
            if (!float.IsInfinity(this.flierAltitudeTolerance))
            {
                Gizmos.DrawWireCube(position, new Vector3(this.lookRange * 2f, this.flierAltitudeTolerance * 2f, this.lookRange * 2f));
            }
        }

        // Token: 0x0400245C RID: 9308
        [Tooltip("How far ahead the projectile should look to find a target.")]
        public float lookRange;

        // Token: 0x0400245D RID: 9309
        [Tooltip("How long before searching for a target.")]
        public float targetSearchInterval = 0.5f;

        // Token: 0x0400245E RID: 9310
        [Tooltip("Will not search for new targets once it has one.")]
        public bool onlySearchIfNoTarget;

        // Token: 0x0400245F RID: 9311
        [Tooltip("Allows the target to be lost if it's outside the acceptable range.")]
        public bool allowTargetLoss;

        // Token: 0x04002460 RID: 9312
        [Tooltip("If set, targets can only be found when there is a free line of sight.")]
        public bool testLoS;

        // Token: 0x04002461 RID: 9313
        [Tooltip("Whether or not airborne characters should be ignored.")]
        public bool ignoreAir;

        // Token: 0x04002462 RID: 9314
        [Tooltip("The difference in altitude at which a result will be ignored.")]
        public float flierAltitudeTolerance = float.PositiveInfinity;

        // Token: 0x04002463 RID: 9315
        public UnityEvent onNewTargetFound;

        // Token: 0x04002464 RID: 9316
        public UnityEvent onTargetLost;

        // Token: 0x04002465 RID: 9317
        private new Transform transform;

        // Token: 0x04002466 RID: 9318
        private TeamFilter teamFilter;

        // Token: 0x04002467 RID: 9319
        private ProjectileTargetComponent targetComponent;

        // Token: 0x04002468 RID: 9320
        private float searchTimer;

        // Token: 0x04002469 RID: 9321
        private SphereSearch sphereSearch;

        // Token: 0x0400246A RID: 9322
        private bool hasTarget;

        // Token: 0x0400246B RID: 9323
        private bool hadTargetLastUpdate;

        // Token: 0x0400246C RID: 9324
        private HurtBox lastFoundHurtBox;

        // Token: 0x0400246D RID: 9325
        private Transform lastFoundTransform;

        // Token: 0x0400246E RID: 9326
        private static readonly List<HurtBox> foundHurtBoxes = new List<HurtBox>();
    }
}
