using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    public class DebugLifetimeComponent : MonoBehaviour
    {
        private void Awake() { AutimeciaPlugin.ModLogger.LogInfo($"{name}.Awake()"); }
        private void OnDestroy() { AutimeciaPlugin.ModLogger.LogInfo($"{name}.OnDestroy()"); }
        private void OnEnable() { AutimeciaPlugin.ModLogger.LogInfo($"{name}.OnEnable()"); }
        private void OnDisable() {
            var projSimple = gameObject.GetComponent<ProjectileSimple>();
            if (projSimple)
                AutimeciaPlugin.ModLogger.LogInfo($"Projectile Lifetime: {projSimple.stopwatch}/{projSimple.lifetime}");
            AutimeciaPlugin.ModLogger.LogInfo($"{name}.OnDisable()");
        }
    }
}
