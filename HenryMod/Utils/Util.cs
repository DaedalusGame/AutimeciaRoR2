using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class MiscUtil
    {
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
    }
}
