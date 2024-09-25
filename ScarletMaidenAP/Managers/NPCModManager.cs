using Rewired;
using System.Linq;
using System.Security.Cryptography;
using BepInEx5ArchipelagoPluginTemplate.templates;
using UnityEngine;
using UnityEngine.Localization;
using static UnityEngine.Object;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
            On.Blacksmith.GetMaxXPInCurrentLevel += Blacksmith_GetMaxXPInCurrentLevel;
            On.Blacksmith.GetXP += Blacksmith_GetXP;
            On.Blacksmith.PurchaseXP += Blacksmith_PurchaseXP;
            On.LootManager.DropLootForSmith += LootManager_DropLootForSmith;
            // Roman
            On.Roman.Start += Roman_Start;
            On.Roman.GetLevel += Roman_GetLevel;
            // Faelina
            On.Faelina.Start += Faelina_Start;
            On.Faelina.GetLevel += Faelina_GetLevel;
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

        public static int BlacksmithLevelReceived = 500;
        public static bool BlacksmithSent = true;
        public static int BlacksmithLevelSent = 1; // MUST START AT 1!
        public static int BlacksmithXP = 0;

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
                    Destroy(self.signPost);
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
            //Plugin.BepinLogger.LogWarning($"Smith level checked. Caller: {caller}");
            return caller.Contains("DropLoot") ? BlacksmithLevelReceived : BlacksmithLevelSent;
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

        public static int RomanLevelReceived = 500;
        public static bool RomanSent = true;
        public static int RomanLevelSent = 1; // MUST START AT 1!
        public static int RomanXP = 0;

        /// <summary>
        /// Sets the level to the current received and clear the loot cache
        /// </summary>
        private void NPCStateRoman_Init(On.NPCStateRoman.orig_Init orig, NPCStateRoman self)
        {
            self.level = RomanLevelReceived;
            self.lootCache = null;
            orig(self);
        }

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

        private int Roman_GetLevel(On.Roman.orig_GetLevel orig, Roman self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;
            Plugin.BepinLogger.LogWarning($"Roman level checked. Caller: {caller}");
            return caller.Contains("DropLoot") ? RomanLevelReceived : RomanLevelSent;
        }

        #endregion

        #region Faelina

        public static int FaelinaLevelReceived = 500;
        public static bool FaelinaSent = true;
        public static int FaelinaLevelSent = 1; // MUST START AT 1!
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

        private int Faelina_GetLevel(On.Faelina.orig_GetLevel orig, Faelina self)
        {
            var caller = (new System.Diagnostics.StackTrace()).GetFrame(2).GetMethod().Name;
            Plugin.BepinLogger.LogWarning($"Faelina level checked. Caller: {caller}");
            return caller.Contains("DropLoot") ? FaelinaLevelReceived : FaelinaLevelSent;
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
