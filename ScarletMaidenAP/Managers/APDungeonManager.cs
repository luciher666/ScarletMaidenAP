using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ScarletMaidenAP.Managers
{
    public class APDungeonManager
    {
        public Sprite OpenGate;
        public Sprite ClosedGate;

        public bool[] BossKeys =
        [
            true,
            true,
            true,
            true,
            true
        ];

        public APDungeonManager()
        {
            return;
            // Unused currently
            On.DungeonExitDoor.Update += DungeonExitDoor_Update;
            On.DungeonExitDoor.Start += DungeonExitDoor_Start;
            On.DungeonExitDoor.HandleInput += DungeonExitDoor_HandleInput;
        }

        private void DungeonExitDoor_HandleInput(On.DungeonExitDoor.orig_HandleInput orig, DungeonExitDoor self)
        {
            if (ShouldGateBeClosed())
            {
                return;
            }

            orig(self);
        }

        public void DungeonExitDoor_Start(On.DungeonExitDoor.orig_Start orig, DungeonExitDoor self)
        {
            if (OpenGate == null)
            {
                OpenGate = self.GetComponent<SpriteRenderer>().sprite;
                ClosedGate = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault(o => o.name.Equals("gateway1_6"));
            }
            orig(self);
        }

        public void DungeonExitDoor_Update(On.DungeonExitDoor.orig_Update orig, DungeonExitDoor self)
        {
            self.GetComponent<SpriteRenderer>().sprite = ShouldGateBeClosed() ? ClosedGate : OpenGate;
            orig(self);
        }

        public bool ShouldGateBeClosed()
        {
            return StatsManager.instance.GetRunState().currentSubDungeonIndex == 2 && !BossKeys[StatsManager.instance.GetRunState().currentDungeonLevel];
        }
    }
}
