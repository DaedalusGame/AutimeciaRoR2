using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.Utils
{
    class ProjectileSalvoHelper
    {
        static bool capture;
        static List<ProjectileController> projectiles = new List<ProjectileController>();

        public static void Init()
        {
            On.RoR2.Projectile.ProjectileManager.InitializeProjectile += ProjectileManager_InitializeProjectile;
        }

        public static void StartSalvo()
        {
            capture = true;
        }

        public static IEnumerable<ProjectileController> EndSalvo()
        {
            var toReturn = projectiles.ToArray();
            capture = false;
            projectiles.Clear();
            return toReturn;
        }

        private static void ProjectileManager_InitializeProjectile(On.RoR2.Projectile.ProjectileManager.orig_InitializeProjectile orig, ProjectileController projectileController, FireProjectileInfo fireProjectileInfo)
        {
            orig(projectileController, fireProjectileInfo);
            if (capture)
                projectiles.Add(projectileController);
        }
    }
}
