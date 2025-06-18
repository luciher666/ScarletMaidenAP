using System.Linq;
using UnityEngine.Localization;

namespace ScarletMaidenAP.Utils
{
    public static class APQuestGenerator
    {
        public static void UpdateQuestListings()
        {
            var quests = QuestManager.instance.quests.ToList();
            if (quests.Any(q => q.id.Equals("FinalBossGoal"))) // only add new quests if they don't exist
                return;

            foreach (var quest in quests)
            {
                Plugin.BepinLogger.LogWarning($"{quest.title.GetLocalizedString()}: {quest.itemSprite.texture.name}");
            }

            var finalBossGoal = new Quest
            {
                countItems = 1,
                title = new LocalizedString("StringTable", "Archipelago"),
                description = new LocalizedString("StringTable", "FinalBossGoal"),
                galleryAsset = null,
                id = "FinalBossGoal",
                //itemSprite = , // TODO: Add Archipelago sprite
                lewdDungeonSceneName = string.Empty,
                minDungeonLevelForItemDrop = int.MaxValue,
                mistyDialogID = string.Empty,
            };
            quests.Add(finalBossGoal);
            var allBossGoal = new Quest
            {
                countItems = 5,
                title = new LocalizedString("StringTable", "Archipelago"),
                description = new LocalizedString("StringTable", "AllBossGoal"),
                galleryAsset = null,
                id = "AllBossGoal",
                //itemSprite = , // TODO: Add Archipelago sprite
                lewdDungeonSceneName = string.Empty,
                minDungeonLevelForItemDrop = int.MaxValue,
                mistyDialogID = string.Empty,
            };
            quests.Add(allBossGoal);
            var allQuestGoal = new Quest
            {
                countItems = 1,
                title = new LocalizedString("StringTable", "Archipelago"),
                description = new LocalizedString("StringTable", "AllQuestGoal"),
                galleryAsset = null,
                id = "AllQuestGoal",
                //itemSprite = , // TODO: Add Archipelago sprite
                lewdDungeonSceneName = string.Empty,
                minDungeonLevelForItemDrop = int.MaxValue,
                mistyDialogID = string.Empty,
            };
            quests.Add(allQuestGoal);
            QuestManager.instance.quests = quests.ToArray();
        }
    }
}
