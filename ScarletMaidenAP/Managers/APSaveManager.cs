namespace ScarletMaidenAP.Managers
{
    public class APSaveManager
    {
        public GameState AP_GameState;

        public APSaveManager()
        {
            //AP_GameState = new GameState();
            //AP_GameState.Init();
            //AP_GameState.juliaState.state = NPCStateJulia.State.Repent;
            //AP_GameState.tigerState.state = NPCStateTiger.State.NonActive;
            //AP_GameState.isTutorialCompleted = true;
            //On.SaveSlot.Load += AP_Load;
            On.SaveSlot.Save += AP_Save;
            //On.SaveSlot.GetGameState += AP_GetGameState;
        }

        private void AP_Save(On.SaveSlot.orig_Save orig, SaveSlot self)
        {
            Plugin.BepinLogger.LogMessage("Save intercepted");
        }

        private void AP_Load(On.SaveSlot.orig_Load orig, SaveSlot self)
        {
            Plugin.BepinLogger.LogMessage("Loading AP Save");
            self.gameState = AP_GameState;
            Plugin.BepinLogger.LogMessage("AP Save Loaded");
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
