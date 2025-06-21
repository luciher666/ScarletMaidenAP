using System.Collections.Generic;

namespace ScarletMaidenAP.Managers
{
    public class APSaveManager
    {
        public GameState AP_GameState;

        public APSaveManager()
        {
            On.SaveSlot.Load += AP_Load;
            On.SaveSlot.Save += AP_Save;
            On.SaveSlot.GetGameState += AP_GetGameState;
            On.MainMenu.LoadSaveFileMenu += MainMenu_LoadSaveFileMenu;
        }

        private void MainMenu_LoadSaveFileMenu(On.MainMenu.orig_LoadSaveFileMenu orig, MainMenu self)
        {
            GameManager.instance.LoadSaveFile(new SaveSlot("tmp"));
        }

        private void AP_Save(On.SaveSlot.orig_Save orig, SaveSlot self)
        {
            Plugin.BepinLogger.LogMessage("Save intercepted");
        }

        private void AP_Load(On.SaveSlot.orig_Load orig, SaveSlot self)
        {
            Plugin.BepinLogger.LogMessage("Loading AP Save");
            AP_GameState = new GameState();
            AP_GameState.Init();
            AP_GameState.juliaState = new NPCStateJulia { state = NPCStateJulia.State.Repent };
            AP_GameState.journalState = new JournalState
            {
                activeQuestState = new QuestState
                    { countItemsCollected = 0, isCompleted = false, questID = "Archipelago" },
                completedQuestStates = new HashSet<QuestState>(),
                unlockedChapters = new HashSet<string>(),
            };
            AP_GameState.isTutorialCompleted = true;
            AP_GameState.difficultySetting = DifficultySetting.Normal; // TODO: Load from slotdata
            AP_GameState.isMainHubVisited = true;                      // TODO: Load from slotdata
            self.gameState = AP_GameState;
        }

        /// <summary>
        /// Get overriden game state using AP stuff
        /// </summary>
        private GameState AP_GetGameState(On.SaveSlot.orig_GetGameState orig, SaveSlot self)
        {
            return AP_GameState;
        }
    }
}
