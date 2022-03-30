using RoR2;
using RoR2.Navigation;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EntityStates;

namespace Autimecia.Utils
{
    static class MiscUtil
    {
        /// <summary>
        /// A helper that will set up the RendererInfos of a GameObject that you pass in.
        /// <para>This allows it to go invisible when your character is not visible, as well as letting overlays affect it.</para>
        /// </summary>
        /// <param name="obj">The GameObject/Prefab that you wish to set up RendererInfos for.</param>
        /// <param name="debugmode">Do we attempt to attach a material shader controller instance to meshes in this?</param>
        /// <returns>Returns an array full of RendererInfos for GameObject.</returns>
        public static CharacterModel.RendererInfo[] ItemDisplaySetup(GameObject obj, bool debugmode = false)
        {

            List<Renderer> AllRenderers = new List<Renderer>();

            var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length > 0) { AllRenderers.AddRange(meshRenderers); }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderers.Length > 0) { AllRenderers.AddRange(skinnedMeshRenderers); }

            CharacterModel.RendererInfo[] renderInfos = new CharacterModel.RendererInfo[AllRenderers.Count];

            for (int i = 0; i < AllRenderers.Count; i++)
            {
                if (debugmode)
                {
                    var controller = AllRenderers[i].gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                    controller.Renderer = AllRenderers[i];
                }

                renderInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = AllRenderers[i] is SkinnedMeshRenderer ? AllRenderers[i].sharedMaterial : AllRenderers[i].material,
                    renderer = AllRenderers[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false //We allow the mesh to be affected by overlays like OnFire or PredatoryInstinctsCritOverlay.
                };
            }

            return renderInfos;
        }

        public static void FilterOutFaustBargains(this BullseyeSearch search)
        {
            search.candidatesEnumerable = search.candidatesEnumerable.Where(x => !HasFaust(x.hurtBox));
        }

        private static bool HasFaust(HurtBox hurtBox)
        {
            if(hurtBox && hurtBox.healthComponent)
            {
                return hurtBox.healthComponent.GetComponent<FaustComponent>();
            }
            return false;
        }

        public static void SetBodyState(this EntityState currentState, EntityState state)
        {
            var bodyMachine = EntityStateMachine.FindByCustomName(currentState.gameObject, "Body");

            if(bodyMachine)
                bodyMachine.SetNextState(state);
        }

        public static Vector3? RaycastToDirection(Vector3 position, float maxDistance, Vector3 direction)
        {
            if (Physics.Raycast(new Ray(position, direction), out RaycastHit raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                return raycastHit.point;
            }
            return null;
        }

        public static Vector3? FindClosestGroundNodeToPosition(Vector3 position, HullClassification hullClassification)
        {
            Vector3 ResultPosition;

            NodeGraph groundNodes = SceneInfo.instance.groundNodes;

            var closestNode = groundNodes.FindClosestNode(position, hullClassification);

            if (closestNode != NodeGraph.NodeIndex.invalid)
            {
                groundNodes.GetNodePosition(closestNode, out ResultPosition);
                return ResultPosition;
            }

            return null;
        }

        public static Vector3? AboveTargetBody(CharacterBody body, float distanceAbove)
        {
            if (!body) { return null; }

            var closestPointOnBounds = body.mainHurtBox.collider.ClosestPointOnBounds(body.transform.position + new Vector3(0, 10000, 0));

            var raycastPoint = RaycastToDirection(closestPointOnBounds, distanceAbove, Vector3.up);
            if (raycastPoint.HasValue)
            {
                return raycastPoint.Value;
            }
            else
            {
                return closestPointOnBounds + (Vector3.up * distanceAbove);
            }
        }

        public static T GetFirstOfType<T>(this IEnumerable enumerable)
        {
            return enumerable.OfType<T>().First();
        }
    }
}
