using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.Modules.ProjectileComponents
{
    class ProjectileBounce : MonoBehaviour, IProjectileImpactBehavior
    {
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private ProjectileTargetComponent projectileTarget;

        private Rigidbody rb;

        public float bounceForce = 1200;
        public int bouncesLeft = 15;
        public float lifetime;
        public float forwardSpeed;
        public float homingFactor = 0.01f;
        public float homingSpeed = 20f;
        public float randomBounceVelocity = 5f;

        float stopwatch;

        bool alive = true;

        public GameObject bounceEffectPrefab;
        public GameObject fadeEffectPrefab;

        private void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            projectileController = gameObject.GetComponent<ProjectileController>();
            projectileDamage = gameObject.GetComponent<ProjectileDamage>();
            projectileTarget = gameObject.GetComponent<ProjectileTargetComponent>();
        }

        protected void OnEnable()
        {
            this.SetForwardSpeed(this.forwardSpeed);
        }

        protected void Start()
        {
            this.SetForwardSpeed(this.forwardSpeed);
        }

        protected void SetForwardSpeed(float speed)
        {
            if (this.rb)
            {
                this.rb.velocity = this.transform.forward * speed;
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!alive)
            {
                return;
            }
            Collider collider = impactInfo.collider;
            if (collider)
            {
                HurtBox component = collider.GetComponent<HurtBox>();
                if (!component) //World collision
                {
                    if (impactInfo.estimatedImpactNormal.y > 0)
                        this.rb.AddForce(-Physics.gravity + new Vector3(0, bounceForce - rb.velocity.y, 0), ForceMode.Acceleration);
                    float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                    this.rb.velocity += new Vector3(Mathf.Sin(angle),0,Mathf.Cos(angle)) * randomBounceVelocity;
                    this.bouncesLeft--;
                    lifetime += 0.5f;
                    if (bounceEffectPrefab)
                    {
                        EffectManager.SpawnEffect(bounceEffectPrefab, new EffectData()
                        {
                            origin = impactInfo.estimatedPointOfImpact,
                            rotation = Quaternion.LookRotation(impactInfo.estimatedImpactNormal, Vector3.up),
                            scale = 3f,
                        }, true);
                    }
                }

                if (this.bouncesLeft <= 0)
                {
                    FadeEffect();
                    alive = false;
                }
            }

            if (!alive)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
   
        private void FixedUpdate()
        {
            if (!alive)
            {
                return;
            }

            stopwatch += Time.deltaTime;
            if (this.projectileTarget.target)
            {
                Vector3 vector = this.projectileTarget.target.transform.position - this.transform.position;
                if (vector != Vector3.zero)
                {
                    float lerp = homingFactor;

                    var currentVelocity = rb.velocity;
                    var homingVelocity = vector.normalized * homingSpeed;

                    currentVelocity.y = 0;
                    homingVelocity.y = 0;

                    Vector3 newVelocity = currentVelocity * (1 - lerp) + homingVelocity * lerp;
                    newVelocity.y = rb.velocity.y;
                    rb.velocity = newVelocity;
                }
            }

            if (stopwatch > lifetime)
            {
                FadeEffect();
                alive = false;
            }

            if(!alive)
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }

        private void FadeEffect()
        {
            if (fadeEffectPrefab)
            {
                EffectManager.SpawnEffect(fadeEffectPrefab, new EffectData()
                {
                    origin = transform.position,
                    rotation = transform.rotation,
                    scale = 3f,
                }, true);
            }
        }
    }
}
