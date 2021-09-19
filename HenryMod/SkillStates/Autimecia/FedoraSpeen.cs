using Autimecia.Modules.ProjectileComponents;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class FedoraSpeen : BaseSkillState
    {
        private const float fireThreshold = 0.59f;

        protected Animator animator;
        public float durationBase = 0.7f;
        public float duration;
        public bool fired;

        ProjectileFedoraLaser fedoraLaser;

        SphereSearch sphereSearch;

        public FedoraSpeen()
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            duration = durationBase / attackSpeedStat;

            var aimRay = GetAimRay();
            List<ProjectileController> projectileControllers = new List<ProjectileController>();
            sphereSearch = new SphereSearch
            {
                mask = LayerIndex.fakeActor.mask,
                origin = aimRay.GetPoint(4f),
                queryTriggerInteraction = QueryTriggerInteraction.Collide,
                radius = 3f,
            };
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByProjectileControllers();
            sphereSearch.GetProjectileControllers(projectileControllers);
            foreach(var projectileController in projectileControllers)
            {
                var checkFedora = projectileController.gameObject.GetComponent<ProjectileFedoraLaser>();
                if (checkFedora && projectileController.owner == characterBody.gameObject && !checkFedora.spinning)
                {
                    fedoraLaser = checkFedora;
                    break;
                }
            }

            if (fedoraLaser == null)
            {
                outer.SetNextState(new FedoraBeam());
            }
            else
            {
                animator = GetModelAnimator();
                StartAimMode(0.5f + duration, false);
                characterBody.outOfCombatStopwatch = 0f;
                animator.SetBool("attacking", true);

                PlayCrossfade("FullBody, Override", "Cast1", "Cast1.playbackRate", duration, 0.05f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if(animator)
                animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge / duration >= fireThreshold && !fired)
            {
                fired = true;
                SpinFedora();
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        private void SpinFedora()
        {
            if (fedoraLaser.spinning)
                return;

            var aim = GetAimRay();
            var delta = fedoraLaser.gameObject.transform.position - characterBody.corePosition;

            var angle = Vector3.SignedAngle(delta, aim.direction, Vector3.up);

            if(angle < 0)
            {
                fedoraLaser.Spin(-45f);
            }
            else
            {
                fedoraLaser.Spin(45f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
