using System.Linq;
using ScarletMaidenAP.Utils;
using UnityEngine;

namespace ScarletMaidenAP.Managers
{
    public class QuestModManager
    {
        public QuestModManager()
        {
            On.QuestManager.GetActiveQuest += QuestManager_GetActiveQuest;
            On.QuestManager.GetActiveQuestState += QuestManager_GetActiveQuestState;
            On.QuestManager.GetIsQuestItemAvailable += QuestManager_GetIsQuestItemAvailable;
            On.PauseMenuTabJournal.SetUpQuests += PauseMenuTabJournal_SetUpQuests;
        }

        private QuestState QuestManager_GetActiveQuestState(On.QuestManager.orig_GetActiveQuestState orig, QuestManager self)
        {
            return ActiveQuest == null
                ? null
                : new QuestState
                {
                    countItemsCollected = 0, // TODO: Get from multiworld
                    isCompleted = false,
                    questID = ActiveQuest.id,
                };
        }

        private void PauseMenuTabJournal_SetUpQuests(On.PauseMenuTabJournal.orig_SetUpQuests orig, PauseMenuTabJournal self)
        {
            APQuestGenerator.UpdateQuestListings();
            self.activeQuestItem.gameObject.SetActive(true);
            self.activeQuestItem.SetUpAsQuestItem(new QuestState
            {
                questID = "FinalBossGoal",
                countItemsCollected = 0,
                isCompleted = false,
            });
            self.activeQuestItem.text.text = "Archipelago";
            self.listItems.Add(self.activeQuestItem);
            // TODO: Add all vanilla quests to the archive

            //foreach (QuestState completedQuestState in GameManager.instance.GetSaveSlot().gameState.journalState)
            //{
            //    Quest quest = completedQuestState.GetQuest();
            //    JournalListItem journalListItem = Object.Instantiate<JournalListItem>(self.storyItemPrefab, (Vector3)Vector2.zero, Quaternion.identity, (Transform)self.storiesContainer);
            //    journalListItem.text.text = quest.title.GetLocalizedString();
            //    journalListItem.SetUpAsQuestItem(completedQuestState);
            //    self.listItems.Add(journalListItem);
            //}
        }

        private bool QuestManager_GetIsQuestItemAvailable(On.QuestManager.orig_GetIsQuestItemAvailable orig, QuestManager self)
        {
            ActiveQuest = GetAvailableQuest();
            Plugin.BepinLogger.LogWarning($"ActiveQuest set: {ActiveQuest?.title.GetLocalizedString() ?? "<Null>"}");
            return ActiveQuest != null;
        }

        public Quest ActiveQuest { get; set; }

        public Quest GetAvailableQuest()
        {
            var availableQuests = QuestManager.instance.quests.Where(IsQuestAvailable).ToArray();
            Plugin.BepinLogger.LogWarning($"Possible active quests: {availableQuests.Length}");
            return availableQuests.Any() ? availableQuests[Random.Range(0, availableQuests.Length)] : null;
        }

        public bool IsQuestAvailable(Quest quest)
        {
            // TODO: Check if all items collected from multiworld
            return StatsManager.instance.GetRunState().currentDungeonLevel >= quest.minDungeonLevelForItemDrop;
        }

        private Quest QuestManager_GetActiveQuest(On.QuestManager.orig_GetActiveQuest orig, QuestManager self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2);
            Plugin.BepinLogger.LogMessage($"Called by {caller}");
            if (ActiveQuest != null)
            {
                var active = ActiveQuest;
                Plugin.BepinLogger.LogWarning($"Using available activequest: {active.title.GetLocalizedString()}");
                return active;
            }
            Plugin.BepinLogger.LogWarning($"No available activequest");
            return orig(self);
        }
    }
}
