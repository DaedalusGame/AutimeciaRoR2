using HG;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    public class BeamSearch
    {
        // Token: 0x0600220B RID: 8715 RVA: 0x00088F2E File Offset: 0x0008712E
        public BeamSearch ClearCandidates()
        {
            this.searchData = BeamSearch.SearchData.empty;
            return this;
        }

        public BeamSearch RefreshCandidates()
        {
            RaycastHit[] hits = Physics.SphereCastAll(this.ray, this.sphereRadius, this.maxDistance, this.mask, this.queryTriggerInteraction);
            BeamSearch.Candidate[] array2 = new BeamSearch.Candidate[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                ref BeamSearch.Candidate ptr = ref array2[i];
                ptr.collider = collider;
                MeshCollider meshCollider;
                if ((meshCollider = (collider as MeshCollider)) != null && !meshCollider.convex)
                {
                    ptr.position = collider.ClosestPointOnBounds(this.ray.origin);
                }
                else
                {
                    ptr.position = collider.ClosestPoint(this.ray.origin);
                }
                ptr.difference = ptr.position - this.ray.origin;
                ptr.distanceSqr = ptr.difference.sqrMagnitude;
            }
            this.searchData = new BeamSearch.SearchData(array2);
            return this;
        }

        public BeamSearch OrderCandidatesByDistance()
        {
            this.searchData.OrderByDistance();
            return this;
        }

        public BeamSearch FilterCandidatesByHurtBoxTeam(TeamMask mask)
        {
            this.searchData.FilterByHurtBoxTeam(mask);
            return this;
        }

        public BeamSearch FilterCandidatesByColliderEntities()
        {
            this.searchData.FilterByColliderEntities();
            return this;
        }

        public BeamSearch FilterCandidatesByDistinctColliderEntities()
        {
            this.searchData.FilterByColliderEntitiesDistinct();
            return this;
        }

        public BeamSearch FilterCandidatesByDistinctHurtBoxEntities()
        {
            this.searchData.FilterByHurtBoxEntitiesDistinct();
            return this;
        }

        public BeamSearch FilterCandidatesByProjectileControllers()
        {
            this.searchData.FilterByProjectileControllers();
            return this;
        }

        public HurtBox[] GetHurtBoxes()
        {
            return this.searchData.GetHurtBoxes();
        }

        public void GetHurtBoxes(List<HurtBox> dest)
        {
            this.searchData.GetHurtBoxes(dest);
        }

        public void GetProjectileControllers(List<ProjectileController> dest)
        {
            this.searchData.GetProjectileControllers(dest);
        }

        public void GetColliders(List<Collider> dest)
        {
            this.searchData.GetColliders(dest);
        }

        public Ray ray;

        public float maxDistance;

        public float sphereRadius;

        public LayerMask mask;

        public QueryTriggerInteraction queryTriggerInteraction;

        private BeamSearch.SearchData searchData = BeamSearch.SearchData.empty;

        // Token: 0x02000565 RID: 1381
        private struct Candidate
        {
            // Token: 0x06002218 RID: 8728 RVA: 0x000890B2 File Offset: 0x000872B2
            public static bool HurtBoxHealthComponentIsValid(BeamSearch.Candidate candidate)
            {
                return candidate.hurtBox.healthComponent;
            }

            // Token: 0x04001DCD RID: 7629
            public Collider collider;

            // Token: 0x04001DCE RID: 7630
            public HurtBox hurtBox;

            // Token: 0x04001DCF RID: 7631
            public Vector3 position;

            // Token: 0x04001DD0 RID: 7632
            public Vector3 difference;

            // Token: 0x04001DD1 RID: 7633
            public float distanceSqr;

            // Token: 0x04001DD2 RID: 7634
            public Transform root;

            // Token: 0x04001DD3 RID: 7635
            public ProjectileController projectileController;

            // Token: 0x04001DD4 RID: 7636
            public EntityLocator entityLocator;
        }

        // Token: 0x02000566 RID: 1382
        private struct SearchData
        {
            // Token: 0x06002219 RID: 8729 RVA: 0x000890C4 File Offset: 0x000872C4
            public SearchData(BeamSearch.Candidate[] candidatesBuffer)
            {
                this.candidatesBuffer = candidatesBuffer;
                this.candidatesMapping = new int[candidatesBuffer.Length];
                this.candidatesCount = candidatesBuffer.Length;
                for (int i = 0; i < candidatesBuffer.Length; i++)
                {
                    this.candidatesMapping[i] = i;
                }
                this.hurtBoxesLoaded = false;
                this.rootsLoaded = false;
                this.projectileControllersLoaded = false;
                this.entityLocatorsLoaded = false;
                this.filteredByHurtBoxes = false;
                this.filteredByHurtBoxHealthComponents = false;
                this.filteredByProjectileControllers = false;
                this.filteredByEntityLocators = false;
            }

            private ref BeamSearch.Candidate GetCandidate(int i)
            {
                return ref this.candidatesBuffer[this.candidatesMapping[i]];
            }

            private void RemoveCandidate(int i)
            {
                ArrayUtils.ArrayRemoveAt<int>(this.candidatesMapping, ref this.candidatesCount, i, 1);
            }

            public void LoadHurtBoxes()
            {
                if (this.hurtBoxesLoaded)
                {
                    return;
                }
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    candidate.hurtBox = candidate.collider.GetComponent<HurtBox>();
                }
                this.hurtBoxesLoaded = true;
            }


            public void LoadRoots()
            {
                if (this.rootsLoaded)
                {
                    return;
                }
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    candidate.root = candidate.collider.transform.root;
                }
                this.rootsLoaded = true;
            }

            public void LoadProjectileControllers()
            {
                if (this.projectileControllersLoaded)
                {
                    return;
                }
                this.LoadRoots();
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    candidate.projectileController = (candidate.root ? candidate.root.GetComponent<ProjectileController>() : null);
                }
                this.projectileControllersLoaded = true;
            }

            public void LoadColliderEntityLocators()
            {
                if (this.entityLocatorsLoaded)
                {
                    return;
                }
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    candidate.entityLocator = candidate.collider.GetComponent<EntityLocator>();
                }
                this.entityLocatorsLoaded = true;
            }

            // Token: 0x06002220 RID: 8736 RVA: 0x000892A4 File Offset: 0x000874A4
            public void FilterByProjectileControllers()
            {
                if (this.filteredByProjectileControllers)
                {
                    return;
                }
                this.LoadProjectileControllers();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    if (!this.GetCandidate(i).projectileController)
                    {
                        this.RemoveCandidate(i);
                    }
                }
                this.filteredByProjectileControllers = true;
            }

            // Token: 0x06002221 RID: 8737 RVA: 0x000892F4 File Offset: 0x000874F4
            public void FilterByHurtBoxes()
            {
                if (this.filteredByHurtBoxes)
                {
                    return;
                }
                this.LoadHurtBoxes();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    if (!this.GetCandidate(i).hurtBox)
                    {
                        this.RemoveCandidate(i);
                    }
                }
                this.filteredByHurtBoxes = true;
            }

            // Token: 0x06002222 RID: 8738 RVA: 0x00089344 File Offset: 0x00087544
            public void FilterByHurtBoxHealthComponents()
            {
                if (this.filteredByHurtBoxHealthComponents)
                {
                    return;
                }
                this.FilterByHurtBoxes();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    if (!this.GetCandidate(i).hurtBox.healthComponent)
                    {
                        this.RemoveCandidate(i);
                    }
                }
                this.filteredByHurtBoxHealthComponents = true;
            }

            // Token: 0x06002223 RID: 8739 RVA: 0x0008939C File Offset: 0x0008759C
            public void FilterByHurtBoxTeam(TeamMask teamMask)
            {
                this.FilterByHurtBoxes();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    if (!teamMask.HasTeam(candidate.hurtBox.teamIndex))
                    {
                        this.RemoveCandidate(i);
                    }
                }
            }

            // Token: 0x06002224 RID: 8740 RVA: 0x000893E8 File Offset: 0x000875E8
            public void FilterByHurtBoxEntitiesDistinct()
            {
                this.FilterByHurtBoxHealthComponents();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    for (int j = i - 1; j >= 0; j--)
                    {
                        ref BeamSearch.Candidate candidate2 = ref this.GetCandidate(j);
                        if (candidate.hurtBox.healthComponent == candidate2.hurtBox.healthComponent)
                        {
                            this.RemoveCandidate(i);
                            break;
                        }
                    }
                }
            }

            // Token: 0x06002225 RID: 8741 RVA: 0x00089450 File Offset: 0x00087650
            public void FilterByColliderEntities()
            {
                if (this.filteredByEntityLocators)
                {
                    return;
                }
                this.LoadColliderEntityLocators();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    if (!candidate.entityLocator || !candidate.entityLocator.entity)
                    {
                        this.RemoveCandidate(i);
                    }
                }
                this.filteredByEntityLocators = true;
            }

            // Token: 0x06002226 RID: 8742 RVA: 0x000894B4 File Offset: 0x000876B4
            public void FilterByColliderEntitiesDistinct()
            {
                this.FilterByColliderEntities();
                for (int i = this.candidatesCount - 1; i >= 0; i--)
                {
                    ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                    for (int j = i - 1; j >= 0; j--)
                    {
                        ref BeamSearch.Candidate candidate2 = ref this.GetCandidate(j);
                        if (candidate.entityLocator.entity == candidate2.entityLocator.entity)
                        {
                            this.RemoveCandidate(i);
                            break;
                        }
                    }
                }
            }

            // Token: 0x06002227 RID: 8743 RVA: 0x0008951C File Offset: 0x0008771C
            public void OrderByDistance()
            {
                if (this.candidatesCount == 0)
                {
                    return;
                }
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    ref BeamSearch.Candidate ptr = ref this.GetCandidate(0);
                    int i = 1;
                    int num = this.candidatesCount - 1;
                    while (i < num)
                    {
                        ref BeamSearch.Candidate candidate = ref this.GetCandidate(i);
                        if (ptr.distanceSqr > candidate.distanceSqr)
                        {
                            Util.Swap<int>(ref this.candidatesMapping[i - 1], ref this.candidatesMapping[i]);
                            flag = true;
                        }
                        else
                        {
                            ptr = ref candidate;
                        }
                        i++;
                    }
                }
            }

            // Token: 0x06002228 RID: 8744 RVA: 0x00089598 File Offset: 0x00087798
            public HurtBox[] GetHurtBoxes()
            {
                this.FilterByHurtBoxes();
                HurtBox[] array = new HurtBox[this.candidatesCount];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = this.GetCandidate(i).hurtBox;
                }
                return array;
            }

            // Token: 0x06002229 RID: 8745 RVA: 0x000895D8 File Offset: 0x000877D8
            public void GetHurtBoxes(List<HurtBox> dest)
            {
                int num = dest.Count + this.candidatesCount;
                if (dest.Capacity < num)
                {
                    dest.Capacity = num;
                }
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    dest.Add(this.GetCandidate(i).hurtBox);
                }
            }

            // Token: 0x0600222A RID: 8746 RVA: 0x00089628 File Offset: 0x00087828
            public void GetProjectileControllers(List<ProjectileController> dest)
            {
                int num = dest.Count + this.candidatesCount;
                if (dest.Capacity < num)
                {
                    dest.Capacity = num;
                }
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    dest.Add(this.GetCandidate(i).projectileController);
                }
            }

            // Token: 0x0600222B RID: 8747 RVA: 0x00089678 File Offset: 0x00087878
            public void GetColliders(List<Collider> dest)
            {
                int num = dest.Count + this.candidatesCount;
                if (dest.Capacity < num)
                {
                    dest.Capacity = num;
                }
                for (int i = 0; i < this.candidatesCount; i++)
                {
                    dest.Add(this.GetCandidate(i).collider);
                }
            }

            // Token: 0x04001DD5 RID: 7637
            private BeamSearch.Candidate[] candidatesBuffer;

            // Token: 0x04001DD6 RID: 7638
            private int[] candidatesMapping;

            // Token: 0x04001DD7 RID: 7639
            private int candidatesCount;

            // Token: 0x04001DD8 RID: 7640
            private bool hurtBoxesLoaded;

            // Token: 0x04001DD9 RID: 7641
            private bool rootsLoaded;

            // Token: 0x04001DDA RID: 7642
            private bool projectileControllersLoaded;

            // Token: 0x04001DDB RID: 7643
            private bool entityLocatorsLoaded;

            // Token: 0x04001DDC RID: 7644
            private bool filteredByHurtBoxes;

            // Token: 0x04001DDD RID: 7645
            private bool filteredByHurtBoxHealthComponents;

            // Token: 0x04001DDE RID: 7646
            private bool filteredByProjectileControllers;

            // Token: 0x04001DDF RID: 7647
            private bool filteredByEntityLocators;

            // Token: 0x04001DE0 RID: 7648
            public static readonly BeamSearch.SearchData empty = new BeamSearch.SearchData
            {
                candidatesBuffer = Array.Empty<BeamSearch.Candidate>(),
                candidatesMapping = Array.Empty<int>(),
                candidatesCount = 0,
                hurtBoxesLoaded = false
            };
        }
    }
}
