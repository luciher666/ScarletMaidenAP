using System.Linq;
using BepInEx;
using BepInEx.Logging;
using BepInEx5ArchipelagoPluginTemplate.templates.Archipelago;
using BepInEx5ArchipelagoPluginTemplate.templates.Utils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using ScarletMaidenAP.Managers;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace BepInEx5ArchipelagoPluginTemplate.templates
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.luciher666.ScarletMaidenAP";
        public const string PluginName = "ScarletMaidenAP";
        public const string PluginVersion = "0.1.0";

        public const string ModDisplayInfo = $"{PluginName} v{PluginVersion}";
        private const string APDisplayInfo = $"Archipelago v{ArchipelagoClient.APVersion}";
        public static ManualLogSource BepinLogger;
        public static ArchipelagoClient ArchipelagoClient;
        public static Scarlet PlayerInstance;

        public static NPCModManager NPCManagerInstance;
        public static APSaveManager APSaveManagerInstance;

        private void Awake()
        {
            // Plugin startup logic
            BepinLogger = Logger;
            ArchipelagoClient = new ArchipelagoClient();
            ArchipelagoConsole.Awake();

            ArchipelagoConsole.LogMessage($"{ModDisplayInfo} loaded!");

            NPCManagerInstance = new NPCModManager();
            APSaveManagerInstance = new APSaveManager();

            On.SaveSlot.Save += SaveSlot_Save;
            //On.NPCStateCandy.Init += NPCStateCandy_Init;
            //On.BaseNPC.Start += BaseNPC_Start;
            On.Scarlet.Start += Scarlet_Start;
        }

        private void Scarlet_Start(On.Scarlet.orig_Start orig, Scarlet self)
        {
            PlayerInstance = self;
            self.DEBUG = true;
            self.DEBUG_infiniteHealth = true;
            self.DEBUG_infiniteMana = true;
            self.sin = 999999;
            orig(self);
        }


        private void SaveSlot_Save(On.SaveSlot.orig_Save orig, SaveSlot self)
        {
            BepinLogger.LogMessage("Save intercepted");
        }

        private void OnGUI()
        {
            // show the mod is currently loaded in the corner
            GUI.Label(new Rect(16, 16, 300, 20), ModDisplayInfo);
            ArchipelagoConsole.OnGUI();

            string statusMessage;
            // show the Archipelago Version and whether we're connected or not
            if (ArchipelagoClient.Authenticated)
            {
                // if your game doesn't usually show the cursor this line may be necessary
                // Cursor.visible = false;

                statusMessage = " Status: Connected";
                GUI.Label(new Rect(16, 50, 300, 20), APDisplayInfo + statusMessage);
            }
            else
            {
                // if your game doesn't usually show the cursor this line may be necessary
                // Cursor.visible = true;

                statusMessage = " Status: Disconnected";
                GUI.Label(new Rect(16, 50, 300, 20), APDisplayInfo + statusMessage);
                GUI.Label(new Rect(16, 70, 150, 20), "Host: ");
                GUI.Label(new Rect(16, 90, 150, 20), "Player Name: ");
                GUI.Label(new Rect(16, 110, 150, 20), "Password: ");

                ArchipelagoClient.ServerData.Uri = GUI.TextField(new Rect(150, 70, 150, 20),
                    ArchipelagoClient.ServerData.Uri);
                ArchipelagoClient.ServerData.SlotName = GUI.TextField(new Rect(150, 90, 150, 20),
                    ArchipelagoClient.ServerData.SlotName);
                ArchipelagoClient.ServerData.Password = GUI.TextField(new Rect(150, 110, 150, 20),
                    ArchipelagoClient.ServerData.Password);

                // requires that the player at least puts *something* in the slot name
                if (GUI.Button(new Rect(16, 130, 100, 20), "Connect") &&
                    !ArchipelagoClient.ServerData.SlotName.IsNullOrWhiteSpace())
                {
                    ArchipelagoClient.Connect();
                }
            }
            // this is a good place to create and add a bunch of debug buttons
        }
    }
}