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
            On.Misty.Start += Misty_Start;
            On.Misty.OnInteract += Misty_OnInteract;
            On.Misty.OnSelection1Selected += Misty_OnSelection1Selected;
            On.Misty.Reset += Misty_Reset;
            On.Misty.OnDialogExhausted += Misty_OnDialogExhausted;
            On.Misty.TestShouldShowAttentionIndicator += Misty_TestShouldShowAttentionIndicator;
            On.Misty.StartQuest += Misty_StartQuest;
            On.LewdDungeonMenu.UpdateOptions += LewdDungeonMenu_UpdateOptions;
            On.LewdDungeonMenu.OnOptionConfirmed += LewdDungeonMenu_OnOptionConfirmed;
        }

        /// <summary>
        /// Disable quest start script.
        /// </summary>
        private void Misty_StartQuest(On.Misty.orig_StartQuest orig, Misty self, string questID)
        {
            // Do nothing. Quests don't need to be started, all are always active
        }

        /// <summary>
        /// Show the dungeon menu again after a quest explanation ends
        /// </summary>
        private void Misty_OnDialogExhausted(On.Misty.orig_OnDialogExhausted orig, Misty self)
        {
            if (SavedIndex != null)
            {
                self.OpenLewdDungeonMenu();
            }
        }

        /// <summary>
        /// Abridge Misty's quest dialogs
        /// </summary>
        private void Misty_Start(On.Misty.orig_Start orig, Misty self)
        {
            orig(self);
            // Abridge quest dialogs to show relevant info and remove reference to prior quests or specific world state
            self.dialogs[0].lines = self.dialogs[0].lines.Skip(14).Take(7).ToArray();
            self.dialogs[7].lines = self.dialogs[7].lines.Skip(2).ToArray();
            self.dialogs[8].lines = self.dialogs[8].lines.Skip(3).ToArray();
            self.dialogs[9].lines = self.dialogs[9].lines.Skip(4).ToArray();
            self.dialogs[10].lines = self.dialogs[10].lines.Skip(4).ToArray();
        }

        /// <summary>
        /// TODO: Check if any quest is ready to be completed
        /// 
        /// Change when exclamation point appears above Misty's head
        /// </summary>
        private bool Misty_TestShouldShowAttentionIndicator(On.Misty.orig_TestShouldShowAttentionIndicator orig, Misty self)
        {
            return false;
        }

        /// <summary>
        /// Remove the saved index for quest selection on exiting dialog.
        /// </summary>
        private void Misty_Reset(On.Misty.orig_Reset orig, Misty self)
        {
            SavedIndex = null;
            orig(self);
        }

        /// <summary>
        /// Always open the menu, since all quests are always active.
        /// </summary>
        private void Misty_OnSelection1Selected(On.Misty.orig_OnSelection1Selected orig, Misty self)
        {
            self.OpenLewdDungeonMenu();
        }

        public int? SavedIndex;
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
            self.SetOptionSelected(SavedIndex ?? 0);
        }

        /// <summary>
        /// Activate Misty's not enough items dialog when not enough items are obtained.
        /// </summary>
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
                SavedIndex = self.pointerIndex;
                self.gameObject.SetActive(false);
                self.misty.activeDialog = self.misty.GetDialogWithID(self.quests[self.pointerIndex].mistyDialogID);
                self.misty.hud.npcDialog.ShowDialog(self.misty, self.misty.activeDialog, self.misty.OnDialogCallback);
                //self.misty.EnterLewdDungeon(self.quests[self.pointerIndex].lewdDungeonSceneName);
            }
        }

        /// <summary>
        /// TODO: When a quest is ready to complete, send to dungeon automatically
        /// 
        /// Only one path through Misty's dialog now, to allow all quests at once.
        /// </summary>
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

        /// <summary>
        /// Get specific active quest if it's been rolled
        /// </summary>
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
                questID = "FinalBossGoal", // TODO: Get goal string from multiworld
                countItemsCollected = 0, // TODO: Check goal state from multiworld
                isCompleted = false,
            });
            self.activeQuestItem.text.text = "Archipelago";
            self.listItems.Add(self.activeQuestItem);
            foreach (var quest in QuestManager.instance.quests.Where(q =>
                         !q.title.GetLocalizedString().Equals("Archipelago")))
            {
                var itemsCollected = 0; // TODO: Get from multiworld
                var questState = new QuestState
                {
                    countItemsCollected = 0,
                    questID = quest.id,
                    isCompleted = false, // Doesn't matter
                };
                var journalListItem = Object.Instantiate(self.storyItemPrefab, Vector2.zero, Quaternion.identity, self.storiesContainer);
                journalListItem.text.text = quest.title.GetLocalizedString();
                journalListItem.SetUpAsQuestItem(questState);
                self.listItems.Add(journalListItem);
            }
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
                Plugin.BepinLogger.LogWarning($"Using previously rolled active quest: {active.title.GetLocalizedString()}");
                return active;
            }
            Plugin.BepinLogger.LogError("No available active quest");
            return orig(self);
        }
    }
}
