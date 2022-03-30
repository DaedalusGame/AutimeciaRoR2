using BepInEx;
using Autimecia.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Globalization;
using Autimecia.Utils;
using R2API;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace Autimecia
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "ItemAPI",
        "EffectAPI",
        "ProjectileAPI",
        "LanguageAPI",
        "SoundAPI",
        "RecalculateStatsAPI",
    })]

    public class AutimeciaPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.Bord.Autimecia";
        public const string MODNAME = "Autimecia";
        public const string MODVERSION = "1.0.0";

        internal static BepInEx.Logging.ManualLogSource ModLogger;

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string developerPrefix = "BORD";

        internal List<SurvivorBase> Survivors = new List<SurvivorBase>();

        public static AutimeciaPlugin instance;

        private void Awake()
        {
            instance = this;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            ModLogger = this.Logger;

            // load assets and read config
            Modules.Assets.Initialize();
            Modules.Config.ReadConfig();
            Modules.Items.Initialize();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new Modules.Survivors.Autimecia().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;

            Hook();
        }

        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            // have to set item displays later now because they require direct object references..
            Modules.Survivors.Autimecia.instance.SetItemDisplays();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line

            ProjectileSalvoHelper.Init();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender)
            {
                if(sender.HasBuff(Modules.Buffs.earthBuff))
                {
                    args.armorAdd += 300f;
                }

                if (sender.HasBuff(Modules.Buffs.waterBuff))
                {
                    args.baseRegenAdd += 15f;
                }

                if (sender.HasBuff(Modules.Buffs.windBuff))
                {
                    args.moveSpeedMultAdd += 1.0f;
                }
            }
        }

        /*private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.armorBuff))
                {
                    self.armor += 300f;
                }
            }
        }*/
    }
}