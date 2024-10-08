using Random = UnityEngine.Random;

namespace ScarletMaidenAP.Managers
{
    public class ScarletModManager
    {
        public static Scarlet PlayerInstance;

        public ScarletModManager()
        {
            On.Scarlet.Awake += Scarlet_Awake;
            On.Scarlet.Start += Scarlet_Start;
            On.Scarlet.ResetItems += Scarlet_ResetItems;
            On.Scarlet.LoadState += Scarlet_LoadState;
        }

        private void Scarlet_LoadState(On.Scarlet.orig_LoadState orig, Scarlet self)
        {
            var scarletState = GameManager.instance.GetSaveSlot().GetGameState().scarletState;
            orig(self);
            if (scarletState.activeWeaponStatsInstance == null)
                self.SetActiveWeaponStatsInstance(GetWeapon(self).weaponStatsInstance);
        }

        private void Scarlet_Awake(On.Scarlet.orig_Awake orig, Scarlet self)
        {
            PlayerInstance = self;
            foreach (var spell in LootManager.instance.GetLevelSettings(1).spellPool)
            {
                Plugin.BepinLogger.LogWarning(spell.spell.title.GetLocalizedString());
            }
            self.defaultSpell = GetSpell(self);
            orig(self);
        }

        public enum SpellType
        {
            ArcaneOrb = 0,
            IceShard = 1,
            FlameArrow = 2,
            ArcaneBoomerang = 3,
            FistOfFire = 4,
            FireShield = 5,
            ArcaneSubmission = 6,
            None = -1,
            Random = 99
        }

        public SpellType DefaultSpell = SpellType.Random;

        public Spell GetSpell(Scarlet self)
        {
            switch (DefaultSpell)
            {
                case SpellType.None:
                    return null;
                case SpellType.ArcaneOrb:
                    return self.defaultSpell;
                case SpellType.Random:
                    return Random.Range(0, 7) == 0
                        ? self.defaultSpell // arcane orb doesn't appear in level 1 loot table
                        : LootManager.instance.GetLevelSettings(1).GetRandomSpell();
                default:
                    return LootManager.instance.GetLevelSettings(1).spellPool[(int) DefaultSpell - 1].spell;
            }
        }

        public enum WeaponType
        {
            Sword = 0,
            Spear = 1,
            Dagger = 2,
            Hammer = 3,
            Random = 99
        }

        public UIWeapon GetWeapon(Scarlet self)
        {
            var weaponType = DefaultWeapon;
            if (weaponType == WeaponType.Random)
            {
                weaponType = (WeaponType)Random.Range(0, 4);
            }
            switch (weaponType)
            {
                case WeaponType.Spear:
                    return self.spear;
                case WeaponType.Dagger:
                    return self.dagger;
                case WeaponType.Hammer:
                    return self.hammer;
                case WeaponType.Sword:
                default:
                    return self.sword;
            }
        }

        public WeaponType DefaultWeapon = WeaponType.Random;
        private void Scarlet_ResetItems(On.Scarlet.orig_ResetItems orig, Scarlet self)
        {
            self.activeAmuletInstance = null;
            self.activeRingInstance = null;
            self.activeSpellInstance = null;
            self.SetActiveWeaponStatsInstance(GetWeapon(self).weaponStatsInstance);
        }

        private void Scarlet_Start(On.Scarlet.orig_Start orig, Scarlet self)
        {
#if DEBUG
            self.DEBUG = true;
            self.DEBUG_infiniteHealth = true;
            self.DEBUG_infiniteMana = true;
            self.sin = int.MaxValue;
#endif
            orig(self);
        }
    }
}
