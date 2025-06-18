using ScarletMaidenAP.Utils;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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
            On.Misty.OnInteract += Misty_OnInteract;
            On.Misty.OnSelection1Selected += Misty_OnSelection1Selected;
            On.LewdDungeonMenu.UpdateOptions += LewdDungeonMenu_UpdateOptions;
            On.LewdDungeonMenu.OnOptionConfirmed += LewdDungeonMenu_OnOptionConfirmed;
        }

        private void Misty_OnSelection1Selected(On.Misty.orig_OnSelection1Selected orig, Misty self)
        {
            self.OpenLewdDungeonMenu();
        }

        /// <summary>
        /// Display all quests in Misty's submenu.
        /// </summary>
        private void LewdDungeonMenu_UpdateOptions(On.LewdDungeonMenu.orig_UpdateOptions orig, LewdDungeonMenu self)
        {
            foreach (Component component in self.optionsLayoutGroup.transform)
                Object.Destroy(component.gameObject);
            self.quests.Clear();
            self.options.Clear();
            foreach (var quest in QuestManager.instance.quests.Where(q =>
                         !q.title.GetLocalizedString().Equals("Archipelago")))
            {
                self.quests.Add(quest);
                var gameObject = Object.Instantiate(self.listItemPrefab, Vector3.zero, Quaternion.identity, self.optionsLayoutGroup.transform);
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = quest.title.GetLocalizedString();
                self.options.Add(gameObject.GetComponent<Image>());
            }
            self.options.Add(self.leaveOption.GetComponent<Image>());
            self.SetOptionSelected(0);
        }

        private void LewdDungeonMenu_OnOptionConfirmed(On.LewdDungeonMenu.orig_OnOptionConfirmed orig, LewdDungeonMenu self)
        {
            if (self.pointerIndex == self.options.Count - 1)
            {
                self.misty.ExitLewdDungeonMenu();
            }
            else
            {
                AudioManager.instance.PlaySFX("UI/ui_confirm");
                // TODO: Conditional on if you've obtained enough quest items
                self.gameObject.SetActive(false);
                self.misty.activeDialog = self.misty.GetDialogWithID("quest_in_progress");
                self.misty.hud.npcDialog.ShowDialog(self.misty, self.misty.activeDialog, self.misty.OnDialogCallback);
                //self.misty.EnterLewdDungeon(self.quests[self.pointerIndex].lewdDungeonSceneName);
            }
        }

        private void Misty_OnInteract(On.Misty.orig_OnInteract orig, Misty self)
        {
            if (self.isScarletInDialogPosition)
            {
                if (self.moveScarletToDialogPositionCoroutine != null || self.hud.npcDialog.gameObject.activeSelf || self.lewdDungeonMenu.gameObject.activeSelf)
                    return;
                self.activeDialog = self.GetDialogWithID("quest_in_progress_dungeon_menu_available");
                self.hud.npcDialog.ShowDialog(self, self.activeDialog, self.OnDialogCallback);
            }
            else
                self.MoveScarletToDialogPosition((Action)self.OnInteract);
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
            // TODO: Add all vanilla quests to the archive, tracking progress from multiworld

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
