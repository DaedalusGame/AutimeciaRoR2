using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosFireArea : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 3.3f;

        public float durationAnim;
        public float durationAnimBase = 3f;

        public float fireThreshold = 0.355f;
        bool fired;

        public float explosionStopwatch;
        public float explosionRate = 0.03f;
        int explosionCount;

        public float damageCoefficient = 10f;
        public float force = 0;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = durationBase / attackSpeedStat;
            durationAnim = durationAnimBase / attackSpeedStat;

            animator = GetModelAnimator();
            StartAimMode(2f + duration, false);
            characterBody.outOfCombatStopwatch = 0f;
            animator.SetBool("attacking", true);

            PlayCrossfade("FullBody, Override", "ChaosCast3", "Chaos.playbackRate", durationAnim, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("attacking", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge / durationAnim >= fireThreshold && !fired)
            {
                FireExplosion();
                fired = true;
                this.SetBodyState(new FreezeState() { duration = duration - fixedAge });
            }

            if(fired)
            {
                explosionStopwatch += Time.fixedDeltaTime;
                if (explosionStopwatch >= explosionRate)
                {
                    FireExplosion();
                    explosionStopwatch -= explosionRate;
                }
                if (characterMotor)
                    characterMotor.velocity = Vector3.zero;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireExplosion()
        {
            if (base.isAuthority)
            {
                Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.4f, 1f), UnityEngine.Random.Range(-1f, 1f));
                randomOffset.Normalize();
                float distance = UnityEngine.Random.Range(3f, 5f) + 1f * explosionCount;
                Vector3 impactPos;
                RaycastHit initialHit;
                if (Physics.Raycast(new Ray(characterBody.corePosition, randomOffset), out initialHit, distance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                    impactPos = initialHit.point - randomOffset * 1.5f;
                else
                    impactPos = base.characterBody.corePosition + randomOffset * distance;

                BlastAttack blastAttack = new BlastAttack()
                {
                    position = impactPos,
                    radius = 5,
                    attacker = gameObject,
                    inflictor = gameObject,
                    baseDamage = damageStat * damageCoefficient,
                    damageType = DamageType.IgniteOnHit,
                    baseForce = 1000,
                    procCoefficient = 0.7f,
                    crit = Util.CheckRoll(critStat, characterBody.master),
                    attackerFiltering = AttackerFiltering.NeverHit,
                    teamIndex = teamComponent.teamIndex,
                    losType = BlastAttack.LoSType.None,
                    falloffModel = BlastAttack.FalloffModel.Linear,
                };
                blastAttack.Fire();
                EffectManager.SpawnEffect(Projectiles.effectFireExplosion, new EffectData()
                {
                    origin = impactPos,
                    scale = UnityEngine.Random.Range(2f, 4f),
                }, true);

                RaycastHit rayHit;
                if (explosionCount % 4 == 0 && Physics.Raycast(new Ray(impactPos, Vector3.down), out rayHit, 3f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    var trueImpact = rayHit.point;
                    Vector3 forward = Util.ApplySpread(Vector3.forward, 0f, 0f, 1f, 1f, 0, 0f);
                    FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
                    fireProjectileInfo.projectilePrefab = Projectiles.chaosFireField;
                    fireProjectileInfo.position = trueImpact;
                    fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(forward);
                    fireProjectileInfo.owner = base.gameObject;
                    fireProjectileInfo.damage = this.damageStat * damageCoefficient * 0.25f;
                    fireProjectileInfo.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }

                explosionCount++;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
