using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class ProjectileIgnoreSalvo : MonoBehaviour
    {
        public List<Collider> salvoColliders;        

        public void Start()
        {
            var colliders = GetComponentsInChildren<Collider>();

            if(salvoColliders == null)
            {
                AutimeciaPlugin.ModLogger.LogError("Projectile has no salvo collider list");
                return;
            }

            foreach(var collider in colliders)
            {
                foreach (var other in salvoColliders)
                    Physics.IgnoreCollision(collider, other);
                salvoColliders.Add(collider);
            }
        }
    }
}
