using Autimecia.Utils;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.Modules.ProjectileComponents
{
    [RequireComponent(typeof(ProjectileController))]
    class ProjectileFedoraLaser : MonoBehaviour
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public float distance = 100;
        public AnimationCurve moveCurve;

        public float stopwatch;
        public float spintime;

        public float angleVelocity = 0;
        public bool spinning = false;

        private void Awake()
        {
        }

        protected void OnEnable()
        {
            startPosition = transform.position;
            
            var delta = transform.forward * distance;
            RaycastHit raycastHit;
            var rayDist = distance;
            if(Physics.SphereCast(new Ray(startPosition, delta), 0.5f, out raycastHit, distance, LayerIndex.world.mask))
            {
                rayDist = raycastHit.distance;
            }

            endPosition = startPosition + transform.forward * rayDist;
        }

        public void Spin(float n)
        {
            spinning = true;
            spintime = stopwatch;
            angleVelocity = n;
        }

        public void Update()
        {
            stopwatch += Time.deltaTime;
            transform.position = startPosition + (endPosition - startPosition) * moveCurve.Evaluate(stopwatch);
            transform.Rotate(Vector3.up, angleVelocity * Time.deltaTime);
        }
    }
}
