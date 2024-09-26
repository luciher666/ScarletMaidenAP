using Rewired;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using static UnityEngine.Object;
using UnityEngine.SceneManagement;

namespace ScarletMaidenAP.Managers
{
    public class NPCModManager
    {
        public NPCModManager()
        {
            // Generic
            On.NPCRescueInfo.GetIsRescued += NPCRescueInfo_GetIsRescued;
            // Blacksmith
            On.Blacksmith.Start += Blacksmith_Start;
            On.Blacksmith.GetLevel += Blacksmith_GetLevel;
            On.Blacksmith.GetMaxLevel += Blacksmith_GetMaxLevel;
            On.Blacksmith.GetMaxXPInCurrentLevel += Blacksmith_GetMaxXPInCurrentLevel;
            On.Blacksmith.GetXP += Blacksmith_GetXP;
            On.Blacksmith.PurchaseXP += Blacksmith_PurchaseXP;
            On.LootManager.DropLootForSmith += LootManager_DropLootForSmith;
            // Roman
            On.Roman.Start += Roman_Start;
            On.Roman.GetLevel += Roman_GetLevel;
            On.Roman.GetMaxLevel += Roman_GetMaxLevel;
            On.Roman.GetMaxXPInCurrentLevel += Roman_GetMaxXPInCurrentLevel;
            On.Roman.GetXP += Roman_GetXP;
            On.Roman.PurchaseXP += Roman_PurchaseXP;
            On.LootManager.DropLootForRoman += LootManager_DropLootForRoman;
            // Faelina
            On.Faelina.Start += Faelina_Start;
            On.Faelina.GetLevel += Faelina_GetLevel;
            On.Faelina.GetMaxLevel += Faelina_GetMaxLevel;
            On.Faelina.GetMaxXPInCurrentLevel += Faelina_GetMaxXPInCurrentLevel;
            On.Faelina.GetXP += Faelina_GetXP;
            On.Faelina.PurchaseXP += Faelina_PurchaseXP;
            On.LootManager.DropLootForFaelina += LootManager_DropLootForFaelina;
            // Candy
            On.Candy.Start += Candy_Start;
            On.Candy.GetNPCState += Candy_GetNPCState;
            On.Candy.Warp += Candy_Warp;
            On.WarpMenu.UpdateOptions += WarpMenu_UpdateOptions;
            On.GameManager.StartRun += GameManager_StartRun;
        }

        #region Generic

        /// <summary>
        /// Reimplementation of BaseNPC.Start(), which is called at the beginning of every NPC Start() method.
        /// Functionally identical to original method, used for simplicity due to no direct way to call base method from hooked override
        /// </summary>
        public static void BaseNPC_Start(BaseNPC self)
        {
            self.player = ReInput.players.GetPlayer(0);
            self.animator = self.GetComponent<Animator>();
            self.spriteRenderer = self.GetComponent<SpriteRenderer>();
            self.scarlet = GameObject.FindGameObjectWithTag("Scarlet").GetComponent<Scarlet>();
            self.hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
            self.moveDirection = self.spriteRenderer.flipX ? CharacterMoveDirection.Left : CharacterMoveDirection.Right;
            self.isInMainHub = GameObject.FindGameObjectWithTag("MainHubManager") != null;
            if (self.minimapIcon != null)
                self.minimapIcon.color = self.featureColor;
        }

        private bool NPCRescueInfo_GetIsRescued(On.NPCRescueInfo.orig_GetIsRescued orig, NPCRescueInfo self)
        {
            switch (self.npc)
            {
                case NPC.Blacksmith:
                    return BlacksmithSent;
                case NPC.Faelina:
                    return FaelinaSent;
                case NPC.Roman:
                    return RomanSent;
                case NPC.Candy:
                    return CandySent;
            }
            return true;
        }

        #endregion

        #region Blacksmith

        public static int BlacksmithLevelReceived = 0;
        public static bool BlacksmithSent = false;
        public static int BlacksmithLevelSent = 1; // MUST START AT 1!
        public static int BlacksmithMaxLevel = 12;
        public static int BlacksmithXP = 0;
        public static bool BlacksmithSignHints = false;

        /// <summary>
        /// Reimplementation of Blacksmith.Start(), separating out logic for sent/received. Allows Blacksmith to be rescued even if already in main hub.
        /// </summary>
        private void Blacksmith_Start(On.Blacksmith.orig_Start orig, Blacksmith self)
        {
            BaseNPC_Start(self);
            self.blacksmithState = new NPCStateBlacksmith
            {
                level = BlacksmithLevelSent,
                xp = BlacksmithXP
            };
            if (self.isInMainHub)
            {
                Destroy(self.cage.gameObject);
                if (BlacksmithLevelReceived > 0)
                {
                    if (!BlacksmithSignHints)
                    {
                        Destroy(self.signPost);
                    }
                    else
                    {
                        self.signPost.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                        self.signPost.gameObject.transform.position -= new Vector3(8, 0, 0);
                    }
                    self.blacksmithState.state = NPCStateBlacksmith.State.Merchant;
                }
                else
                {
                    if(self.signPost != null)
                        self.signPost.transform.parent = self.transform.parent;
                    Destroy(self.gameObject);
                    self.blacksmithState.state = NPCStateBlacksmith.State.Unrescued;
                }
            }
            else
            {
                Destroy(self.signPost);
                if (BlacksmithSent)
                {
                    Destroy(self.cage.gameObject);
                    Destroy(self.gameObject);
                    self.blacksmithState.state = NPCStateBlacksmith.State.Merchant;
                }
                else
                {
                    self.blacksmithState.state = NPCStateBlacksmith.State.Unrescued;
                }
            }
        }

        /// <summary>
        /// Returns the received level for the purpose of loot dropping, returns the sent level otherwise.
        /// </summary>
        private int Blacksmith_GetLevel(On.Blacksmith.orig_GetLevel orig, Blacksmith self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;
            return caller.Contains("DropLoot") ? BlacksmithLevelReceived : BlacksmithLevelSent;
        }

        /// <summary>
        /// Returns the max level available for the Blacksmith. Game default is 12 but should theoretically support any positive int up to int.MaxValue / 500
        ///
        /// Prob a bad idea to allow numbers that high anyway so prob gonna set a more reasonable limit in-APWorld
        /// </summary>
        private int Blacksmith_GetMaxLevel(On.Blacksmith.orig_GetMaxLevel orig, Blacksmith self)
        {
            return BlacksmithMaxLevel;
        }

        /// <summary>
        /// Matches vanilla but ensures that the Sent level is used for calculation
        /// </summary>
        private int Blacksmith_GetMaxXPInCurrentLevel(On.Blacksmith.orig_GetMaxXPInCurrentLevel orig, Blacksmith self)
        {
            return NPCManager.instance.GetMerchantMaxXPInLevel(BlacksmithLevelSent);
        }

        /// <summary>
        /// Changes XP purchasing to properly use Sent Level and AP XP
        /// </summary>
        private bool Blacksmith_PurchaseXP(On.Blacksmith.orig_PurchaseXP orig, Blacksmith self, int xpAmount)
        {
            if (BlacksmithLevelSent >= self.GetMaxLevel())
            {
                return false;
            }
            int totalPriceForXp = self.GetTotalPriceForXP(xpAmount);
            if (self.scarlet.sin - totalPriceForXp < 0)
                return false;
            self.scarlet.ModifySin(-totalPriceForXp);
            int merchantMaxXpInLevel = NPCManager.instance.GetMerchantMaxXPInLevel(BlacksmithLevelSent);
            int num1 = BlacksmithXP + xpAmount;
            int num2 = num1 - merchantMaxXpInLevel;
            if (num2 >= 0)
            {
                ++BlacksmithLevelSent; // TODO: Add AP Send
                StatsManager.instance.OnNPCLevelUp(NPC.Blacksmith, BlacksmithLevelSent);
                BlacksmithXP = num2;
                self.blacksmithState.lootCache = null;
                AudioManager.instance.PlaySFX("UI/skill_level_up");
                self.isDropLootInProgress = true;
                self.Invoke("DropLootWithSounds", 1f);
            }
            else
            {
                BlacksmithXP = num1;
                AudioManager.instance.PlaySFX("NPC/xp_increased1");
            }
            return true;
        }

        private int Blacksmith_GetXP(On.Blacksmith.orig_GetXP orig, Blacksmith self)
        {
            return BlacksmithXP;
        }

        /// <summary>
        /// Ensures that the loot that drops matches up with received Blacksmith level.
        /// </summary>
        private GameObject[] LootManager_DropLootForSmith(On.LootManager.orig_DropLootForSmith orig, LootManager self, IMerchant merchant, int count)
        {
            var state = GameManager.instance.GetSaveSlot().GetGameState().blacksmithState;
            state.level = BlacksmithLevelReceived;
            state.lootCache = null; // Clear cached loot every time fuck it why not. Otherwise level doesn't properly line up or rerolls don't work.
            return orig(self, merchant, count);
        }

        #endregion

        #region Roman

        public static int RomanLevelReceived = 0;
        public static bool RomanSent = false;
        public static int RomanLevelSent = 1; // MUST START AT 1!
        public static int RomanMaxLevel = 12;
        public static int RomanXP = 0;

        /// <summary>
        /// Reimplementation of Roman.Start(), separating out logic for sent/received. Allows Roman to be rescued even if already in main hub.
        /// </summary>
        private void Roman_Start(On.Roman.orig_Start orig, Roman self)
        {
            BaseNPC_Start(self);
            self.npcState = new NPCStateRoman
            {
                level = RomanLevelSent,
                xp = RomanXP
            };
            if (self.isInMainHub)
            {
                Destroy(self.cage.gameObject);
                if (RomanLevelReceived > 0)
                {
                    Destroy(self.signPost);
                    self.npcState.state = NPCStateRoman.State.Merchant;
                }
                else
                {
                    if (self.signPost != null)
                        self.signPost.transform.parent = self.transform.parent;
                    Destroy(self.gameObject);
                    self.npcState.state = NPCStateRoman.State.Unrescued;
                }
            }
            else
            {
                Destroy(self.signPost);
                if (RomanSent)
                {
                    Destroy(self.cage.gameObject);
                    Destroy(self.gameObject);
                    self.npcState.state = NPCStateRoman.State.Merchant;
                }
                else
                {
                    self.npcState.state = NPCStateRoman.State.Unrescued;
                }
            }
        }

        /// <summary>
        /// Returns the received level for the purpose of loot dropping, returns the sent level otherwise.
        /// </summary>
        private int Roman_GetLevel(On.Roman.orig_GetLevel orig, Roman self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;
            return caller.Contains("DropLoot") ? RomanLevelReceived : RomanLevelSent;
        }

        /// <summary>
        /// Returns the max level available for the Roman. Game default is 12 but should theoretically support any positive int up to int.MaxValue / 500
        ///
        /// Prob a bad idea to allow numbers that high anyway so prob gonna set a more reasonable limit in-APWorld
        /// </summary>
        private int Roman_GetMaxLevel(On.Roman.orig_GetMaxLevel orig, Roman self)
        {
            return RomanMaxLevel;
        }

        /// <summary>
        /// Matches vanilla but ensures that the Sent level is used for calculation
        /// </summary>
        private int Roman_GetMaxXPInCurrentLevel(On.Roman.orig_GetMaxXPInCurrentLevel orig, Roman self)
        {
            return NPCManager.instance.GetMerchantMaxXPInLevel(RomanLevelSent);
        }

        /// <summary>
        /// Changes XP purchasing to properly use Sent Level and AP XP
        /// </summary>
        private bool Roman_PurchaseXP(On.Roman.orig_PurchaseXP orig, Roman self, int xpAmount)
        {
            if (RomanLevelSent >= self.GetMaxLevel())
            {
                return false;
            }
            int totalPriceForXp = self.GetTotalPriceForXP(xpAmount);
            if (self.scarlet.sin - totalPriceForXp < 0)
                return false;
            self.scarlet.ModifySin(-totalPriceForXp);
            int merchantMaxXpInLevel = NPCManager.instance.GetMerchantMaxXPInLevel(RomanLevelSent);
            int num1 = RomanXP + xpAmount;
            int num2 = num1 - merchantMaxXpInLevel;
            if (num2 >= 0)
            {
                ++RomanLevelSent; // TODO: Add AP Send
                StatsManager.instance.OnNPCLevelUp(NPC.Roman, RomanLevelSent);
                RomanXP = num2;
                self.npcState.lootCache = null;
                AudioManager.instance.PlaySFX("UI/skill_level_up");
                self.Invoke("DropLootWithSounds", 1f);
            }
            else
            {
                RomanXP = num1;
                AudioManager.instance.PlaySFX("NPC/xp_increased1");
            }
            return true;
        }

        private int Roman_GetXP(On.Roman.orig_GetXP orig, Roman self)
        {
            return RomanXP;
        }

        /// <summary>
        /// Ensures that the loot that drops matches up with received Roman level.
        /// </summary>
        private GameObject[] LootManager_DropLootForRoman(On.LootManager.orig_DropLootForRoman orig, LootManager self, IMerchant merchant, int count)
        {
            var state = GameManager.instance.GetSaveSlot().GetGameState().romanState;
            state.level = RomanLevelReceived;
            state.lootCache = null; // Clear cached loot every time fuck it why not. Otherwise level doesn't properly line up or rerolls don't work.
            return orig(self, merchant, count);
        }

        #endregion

        #region Faelina

        public static int FaelinaLevelReceived = 0;
        public static bool FaelinaSent = false;
        public static int FaelinaLevelSent = 1; // MUST START AT 1!
        public static int FaelinaMaxLevel = 12;
        public static int FaelinaXP = 0;

        /// <summary>
        /// Reimplementation of Faelina.Start(), separating out logic for sent/received. Allows Faelina to be rescued even if already in main hub.
        /// </summary>
        private void Faelina_Start(On.Faelina.orig_Start orig, Faelina self)
        {
            BaseNPC_Start(self);
            self.npcState = new NPCStateFaelina
            {
                level = FaelinaLevelSent,
                xp = FaelinaXP
            };
            if (self.isInMainHub)
            {
                Destroy(self.cage.gameObject);
                if (FaelinaLevelReceived > 0)
                {
                    Destroy(self.signPost);
                    self.npcState.state = NPCStateFaelina.State.Merchant;
                }
                else
                {
                    if (self.signPost != null)
                        self.signPost.transform.parent = self.transform.parent;
                    Destroy(self.gameObject);
                    self.npcState.state = NPCStateFaelina.State.Unrescued;
                }
            }
            else
            {
                Destroy(self.signPost);
                if (FaelinaSent)
                {
                    Destroy(self.cage.gameObject);
                    Destroy(self.gameObject);
                    self.npcState.state = NPCStateFaelina.State.Merchant;
                }
                else
                {
                    self.npcState.state = NPCStateFaelina.State.Unrescued;
                }
            }
        }

        /// <summary>
        /// Returns the received level for the purpose of loot dropping, returns the sent level otherwise.
        /// </summary>
        private int Faelina_GetLevel(On.Faelina.orig_GetLevel orig, Faelina self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;
            return caller.Contains("DropLoot") ? FaelinaLevelReceived : FaelinaLevelSent;
        }

        /// <summary>
        /// Returns the max level available for the Faelina. Game default is 12 but should theoretically support any positive int up to int.MaxValue / 500
        ///
        /// Prob a bad idea to allow numbers that high anyway so prob gonna set a more reasonable limit in-APWorld
        /// </summary>
        private int Faelina_GetMaxLevel(On.Faelina.orig_GetMaxLevel orig, Faelina self)
        {
            return FaelinaMaxLevel;
        }

        /// <summary>
        /// Matches vanilla but ensures that the Sent level is used for calculation
        /// </summary>
        private int Faelina_GetMaxXPInCurrentLevel(On.Faelina.orig_GetMaxXPInCurrentLevel orig, Faelina self)
        {
            return NPCManager.instance.GetMerchantMaxXPInLevel(FaelinaLevelSent);
        }

        /// <summary>
        /// Changes XP purchasing to properly use Sent Level and AP XP
        /// </summary>
        private bool Faelina_PurchaseXP(On.Faelina.orig_PurchaseXP orig, Faelina self, int xpAmount)
        {
            if (FaelinaLevelSent >= self.GetMaxLevel())
            {
                return false;
            }
            int totalPriceForXp = self.GetTotalPriceForXP(xpAmount);
            if (self.scarlet.sin - totalPriceForXp < 0)
                return false;
            self.scarlet.ModifySin(-totalPriceForXp);
            int merchantMaxXpInLevel = NPCManager.instance.GetMerchantMaxXPInLevel(FaelinaLevelSent);
            int num1 = FaelinaXP + xpAmount;
            int num2 = num1 - merchantMaxXpInLevel;
            if (num2 >= 0)
            {
                ++FaelinaLevelSent; // TODO: Add AP Send
                StatsManager.instance.OnNPCLevelUp(NPC.Faelina, FaelinaLevelSent);
                FaelinaXP = num2;
                self.npcState.lootCache = null;
                AudioManager.instance.PlaySFX("UI/skill_level_up");
                self.Invoke("DropLootWithSounds", 1f);
            }
            else
            {
                FaelinaXP = num1;
                AudioManager.instance.PlaySFX("NPC/xp_increased1");
            }
            return true;
        }

        private int Faelina_GetXP(On.Faelina.orig_GetXP orig, Faelina self)
        {
            return FaelinaXP;
        }

        /// <summary>
        /// Ensures that the loot that drops matches up with received Faelina level.
        /// </summary>
        private GameObject[] LootManager_DropLootForFaelina(On.LootManager.orig_DropLootForFaelina orig, LootManager self, IMerchant merchant, int count)
        {
            var state = GameManager.instance.GetSaveSlot().GetGameState().romanState;
            state.level = FaelinaLevelReceived;
            state.lootCache = null; // Clear cached loot every time fuck it why not. Otherwise level doesn't properly line up or rerolls don't work.
            return orig(self, merchant, count);
        }

        #endregion

        #region Candy

        public static int CandyLevel = 5;
        public static int CurrentWarp = 1;
        public static bool CandySent = false;

        /// <summary>
        /// This is used to check how many warp levels you have unlocked. Use CandyLevel for this.
        /// </summary>
        private NPCStateCandy Candy_GetNPCState(On.Candy.orig_GetNPCState orig, Candy self)
        {
            var state = self.npcState;
            state.maxWarpDungeonLevel = CandyLevel;
            return state;
        }

        /// <summary>
        /// Reimplementation of Candy.Start(), separating out logic for sent/received. Allows Candy to be rescued even if already in main hub.
        /// </summary>
        private void Candy_Start(On.Candy.orig_Start orig, Candy self)
        {
            BaseNPC_Start(self);
            self.npcState = new NPCStateCandy
            {
                warpDungeonLevel = CurrentWarp,
                maxWarpDungeonLevel = CandyLevel
            };
            if (self.isInMainHub)
            {
                Destroy(self.cage.gameObject);
                if (CandyLevel > 1)
                {
                    Destroy(self.signPost);
                    self.npcState.state = NPCStateCandy.State.InMainHub;
                    if(self.mainPortal != null)
                        self.mainPortal.ChangeBG(CurrentWarp, false);
                }
                else
                {
                    if (self.signPost != null)
                        self.signPost.transform.parent = self.transform.parent;
                    Destroy(self.gameObject);
                    CurrentWarp = 1;
                    self.npcState.state = NPCStateCandy.State.Unrescued;
                }
            }
            else
            {
                Destroy(self.signPost);
                if (CandySent)
                {
                    Destroy(self.cage.gameObject);
                    Destroy(self.gameObject);
                    self.npcState.state = NPCStateCandy.State.InMainHub;
                }
                else
                {
                    self.npcState.state = NPCStateCandy.State.Unrescued;
                }
            }
        }

        /// <summary>
        /// Set CurrentWarp here to ensure warp gets set correctly without needing to access game save
        /// </summary>
        private void Candy_Warp(On.Candy.orig_Warp orig, Candy self, int dungeonLevel)
        {
            self.isCasting = true;
            self.npcState.warpDungeonLevel = CurrentWarp = dungeonLevel;
            self.warpMenu.gameObject.SetActive(false);
            self.animator.Play("cast");
        }

        /// <summary>
        /// In vanilla, Candy only has access to 3/5 areas. This adds the final two to her menu.
        /// </summary>
        private void WarpMenu_UpdateOptions(On.WarpMenu.orig_UpdateOptions orig, WarpMenu self)
        {
            if (self.optionsData.Length == 3) // By default, Candy does not give access to the final two areas. Add them here manually.
            {
                var options = self.optionsData.ToList();
                options.Add(new WarpMenu.WarpMenuOption { dungeonLevel = 4, price = 0, label = new LocalizedString("StringTable", "Frozen Passage") });
                options.Add(new WarpMenu.WarpMenuOption { dungeonLevel = 5, price = 0, label = new LocalizedString("StringTable", "Twisted Sanctum") });
                self.optionsData = options.ToArray();
            }
            orig(self);
        }

        /// <summary>
        /// Start a run with the current warp.
        /// QOL Change: Typically this method resets current warp. Instead, this maintains it.
        /// </summary>
        private void GameManager_StartRun(On.GameManager.orig_StartRun orig, GameManager self, Scarlet scarlet)
        {
            scarlet.sin = 0;
            scarlet.ResetState();
            self.activeSaveSlot.UpdateScarletState(scarlet);
            GameManager.SetGamePaused(false, true);
            GameManager.canGamePauseStateBeSetByPlayer = true;
            SceneManager.LoadScene($"Dungeon_{CurrentWarp}", LoadSceneMode.Single);
        }

        #endregion
    }
}
