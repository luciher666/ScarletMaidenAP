using UnityEngine;

namespace ScarletMaidenAP.Managers
{
    public class GalleryModManager
    {
        public GalleryModManager()
        {
            On.BaseSinPump.OnMiniGameMiss += BaseSinPump_OnMiniGameMiss;
            On.BaseSinPump.OnExitMinigame += BaseSinPump_OnExitMinigame;
            On.BaseBossMinigame.OnMinigameEnded += BaseBossMinigame_OnMinigameEnded;
            On.BaseBossMinigame.Die += BaseBossMinigame_Die;
        }

        public int RequiredPumps = 2;

        /// <summary>
        /// Check the required hit count and unlock the gallery asset if applicable.
        /// </summary>
        public void UnlockGallery(GalleryAssetUnlocker galleryAssetUnlocker, int hitCount)
        {
            if (galleryAssetUnlocker == null) return;
            if (hitCount >= RequiredPumps)
            {
                Plugin.BepinLogger.LogMessage(
                    $"Unlocked gallery item: {galleryAssetUnlocker.galleryAsset.title.GetLocalizedString()}");
                galleryAssetUnlocker.UnlockAsset();
            }
            else
            {
                Plugin.BepinLogger.LogMessage($"Failed to unlock gallery item, hits: {hitCount}");
            }
        }

        /// <summary>
        /// This method is called when a base enemy minigame fails. Check hit count and unlock asset if applicable.
        /// </summary>
        private void BaseSinPump_OnMiniGameMiss(On.BaseSinPump.orig_OnMiniGameMiss orig, BaseSinPump self, int tryIndex, bool[] hits)
        {
            UnlockGallery(self.galleryAssetUnlocker, tryIndex);
            self.galleryAssetUnlocker = null;
            orig(self, tryIndex, hits);
        }

        /// <summary>
        /// This method is called when a base enemy minigame succeeds. Unlock asset accordingly.
        /// </summary>
        private void BaseSinPump_OnExitMinigame(On.BaseSinPump.orig_OnExitMinigame orig, BaseSinPump self, bool[] hits)
        {
            if (self.state == BaseSinPump.State.Done)
                return;
            UnlockGallery(self.galleryAssetUnlocker, 3);
            self.galleryAssetUnlocker = null;
            orig(self, hits);
        }

        /// <summary>
        /// This method is called when a boss minigame ends. Check heart count and unlock asset if applicable
        /// </summary>
        private void BaseBossMinigame_OnMinigameEnded(On.BaseBossMinigame.orig_OnMinigameEnded orig, BaseBossMinigame self, int countLootDrops)
        {
            UnlockGallery(self.galleryAssetUnlocker, countLootDrops);
            orig(self, countLootDrops);
        }

        /// <summary>
        /// Normally, boss gallery unlocker is called here. Remove this call to ensure everything functions as expected.
        /// </summary>
        private void BaseBossMinigame_Die(On.BaseBossMinigame.orig_Die orig, BaseBossMinigame self, GameObject effect)
        {
            if (effect != null)
            {
                Vector3 position = new Vector3(self.transform.position.x, self.transform.position.y, self.transform.position.z);
                RoomUtils.Instantiate(self.gameObject, effect, position);
            }
            AudioManager.instance.PlayRandomSoundFromList(AudioManager.instance.enemyDeathSounds);
            Object.Destroy(self.gameObject);
        }
    }
}
