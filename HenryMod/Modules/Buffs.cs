using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Autimecia.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;
        internal static BuffDef earthBuff;
        internal static BuffDef windBuff;
        internal static BuffDef waterBuff;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            armorBuff = AddNewBuff("HenryArmorBuff", Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, false, false);
            earthBuff = AddNewBuff("AutimeciaEarthBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("buff_earth"), Color.white, false, false);
            windBuff = AddNewBuff("AutimeciaWindBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("buff_wind"), new Color32(220, 252, 221, 255), false, false);
            waterBuff = AddNewBuff("AutimeciaWaterBuff", Assets.mainAssetBundle.LoadAsset<Sprite>("buff_water"), new Color32(40, 164, 238, 255), false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}