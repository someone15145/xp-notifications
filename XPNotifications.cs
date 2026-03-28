using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace XPNotifications
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string MODNAME = "XPNotifications";
        public const string AUTHOR = "someone15145";
        public const string GUID = "xpnotifications";
        public const string VERSION = "1.0.1";

        internal static ManualLogSource Log;

        public static ConfigEntry<bool> ShowXPNotifications;
        public static ConfigEntry<bool> ShowRunXP;
        public static ConfigEntry<string> NotificationFormat;
        public static ConfigEntry<int> NotificationTextSizeXP;

        private readonly Harmony _harmony = new Harmony(GUID);

        private void Awake()
        {
            Log = Logger;

            ShowXPNotifications = Config.Bind(
                "General",
                "ShowXPNotifications",
                true,
                new ConfigDescription(
                    "Enable or disable XP notifications.",
                    null,
                    new ConfigurationManagerAttributes { Order = 0 }
                ));

            ShowRunXP = Config.Bind(
                "General",
                "ShowRunXP",
                false,
                new ConfigDescription(
                    "Show XP notifications for running.",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }
                ));

            NotificationFormat = Config.Bind(
                "General",
                "NotificationFormat",
                "{0}%",
                new ConfigDescription(
                    "Notification format.\n" +
                    "{0} = current percentage\n" +
                    "{1} = XP gained from the action\n" +
                    "{2} = current XP\n" +
                    "{3} = XP required for the next level",
                    null,
                    new ConfigurationManagerAttributes { Order = 2 }
                ));

            NotificationTextSizeXP = Config.Bind(
                "General",
                "NotificationTextSizeXP",
                14,
                new ConfigDescription(
                    "Notification text size.",
                    new AcceptableValueRange<int>(8, 40),
                    new ConfigurationManagerAttributes { Order = 3 }
                ));

            _harmony.PatchAll(typeof(Patches));

            Logger.LogInfo($"[{MODNAME}] v{VERSION} loaded");
        }
    }

    // Нужен для работы с Official BepInEx ConfigurationManager
    public class ConfigurationManagerAttributes
    {
        public bool? ShowRangeAsPercent;
        public Action<ConfigEntryBase> CustomDrawer;
        public int? Order;
        public bool? Browsable;
        public string Category;
        public object DefaultValue;
        public bool? ReadOnly;
    }

    internal static class Patches
    {
        [HarmonyPatch(typeof(Skills), nameof(Skills.RaiseSkill))]
        [HarmonyPostfix]
        private static void RaiseSkill_Postfix(Skills __instance, Skills.SkillType skillType, float factor = 1f)
        {
            if (skillType == Skills.SkillType.None)
                return;

            if (!Main.ShowXPNotifications.Value)
                return;

            var getSkillMethod = AccessTools.Method(
                typeof(Skills),
                "GetSkill",
                new Type[] { typeof(Skills.SkillType) });

            Skills.Skill skill = getSkillMethod.Invoke(__instance, new object[] { skillType }) as Skills.Skill;

            if (skill != null)
                XPNotification.Show(skill, factor);
        }
    }

    internal static class XPNotification
    {
        public static void Show(Skills.Skill skill, float factor)
        {
            if (skill.m_level >= 100f)
                return;

            if (!Main.ShowRunXP.Value && skill.m_info.m_skill == Skills.SkillType.Run)
                return;

            string skillName = $"$skill_{skill.m_info.m_skill.ToString().ToLower()}";

            float currentPercent = (float)Math.Round(skill.GetLevelPercentage() * 100f, 1);

            float nextLevelRequirement = Mathf.Pow(Mathf.Floor(skill.m_level + 1f), 1.5f) * 0.5f + 0.5f;

            float currentXP = (float)Math.Round(skill.m_accumulator, 2);
            float neededXP = (float)Math.Round(nextLevelRequirement, 2);
            float gainedXP = (float)Math.Round(skill.m_info.m_increseStep * factor, 2);

            string formattedText;

            try
            {
                formattedText = string.Format(
                    Main.NotificationFormat.Value,
                    currentPercent,
                    gainedXP,
                    currentXP,
                    neededXP);
            }
            catch
            {
                formattedText = string.Empty;
            }

            string localized = Localization.instance.Localize(
                $"{skillName}: {formattedText}");

            MessageHud.instance.ShowMessage(
                MessageHud.MessageType.TopLeft,
                $"<size={Main.NotificationTextSizeXP.Value}>{localized}</size>",
                0,
                null,
                false);
        }
    }
}