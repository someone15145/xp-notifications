using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace XPNotifications
{
    public enum RunXPMode { Disabled, Normal, Throttling }

    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string MODNAME = "XP Notifications";
        public const string AUTHOR = "someone15145";
        public const string GUID = "com.someone15145.xpnotifications";
        public const string VERSION = "1.0.2";

        internal static ManualLogSource Log;

        public static ConfigEntry<bool> ShowXPNotifications;
        public static ConfigEntry<MessageHud.MessageType> NotificationPosition;
        public static ConfigEntry<string> NotificationFormat;
        public static ConfigEntry<int> NotificationTextSizeXP;
        public static ConfigEntry<RunXPMode> RunXPBehavior;
        public static ConfigEntry<int> RunXPTimeout;

        private readonly Harmony _harmony = new Harmony(GUID);

        private void Awake()
        {
            Log = Logger;

            ShowXPNotifications = Config.Bind(
                "General", "ShowXPNotifications",
                true,
                new ConfigDescription(
                    "Enable/disable all XP notifications.",
                    null,
                    new ConfigurationManagerAttributes { Order = 5 }));
            NotificationFormat = Config.Bind(
                "General", "NotificationFormat",
                "{0}% ({2}/{3}) [+{1}] Lv.{4}",
                new ConfigDescription(
                    "Format string placeholders:\n{0}: % | {1}: Gained | {2}: Current | {3}: Next Level | {4}: Level",
                    null,
                    new ConfigurationManagerAttributes { Order = 4 }));
            NotificationPosition = Config.Bind(
                "General", "NotificationPosition",
                MessageHud.MessageType.TopLeft,
                new ConfigDescription(
                    "Screen position: TopLeft or Center.",
                    null,
                    new ConfigurationManagerAttributes { Order = 3 }));
            NotificationTextSizeXP = Config.Bind(
                "General", "NotificationTextSizeXP",
                14,
                new ConfigDescription(
                    "Notification font size.",
                    new AcceptableValueRange<int>(8, 40),
                    new ConfigurationManagerAttributes { Order = 2 }));
            RunXPBehavior = Config.Bind(
                "General", "RunXPBehavior",
                RunXPMode.Throttling,
                new ConfigDescription(
                    "Running skill behavior:\nDisabled: Off\nNormal: Every gain\nThrottling: Once per X seconds",
                    null,
                    new ConfigurationManagerAttributes { Order = 1 }));
            RunXPTimeout = Config.Bind(
                "General", "RunXPTimeout",
                2,
                new ConfigDescription(
                    "Cooldown (seconds) for Throttling mode.",
                    new AcceptableValueRange<int>(2, 20),
                    new ConfigurationManagerAttributes { Order = 0 }));

            _harmony.PatchAll(typeof(Patches));

            Log.LogInfo($"[{MODNAME}] v{VERSION} loaded successfully");
        }
    }

    internal static class Patches
    {
        [HarmonyPatch(typeof(Skills), nameof(Skills.RaiseSkill))]
        [HarmonyPostfix]
        private static void RaiseSkill_Postfix(Skills __instance, Skills.SkillType skillType, float factor = 1f)
        {
            if (skillType == Skills.SkillType.None || !Main.ShowXPNotifications.Value)
                return;

            var getSkillMethod = AccessTools.Method(typeof(Skills), "GetSkill", new Type[] { typeof(Skills.SkillType) });
            Skills.Skill skill = getSkillMethod.Invoke(__instance, new object[] { skillType }) as Skills.Skill;
            if (skill == null) return;

            XPNotification.Show(skill, factor);
        }
    }

    internal static class XPNotification
    {
        private static float _lastRunXPTime = 0f;

        public static void Show(Skills.Skill skill, float factor)
        {
            if (skill.m_level >= 100f)
                return;

            if (skill.m_info.m_skill == Skills.SkillType.Run)
                switch (Main.RunXPBehavior.Value)
                {
                    case RunXPMode.Disabled:
                        return;
                    case RunXPMode.Throttling:
                        if (Time.time - _lastRunXPTime < Main.RunXPTimeout.Value)
                            return;
                        _lastRunXPTime = Time.time;
                        break;
                }

            string skillName = Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower());

            float currentPercent = (float)Math.Round(skill.GetLevelPercentage() * 100f, 1);
            float nextLevelRequirement = Mathf.Pow(skill.m_level + 1f, 1.5f) * 0.5f + 0.5f;
            float currentXP = (float)Math.Round(skill.m_accumulator, 2);
            float neededXP = (float)Math.Round(nextLevelRequirement, 2);
            float gainedXP = (float)Math.Round(skill.m_info.m_increseStep * factor, 2);
            int currentLevel = Mathf.FloorToInt(skill.m_level);

            string formattedText;
            try
            {
                formattedText = string.Format(
                    Main.NotificationFormat.Value,
                    currentPercent, gainedXP, currentXP, neededXP, currentLevel);
            }
            catch
            {
                formattedText = $"{currentPercent}%";
            }

            string finalText = $"{skillName}: {formattedText}";

            MessageHud.instance.ShowMessage(
                Main.NotificationPosition.Value,
                $"<size={Main.NotificationTextSizeXP.Value}>{finalText}</size>",
                0, null, false);
        }
    }
}