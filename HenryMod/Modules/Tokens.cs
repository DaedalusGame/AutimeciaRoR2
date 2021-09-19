using R2API;
using System;
using System.Text;
using UnityEngine;

namespace Autimecia.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Autimecia
            string prefix = AutimeciaPlugin.developerPrefix + "_AUTIMECIA_BODY_";

            string desc = $"Autimecia is a blah blah blah blah blah blah blah blah blah<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > This is where skill descriptions go." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > This is where more skill descriptions go." + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > So anyway do you ever wonder why nighttime makes Silicate Sands glass over? Is that fucked up or what?" + Environment.NewLine + Environment.NewLine;
            desc = desc + $"< ! > {Rainbowize($"Give the guy in charge a message from me:{Environment.NewLine}\"You are already dead.\"")}" + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, to punch out another cthulhu.";
            string outroFailure = "..and so she vanished, not giving you the satisfaction of a win.";

            LanguageAPI.Add(prefix + "NAME", Rainbowize("Autimecia", true));
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Euphoria");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_FEDORA_BEAM_NAME", "Fedora Beam");
            LanguageAPI.Add(prefix + "PRIMARY_FEDORA_BEAM_DESCRIPTION", Helpers.agilePrefix + $"Throw a fedora forward that fires a beam for <style=cIsDamage>600% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_FAUST_NAME", "Faustian Bargain");
            LanguageAPI.Add(prefix + "SECONDARY_FAUST_DESCRIPTION", Helpers.agilePrefix + $"Launch a fedora at one enemy for <style=cIsDamage>{100f * StaticValues.gunDamageCoefficient}% damage</style>. The fedora attaches to the target and you receive 1% of the enemy's money and experience each hit. Additionally, one random skill of the target is disabled while the fedora is attached.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_CHAOS_NAME", "Chaos");
            LanguageAPI.Add(prefix + "SPECIAL_CHAOS_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");
            #endregion
            #endregion
        }

        static int rainbowIndex;
        static string Rainbowize(string str, bool reset = false)
        {
            if (reset)
                rainbowIndex = 0;
            var builder = new StringBuilder();
            foreach(var chr in str)
            {
                var color = Color.HSVToRGB(((rainbowIndex * 10) % 255) / 255f, 150 / 255f, 202 / 255f);
                builder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{chr}</color>");
                rainbowIndex++;
            }
            return builder.ToString();
        }
    }
}