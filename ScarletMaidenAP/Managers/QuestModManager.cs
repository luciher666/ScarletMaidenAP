using ScarletMaidenAP.Utils;

namespace ScarletMaidenAP.Managers
{
    public class QuestModManager
    {
        public QuestModManager()
        {
            On.QuestManager.GetActiveQuest += QuestManager_GetActiveQuest;
            On.QuestManager.GetIsQuestItemAvailable += QuestManager_GetIsQuestItemAvailable;
            On.PauseMenuTabJournal.SetUpQuests += PauseMenuTabJournal_SetUpQuests;
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
            return true;
        }

        private Quest QuestManager_GetActiveQuest(On.QuestManager.orig_GetActiveQuest orig, QuestManager self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2);
            Plugin.BepinLogger.LogMessage($"Called by {caller}");
            foreach (var q in self.quests)
            {
                Plugin.BepinLogger.LogWarning($"{q.title.TableReference.TableCollectionName} - ID:{q.id} - MinLevel:{q.minDungeonLevelForItemDrop} - Sprite:{q.itemSprite.name} - Count:{q.countItems}");
            }
            return orig(self);
        }
    }
}
