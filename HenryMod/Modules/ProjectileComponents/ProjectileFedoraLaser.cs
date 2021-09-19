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
    [RequireComponent(typeof(Animator))]
    class ProjectileFedoraLaser : MonoBehaviour
    {
        public float lifetime = 5f;
        public float movetime = 1f;
        public float firetime = 1.3f;
        public float spintime;
        public bool moved;
        public bool fired;

        public Vector3 startPosition;
        public Vector3 endPosition;
        public float distance = 100;
        public AnimationCurve moveCurve;

        public Animator animator;
        public Transform transform;

        public BeamSimple beamSimple;

        public float stopwatch;

        Dictionary<HealthComponent, float> lastHit = new Dictionary<HealthComponent, float>();

        BeamSimple beam;
        public float angleVelocity = 0;
        public bool spinning = false;

        private void Awake()
        {
            transform = base.transform;
            animator = gameObject.GetComponent<Animator>();
            beam = gameObject.GetComponentInChildren<BeamSimple>();
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
            spintime = stopwatch;
        }

        public void FixedUpdate()
        {
            if (!moved && stopwatch >= movetime)
            {
                animator.CrossFadeInFixedTime("TrilbyBeam", 0.1f, animator.GetLayerIndex("Base"));
                moved = true;
            }

            if (!fired && stopwatch >= firetime)
            {
                animator.Play("TrilbyFire", animator.GetLayerIndex("Base"));
                beam.Fire(AnimationCurve.EaseInOut(0, 0, 0.1f, 1));
                beam.Finish(AnimationCurve.EaseInOut(0, 1, 0.4f, 0), lifetime - stopwatch - 0.4f);
                fired = true;
            }

            if (stopwatch >= lifetime)
            {
                animator.SetBool("disappear", true);
                var layerIndex = animator.GetLayerIndex("Base");
                var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                if (stateInfo.IsTag("disappear") && stateInfo.normalizedTime >= 1)
                    UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
}
