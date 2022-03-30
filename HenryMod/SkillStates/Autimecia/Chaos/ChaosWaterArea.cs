using Autimecia.Modules;
using Autimecia.Utils;
using EntityStates;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Autimecia.SkillStates
{
    class ChaosWaterArea : BaseSkillState
    {
        protected Animator animator;
        public float duration;
        public float durationBase = 3f;

        public float durationAnim;
        public float durationAnimBase = 3f;

        public float fireThreshold = 0.355f;
        bool fired;

        public float radius = 20f;
        public float damageMultiplier = 12f;

        SphereSearch sphereSearch;
       

        public override void OnEnter()
        {
            base.OnEnter();

            sphereSearch = new SphereSearch()
            {
                mask = LayerIndex.entityPrecise.mask,
                radius = radius,
                queryTriggerInteraction = QueryTriggerInteraction.Collide,
            };

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
                fired = true;
                FireWave();
                this.SetBodyState(new FreezeState() { duration = duration - fixedAge });
            }

            if(fired && characterMotor) 
                characterMotor.velocity = Vector3.zero;

            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireWave()
        {
            var explosionEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AffixWhiteExplosion");
            List<HurtBox> foundHurtBoxes = new List<HurtBox>();

            EffectManager.SpawnEffect(explosionEffect, new EffectData
            {
                origin = characterBody.corePosition,
                rotation = Util.QuaternionSafeLookRotation(base.transform.forward),
                scale = radius
            }, true);

            sphereSearch.origin = characterBody.corePosition;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            sphereSearch.OrderCandidatesByDistance();
            sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
            sphereSearch.GetHurtBoxes(foundHurtBoxes);

            foreach(var hurtbox in foundHurtBoxes)
            {
                DamageHurtbox(hurtbox);
            }
        }

        private void DamageHurtbox(HurtBox hurtbox)
        {
            if (!hurtbox)
                return;

            var healthComponent = hurtbox.healthComponent;
            if (healthComponent)
            {
                if (healthComponent.gameObject == gameObject)
                {
                    return;
                }
                if (FriendlyFireManager.ShouldDirectHitProceed(healthComponent, teamComponent.teamIndex))
                {
                    var orb = new OrbIcicle();
                    orb.speed = 5f;
                    orb.origin = characterBody.corePosition;
                    orb.target = hurtbox;
                    orb.damageValue = damageStat * damageMultiplier;
                    orb.isCrit = Util.CheckRoll(critStat, characterBody.master);
                    orb.teamIndex = teamComponent.teamIndex;
                    orb.attacker = gameObject;
                    orb.procCoefficient = 1.0f;
                    orb.freezeTime = 5f;
                    orb.healMultiplier = 0.10f;
                    OrbManager.instance.AddOrb(orb);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
