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
        public const string MODNAME = "XP Notifications";
        public const string AUTHOR = "someone15145";
        public const string GUID = "com.someone15145.xpnotifications";
        public const string VERSION = "1.0.1";

        internal static ManualLogSource Log;

        public static ConfigEntry<bool> ShowXPNotifications;
        public static ConfigEntry<bool> ShowRunXP;
        public static ConfigEntry<string> NotificationFormat;
        public static ConfigEntry<int> NotificationTextSizeXP;
        public static ConfigEntry<MessageHud.MessageType> NotificationPosition;

        private readonly Harmony _harmony = new Harmony(GUID);

        private void Awake()
        {
            Log = Logger;

            ShowXPNotifications = Config.Bind(
                "General", "ShowXPNotifications",
                true,
                new ConfigDescription("Enable or disable all XP notifications.", null));

            ShowRunXP = Config.Bind(
                "General", "ShowRunXP",
                false,
                new ConfigDescription("Show notifications for the Running skill.", null));

            NotificationFormat = Config.Bind(
                "General", "NotificationFormat",
                "{0}% ({2}/{3}) [+{1}]",
                new ConfigDescription(
                    "Notification format string.\n" +
                    "{0} = current percentage\n" +
                    "{1} = XP gained this action\n" +
                    "{2} = current accumulator XP\n" +
                    "{3} = XP needed for next level",
                    null));

            NotificationTextSizeXP = Config.Bind(
                "General", "NotificationTextSizeXP",
                14,
                new ConfigDescription("Text size of the notification.", new AcceptableValueRange<int>(8, 40)));

            NotificationPosition = Config.Bind(
                "General", "NotificationPosition",
                MessageHud.MessageType.TopLeft,
                new ConfigDescription("Where to show the notification (TopLeft or Center).", null));

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
        public static void Show(Skills.Skill skill, float factor)
        {
            if (skill.m_level >= 100f)
                return;

            if (!Main.ShowRunXP.Value && skill.m_info.m_skill == Skills.SkillType.Run)
                return;

            string skillName = Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower());

            float currentPercent = (float)Math.Round(skill.GetLevelPercentage() * 100f, 1);

            // Формула из вики
            float nextLevelRequirement = Mathf.Pow(skill.m_level + 1f, 1.5f) * 0.5f + 0.5f;

            float currentXP = (float)Math.Round(skill.m_accumulator, 2);
            float neededXP = (float)Math.Round(nextLevelRequirement, 2);
            float gainedXP = (float)Math.Round(skill.m_info.m_increseStep * factor, 2);

            string formattedText;
            try
            {
                formattedText = string.Format(Main.NotificationFormat.Value, currentPercent, gainedXP, currentXP, neededXP);
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