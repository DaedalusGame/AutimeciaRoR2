using Autimecia.Utils;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Autimecia.Modules
{
    class Items
    {
        static string prefix = AutimeciaPlugin.developerPrefix + "_ITEM_";

        public static ItemDef Faust;

        public static void Initialize()
        {
            Faust = ScriptableObject.CreateInstance<ItemDef>();
            Faust.name = prefix + "FAUST";
            Faust.nameToken = prefix + "FAUST_NAME";
            Faust.pickupToken = prefix + "FAUST_PICKUP";
            Faust.descriptionToken = prefix + "FAUST_DESCRIPTION";
            Faust.loreToken = prefix + "FAUST_LORE";
            Faust.pickupModelPrefab = new GameObject();
            Faust.pickupIconSprite = null;
            Faust.hidden = true;
            Faust.canRemove = false;
            Faust.tier = ItemTier.NoTier;

            var ItemBodyModelPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("FaustDisplay");
            Projectiles.MakeFedoraDullRainbow(ItemBodyModelPrefab, 0.5f);
            var itemDisplay = ItemBodyModelPrefab.AddComponent<RoR2.ItemDisplay>();
            itemDisplay.rendererInfos = MiscUtil.ItemDisplaySetup(ItemBodyModelPrefab);


            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlAutimecia", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HatBone",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0F),
                    localScale = new Vector3(6F, 6F, 6F)
                },
            });
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.31581F, -0.00002F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(2F, 2F, 2F)
                },
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.268F, -0.00002F),
                    localAngles = new Vector3(10F, 0F, 0F),
                    localScale = new Vector3(2F, 2F, 2F)
                },
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.18332F, 2.82469F, 1.46699F),
                    localAngles = new Vector3(300F, 180F, 10F),
                    localScale = new Vector3(10F, 10F, 10F)
                },
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0F, 0.14441F, 0.09523F),
                    localAngles = new Vector3(30F, 0F, 0F),
                    localScale = new Vector3(1F, 1F, 1F)
                },
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.02416F, 0.14425F, 0.00324F),
                    localAngles = new Vector3(5F, 0F, 10F),
                    localScale = new Vector3(3F, 3F, 3F)
                },
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.23322F, 0.06644F),
                    localAngles = new Vector3(25F, 0F, 0F),
                    localScale = new Vector3(1.5F, 1.5F, 1.5F)
                },
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0.15719F, 1.54412F, 0.16778F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(14F, 25F, 10F)
                },
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.00232F, 0.20873F, 0.03012F),
                    localAngles = new Vector3(10F, 0F, 350F),
                    localScale = new Vector3(1.5F, 2F, 1.5F)
                },
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 3.91489F, 1.65952F),
                    localAngles = new Vector3(278.3512F, 0F, 180F),
                    localScale = new Vector3(10F, 10F, 10F)
                },
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.01024F, 0.26534F, -0.02854F),
                    localAngles = new Vector3(326.567F, 0F, 0F),
                    localScale = new Vector3(2F, 3F, 2F)
                },
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.03061F, 0.2088F, 0.01693F),
                    localAngles = new Vector3(5F, 0F, 340F),
                    localScale = new Vector3(1F, 1F, 1F)
                },
            });
            rules.Add("mdlBeetle", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.50721F, 0.43642F),
                    localAngles = new Vector3(334.1324F, 180F, 0F),
                    localScale = new Vector3(6F, 6F, 6F)
                },
            });
            rules.Add("mdlBeetleGuard", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.00001F, -0.43915F, 2.30031F),
                    localAngles = new Vector3(300F, 0.00001F, 180F),
                    localScale = new Vector3(8F, 40F, 8F)
                },
            });
            rules.Add("mdlBeetleQueen", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 3.84075F, 0.71141F),
                    localAngles = new Vector3(10F, 180F, 0F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlBell", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chain",
                    localPos = new Vector3(0F, 0F, 0F),
                    localAngles = new Vector3(355.8196F, 240.6027F, 188.2107F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlBison", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.14374F, 0.63656F),
                    localAngles = new Vector3(277.6974F, 180F, 0F),
                    localScale = new Vector3(3F, 3F, 3F)
                },
            });
            rules.Add("mdlBrother", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.00001F, 0.07384F, 0.21033F),
                    localAngles = new Vector3(76.38377F, 0F, 0F),
                    localScale = new Vector3(1F, 1F, 1F)
                },
            });
            rules.Add("mdlClayBoss", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "PotLidTop",
                    localPos = new Vector3(0F, 0.65998F, 1.30412F),
                    localAngles = new Vector3(5F, 0F, 0F),
                    localScale = new Vector3(24F, 24F, 24F)
                },
            });
            rules.Add("mdlClayBruiser", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00001F, 0.47472F, 0.09709F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(4F, 4F, 4F)
                },
            });
            rules.Add("mdlMagmaWorm", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.48357F, 0.52811F),
                    localAngles = new Vector3(84.25481F, 180F, 180F),
                    localScale = new Vector3(12F, 12F, 12F)
                },
            });
            rules.Add("mdlGolem", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.84154F, -0.34857F),
                    localAngles = new Vector3(337.5F, 0F, 0F),
                    localScale = new Vector3(15F, 15F, 15F)
                },
            });
            rules.Add("mdlGrandparent", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.00001F, 0.19052F, -1.51033F),
                    localAngles = new Vector3(14.06001F, 0F, 0F),
                    localScale = new Vector3(40F, 50F, 40F)
                },
            });
            rules.Add("mdlGravekeeper", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00001F, 2.56513F, -0.72038F),
                    localAngles = new Vector3(347.964F, 0F, 0F),
                    localScale = new Vector3(15F, 16F, 15F)
                },
            });
            rules.Add("mdlGreaterWisp", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MaskBase",
                    localPos = new Vector3(0F, 0.87668F, 0F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(12F, 12F, 12F)
                },
            });
            rules.Add("mdlHermitCrab", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0.01153F, 1.97345F, -0.03873F),
                    localAngles = new Vector3(10F, 315.7086F, 0F),
                    localScale = new Vector3(4F, 4F, 4F)
                },
            });
            rules.Add("mdlImp", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Neck",
                    localPos = new Vector3(0.07122F, 0.07336F, -0.02511F),
                    localAngles = new Vector3(10F, 180F, 10F),
                    localScale = new Vector3(3F, 3F, 3F)
                },
            });
            rules.Add("mdlImpBoss", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Neck",
                    localPos = new Vector3(0F, -0.4062F, 0.09444F),
                    localAngles = new Vector3(0F, 180F, 0F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlJellyfish", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hull2",
                    localPos = new Vector3(0.30717F, 0.74014F, 0.16873F),
                    localAngles = new Vector3(10F, 0F, 340F),
                    localScale = new Vector3(12F, 12F, 12F)
                },
            });
            rules.Add("mdlLemurian", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00001F, 2.60764F, -1.29676F),
                    localAngles = new Vector3(275F, 0.00001F, 0.00001F),
                    localScale = new Vector3(6F, 8F, 6F)
                },
            });
            rules.Add("mdlLemurianBruiser", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 1.628F, 0.92033F),
                    localAngles = new Vector3(270F, 180F, 0F),
                    localScale = new Vector3(6F, 6F, 6F)
                },
            });
            rules.Add("mdlLunarExploder", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleCore",
                    localPos = new Vector3(0.00701F, 0.79941F, 0.00746F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(8F, 8F, 8F)
                },
            });
            rules.Add("mdlLunarGolem", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 1.26947F, 0.70541F),
                    localAngles = new Vector3(10F, 0F, 0F),
                    localScale = new Vector3(6F, 6F, 6F)
                },
            });
            rules.Add("mdlLunarWisp", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Mask",
                    localPos = new Vector3(0F, 0F, 2.55695F),
                    localAngles = new Vector3(80.00003F, 0F, 0F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlMiniMushroom", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.24393F, 0F, 0F),
                    localAngles = new Vector3(90F, 270F, 0F),
                    localScale = new Vector3(12F, 12F, 12F)
                },
            });
            rules.Add("mdlNullifier", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0F, 1.29621F, 0.23904F),
                    localAngles = new Vector3(10F, 0F, 0F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlParent", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-65.54737F, 116.3315F, -0.00005F),
                    localAngles = new Vector3(330F, 90F, 0F),
                    localScale = new Vector3(750F, 750F, 750F)
                },
            });
            rules.Add("mdlRoboBallBoss", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Center",
                    localPos = new Vector3(0F, 0.79644F, 0F),
                    localAngles = new Vector3(10F, 0F, 0F),
                    localScale = new Vector3(12F, 12F, 12F)
                },
            });
            rules.Add("mdlRoboBallMini", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0F, 0.70609F, -1.13701F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(12F, 12F, 12F)
                },
            });
            rules.Add("mdlScav", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0F, 6.14461F, 0.40801F),
                    localAngles = new Vector3(340F, 180F, 0F),
                    localScale = new Vector3(65F, 65F, 65F)
                },
            });
            rules.Add("mdlTitan", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 6.16367F, 0.86146F),
                    localAngles = new Vector3(20F, 0F, 0F),
                    localScale = new Vector3(25F, 30F, 25F)
                },
            });
            rules.Add("mdlVagrant", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hull",
                    localPos = new Vector3(0F, 1.28064F, 0F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlVulture", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00002F, 0.95108F, -1.15824F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(20F, 20F, 20F)
                },
            });
            rules.Add("mdlWisp1Mouth", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0F, 0.56107F),
                    localAngles = new Vector3(290F, 180F, 0.00001F),
                    localScale = new Vector3(6F, 6F, 6F)
                },
            });



            ItemAPI.Add(new CustomItem(Faust, rules));
        }
    }
}
