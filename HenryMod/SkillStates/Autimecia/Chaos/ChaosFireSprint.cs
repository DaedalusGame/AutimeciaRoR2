using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.SkillStates
{
    class ChaosFireSprint : BaseSkillState
    {
        protected DamageTrail fireTrail;
        protected Animator animator;

        public float duration;
        public float durationBase = 10f;

        public int projectiles;
        public int projectilesLast;
        public float fireRate;
        public float fireRateBase = 0.25f;

        public override void OnEnter()
        {
            base.OnEnter();

            fireTrail = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/FireTrail"), this.transform).GetComponent<DamageTrail>();
            fireTrail.transform.position = characterBody.footPosition;
            fireTrail.owner = base.gameObject;
            fireTrail.radius *= characterBody.radius * 1f;

            duration = durationBase / attackSpeedStat;
            fireRate = fireRateBase / attackSpeedStat;

            animator = GetModelAnimator();
            characterBody.outOfCombatStopwatch = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();
            GameObject.Destroy(fireTrail.gameObject);
        }

        public override void Update()
        {
            base.Update();

            characterBody.isSprinting = true;
            animator.SetBool("isMoving", true);
            animator.SetBool("isSprinting", true);

            inputBank.moveVector += characterBody.characterDirection.forward * 2f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            projectiles = Mathf.FloorToInt(fixedAge / fireRate);
            FireProjectileGroup();
            projectilesLast = projectiles;

            if (fireTrail)
            {
                fireTrail.damagePerSecond = damageStat * 3.0f;
            }

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireProjectileGroup()
        {
            float min = 15f;
            float max = 30f;
            for (int i = projectilesLast; i < projectiles; i++)
            {
                float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                Vector3 forward2 = Vector3.up;
                forward2.x += Mathf.Sin(angle);
                forward2.z += Mathf.Cos(angle);
                float speedOverride2 = UnityEngine.Random.Range(min, max);

                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FireMeatBall"),
                    position = characterBody.corePosition,
                    rotation = Util.QuaternionSafeLookRotation(forward2),
                    procChainMask = default(ProcChainMask),
                    target = null,
                    owner = gameObject,
                    damage = damageStat * 2f,
                    crit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    force = 200f,
                    damageColorIndex = DamageColorIndex.Default,
                    speedOverride = speedOverride2,
                    useSpeedOverride = true
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
