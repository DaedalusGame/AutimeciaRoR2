using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Autimecia.SkillStates
{
    class BaseHatState : BaseState
    {
        protected ProjectileDamage projectileDamage;
        protected ProjectileGhostController projectileGhostController;

        public override void OnEnter()
        {
            base.OnEnter();

            projectileGhostController = projectileController.ghost;
            projectileDamage = GetComponent<ProjectileDamage>();

            if (modelLocator && projectileGhostController)
            {
                modelLocator.modelBaseTransform = projectileGhostController.transform;
                modelLocator.modelTransform = modelLocator.modelBaseTransform;
            }
        }
    }
}
