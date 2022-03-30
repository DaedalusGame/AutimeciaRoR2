using System;
using System.Collections.Generic;
using System.Text;
using Autimecia.Utils;
using BepInEx.Configuration;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace Autimecia.Modules.Survivors
{
    class Autimecia : SurvivorBase
    {
        internal override string bodyName { get; set; } = "Autimecia";

        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal override float sortPosition { get; set; } = 100f;

        internal override ConfigEntry<bool> characterEnabled { get; set; }

        internal override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            armor = 20f,
            armorGrowth = 0f,
            bodyName = "AutimeciaBody",
            bodyNameToken = AutimeciaPlugin.developerPrefix + "_AUTIMECIA_BODY_NAME",
            bodyColor = Color.grey,
            characterPortrait = Modules.Assets.LoadCharacterIcon("Autimecia"),
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            damage = 12f,
            healthGrowth = 33f,
            healthRegen = 1.5f,
            jumpCount = 3,
            maxHealth = 110f,
            subtitleNameToken = AutimeciaPlugin.developerPrefix + "_AUTIMECIA_BODY_SUBTITLE",
            podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
        };

        internal static Material bodyMat = Modules.Assets.LoadMaterial("matBody");
        internal static Material faceMat = Modules.Assets.LoadMaterial("matFaceTest");
        internal static Material hatMat = Modules.Assets.LoadMaterial("matHat");
        internal static Material hatWingsMat = Modules.Assets.LoadMaterial("matHatWings");
        internal static Material scarfMat = Modules.Assets.LoadMaterial("matScarf");
        internal static Material wingsMat = Modules.Assets.LoadMaterial("matWings");
        internal override int mainRendererIndex { get; set; } = 2;

        internal override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
            new CustomRendererInfo
            {
                childName = "Face",
                material = faceMat
            },
            new CustomRendererInfo
            {
                childName = "Hat",
                material = hatMat
            },
            new CustomRendererInfo
            {
                childName = "HatWings",
                material = hatWingsMat
            },
            new CustomRendererInfo
            {
                childName = "Scarf",
                material = scarfMat
            },
            new CustomRendererInfo
            {
                childName = "Wings",
                material = wingsMat
            },
            new CustomRendererInfo
            {
                childName = "Body",
                material = bodyMat
            },
        };

        internal override Type characterMainState { get; set; } = typeof(EntityStates.GenericCharacterMain);

        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; }

        public static SkillDef brokenSkillDef;

        public Autimecia()
        {
            characterMainState = typeof(AutimeciaCharacterState);
        }

        internal override void InitializeCharacter()
        {
            base.InitializeCharacter();

            var tracker = bodyPrefab.AddComponent<AutimeciaTracker>();
            tracker.enabled = false;
            tracker.maxTrackingDistance = 60f;
            tracker.maxTrackingAngle = 30f;
            tracker.trackerUpdateFrequency = 10f;

            var faustController = bodyPrefab.AddComponent<FaustControllerComponent>();
        }

        internal override void InitializeDoppelganger()
        {
            base.InitializeDoppelganger();
        }

        internal override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
        }

        internal override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            string prefix = AutimeciaPlugin.developerPrefix;

            SkillDef fedoraBeamSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.FedoraSpeen)), "Weapon", prefix + "_AUTIMECIA_BODY_PRIMARY_FEDORA_BEAM_NAME", prefix + "_AUTIMECIA_BODY_PRIMARY_FEDORA_BEAM_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"), true);

            SkillDef faustSkillDef = Modules.Skills.CreateSkillDef<SkillDef>(new SkillDefInfo
            {
                skillName = prefix + "_AUTIMECIA_BODY_SECONDARY_FAUST_NAME",
                skillNameToken = prefix + "_AUTIMECIA_BODY_SECONDARY_FAUST_NAME",
                skillDescriptionToken = prefix + "_AUTIMECIA_BODY_SECONDARY_FAUST_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FaustCharge)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });
            SkillDef chaosSkillDef = Modules.Skills.CreateSkillDef<SkillDef>(new SkillDefInfo
            {
                skillName = prefix + "_AUTIMECIA_BODY_SPECIAL_CHAOS_NAME",
                skillNameToken = prefix + "_AUTIMECIA_BODY_SPECIAL_CHAOS_NAME",
                skillDescriptionToken = prefix + "_AUTIMECIA_BODY_SPECIAL_CHAOS_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ChaosYuurei)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            Modules.Skills.AddPrimarySkill(bodyPrefab, fedoraBeamSkillDef);
            Modules.Skills.AddSecondarySkills(bodyPrefab, faustSkillDef);
            Modules.Skills.AddUtilitySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.VoidSkill)), "Weapon", prefix + "_HENRY_BODY_PRIMARY_SLASH_NAME", prefix + "_HENRY_BODY_PRIMARY_SLASH_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"), true));
            Modules.Skills.AddSpecialSkill(bodyPrefab, chaosSkillDef);

            brokenSkillDef = Modules.Skills.CreateSkillDef<BrokenSkillDef>(new SkillDefInfo
            {
                skillName = prefix + "_BROKEN_NAME",
                skillNameToken = prefix + "_BROKEN_NAME",
                skillDescriptionToken = prefix + "_BROKEN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.VoidSkill)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {}
            });

            //TODO: skills
        }

        internal override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(AutimeciaPlugin.developerPrefix + "_AUTIMECIA_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texAutimeciaIcon"),
                defaultRenderers,
                mainRenderer,
                model);

            /*defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenrySword"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenryGun"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHenry"),
                    renderer = defaultRenderers[instance.mainRendererIndex].renderer
                }
            };*/

            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }


        internal override void SetItemDisplays()
        {
            itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }
    }
}
