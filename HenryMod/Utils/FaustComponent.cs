using Autimecia.Modules;
using RoR2;
using RoR2.CharacterSpeech;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Utils
{
    class FaustControllerComponent : MonoBehaviour
    {
        List<FaustComponent> activeBargains = new List<FaustComponent>();
        int maxBargains = 4;

        public void AddBargain(FaustComponent faust)
        {
            activeBargains.Add(faust);
            if(activeBargains.Count > maxBargains)
            {
                var bargain = activeBargains[0];
                GameObject.Destroy(bargain);
                activeBargains.RemoveAt(0);
            }
            activeBargains.RemoveAll(x => !x.enabled);
        }

        internal void RemoveBargain(FaustComponent faustComponent)
        {
            activeBargains.Remove(faustComponent);
        }
    }

    class FaustComponent : MonoBehaviour
    {
        GameObject hatAttachment;
        public GameObject attacker;

        CharacterBody characterBody;
        FaustControllerComponent faustController;
        SkillLocator skillLocator;
        Inventory inventory;

        float stopwatch;
        public float duration = float.PositiveInfinity;

        int skillSealSeed;

        public void OnEnable()
        {
            characterBody = gameObject.GetComponent<CharacterBody>();
            skillLocator = gameObject.GetComponent<SkillLocator>();
            if(characterBody)
                inventory = characterBody.inventory;

            SecretKingDialog();

            skillSealSeed = UnityEngine.Random.Range(0, 10000);

            if (this.skillLocator)
            {
                var skills = new List<GenericSkill>() { this.skillLocator.primary, this.skillLocator.secondary, this.skillLocator.special, this.skillLocator.utility };
                skills.RemoveAll(x => !x);
                if (skills.Count > 0) {
                    var skill = skills[skillSealSeed % skills.Count];
                    skill.SetSkillOverride(this, Modules.Survivors.Autimecia.brokenSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                }
            }

            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;

            if(inventory)
                inventory.GiveItem(Items.Faust);
        }

        private void SecretKingDialog()
        {
            if(gameObject.name.ToLowerInvariant().StartsWith("brother"))
            {
                var speechDriver = GameObject.FindObjectOfType<BrotherSpeechDriver>();
            
                if(speechDriver)
                {
                    bool isHurt = speechDriver.gameObject.name.ToLowerInvariant().Contains("hurt");
                    speechDriver.characterSpeechController.EnqueueSpeech(new CharacterSpeechController.SpeechInfo()
                    {
                        token = isHurt ? "BROTHERHURT_AUTIMECIA_FAUST" : "BROTHER_AUTIMECIA_FAUST",
                        duration = 2,
                        maxWait = 0.5f,
                        priority = 10000,
                        mustPlay = true,
                    });
                }
            }
        }

        public void Start()
        {
            faustController = attacker.GetComponent<FaustControllerComponent>();

            if (faustController)
            {
                faustController.AddBargain(this);
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            if (damageReport.victim.gameObject == gameObject && attacker && damageReport.attackerBody && damageReport.attacker == attacker)
            {
                DropExtraGold(damageReport.attackerBody);
            }
        }

        public void DropExtraGold(CharacterBody attackerBody)
        {
            var corePosition = characterBody.corePosition;
            var rewards = gameObject.GetComponent<DeathRewards>();

            if (rewards)
            {
                var goldRewarded = (uint)Math.Max(1, rewards.goldReward * StaticValues.faustRewardPerHit);
                var expRewarded = (uint)Math.Max(0, rewards.expReward * StaticValues.faustRewardPerHit);

                ExperienceManager.instance.AwardExperience(corePosition, attackerBody, expRewarded);

                if (attackerBody.master)
                {
                    attackerBody.master.GiveMoney(goldRewarded);
                    EffectManager.SpawnEffect(DeathRewards.coinEffectPrefab, new EffectData
                    {
                        origin = corePosition,
                        genericFloat = goldRewarded,
                        scale = characterBody.radius
                    }, true);
                }
            }
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;

            if (stopwatch >= duration)
            {
                GameObject.Destroy(this);
            }
        }

        public void OnDisable()
        {
            if (faustController)
            {
                faustController.RemoveBargain(this);
            }

            if (this.skillLocator)
            {
                var skills = new List<GenericSkill>() { this.skillLocator.primary, this.skillLocator.secondary, this.skillLocator.special, this.skillLocator.utility };
                skills.RemoveAll(x => !x);
                if (skills.Count > 0)
                {
                    var skill = skills[skillSealSeed % skills.Count];
                    skill.UnsetSkillOverride(this, Autimecia.Modules.Survivors.Autimecia.brokenSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                }
            }

            GlobalEventManager.onServerDamageDealt -= GlobalEventManager_onServerDamageDealt;

            if (inventory)
                inventory.RemoveItem(Items.Faust);
        }
    }
}
