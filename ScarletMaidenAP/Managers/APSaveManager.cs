namespace ScarletMaidenAP.Managers
{
    public class APSaveManager
    {
        public GameState AP_GameState;

        public APSaveManager()
        {
            //AP_GameState = new GameState();
            //AP_GameState.Init();

            //On.SaveSlot.Load += AP_Load;
            //On.SaveSlot.GetGameState += AP_GetGameState;
        }

        private void AP_Load(On.SaveSlot.orig_Load orig, SaveSlot self)
        {
            self.Load();
            AP_GameState = new GameState();
            AP_GameState.Init();
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
