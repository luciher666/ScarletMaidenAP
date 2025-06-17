from typing import Dict, NamedTuple, Optional

from BaseClasses import Location
from .Regions import RegionNames, NPCRegions

class ScarletMaidenLocation(Location):
    game: str = "Scarlet Maiden"


class ScarletMaidenLocationData(NamedTuple):
    category: str
    code: Optional[int]
    regions: Optional[list[str]] = None


def get_locations_by_category(category: str) -> Dict[str, ScarletMaidenLocationData]:
    location_dict: Dict[str, ScarletMaidenLocationData] = {}
    for name, data in location_table.items():
        if data.category == category:
            location_dict.setdefault(name, data)

    return location_dict

location_table: Dict[str, ScarletMaidenLocationData] = {
    # Skill Tree (Non-Progressive)
    "Vitality":                         ScarletMaidenLocation("Skill Tree", 1_00_00),
    # Skill Tree (Progressive)

    # Blacksmith
    "Rescue Bruce":                     ScarletMaidenLocationData("Bruce", 100_0000),
    **{f"Bruce Level {i}":              ScarletMaidenLocationData("Bruce Level", 100_0000 + i) for i in range(1, 26)},

    # Roman
    "Rescue Roman":                     ScarletMaidenLocationData("Roman", 200_0000),
    **{f"Roman Level {i}":              ScarletMaidenLocationData("Roman Level", 200_0000 + i) for i in range(1, 26)},

    # Faelina
    "Rescue Faelina":                   ScarletMaidenLocationData("Faelina", 300_0000),
    **{f"Faelina Level {i}":            ScarletMaidenLocationData("Faelina Level", 300_0000 + i) for i in range(1, 26)},

    # Candy
    "Rescue Candy":                         ScarletMaidenLocationData("Candy", 400_0000),
    "Mines of Sorrow - Area Complete":      ScarletMaidenLocation("Candy Teleport", 400_0001),
    "The Royal Library - Area Complete":    ScarletMaidenLocation("Candy Teleport", 400_0002),
    "Subterranean Forest - Area Complete":  ScarletMaidenLocation("Candy Teleport", 400_0003),
    "Frozen Passage - Area Complete":       ScarletMaidenLocation("Extra Candy Teleport", 400_0004),
    "Twisted Sanctum - Area Complete":      ScarletMaidenLocation("Extra Candy Teleport", 400_0005),

    # Gallerysanity
    "Gallerysanity: Orc Warrior":               ScarletMaidenLocation("Gallery", 20_001_0, NPCRegions.OrcWarrior),
    "Gallerysanity: Goblin Shooter":            ScarletMaidenLocation("Gallery", 20_002_0, NPCRegions.GoblinShooter),
    "Gallerysanity: Plant Girl":                ScarletMaidenLocation("Gallery", 20_003_0, NPCRegions.PlantGirl),
    "Gallerysanity: Triboob Huntress":          ScarletMaidenLocation("Gallery", 20_004_0, NPCRegions.TriboobHuntress),
    "Gallerysanity: Wicked Witch":              ScarletMaidenLocation("Gallery", 20_005_0, NPCRegions.WickedWitch),
    "Gallerysanity: Aqua Slime":                ScarletMaidenLocation("Gallery", 20_006_0, NPCRegions.AquaSlime),
    "Gallerysanity: Dark Elf Assassin":         ScarletMaidenLocation("Gallery", 20_007_0, NPCRegions.DarkElfAssassin),
    "Gallerysanity: Ruby Slime":                ScarletMaidenLocation("Gallery", 20_008_0, NPCRegions.RubySlime),
    "Gallerysanity: Orc Axe Thrower (Green)":   ScarletMaidenLocation("Gallery", 20_009_0, NPCRegions.OrcAxeThrowerGreen),
    "Gallerysanity: Succubus":                  ScarletMaidenLocation("Gallery", 20_010_0, NPCRegions.Succubus),
    "Gallerysanity: Wolfina":                   ScarletMaidenLocation("Gallery", 20_011_0, NPCRegions.Wolfina),
    "Gallerysanity: Gimp":                      ScarletMaidenLocation("Gallery", 20_012_0, NPCRegions.Gimp),
    "Gallerysanity: Fairie":                    ScarletMaidenLocation("Gallery", 20_013_0, NPCRegions.Fairie),
    "Gallerysanity: Triboob Crusher":           ScarletMaidenLocation("Gallery", 20_014_0, NPCRegions.TriboobCrusher),
    "Gallerysanity: Dark Elf Goddess":          ScarletMaidenLocation("Gallery", 20_015_0, NPCRegions.DarkElfGoddess),
    "Gallerysanity: Emerald Slime":             ScarletMaidenLocation("Gallery", 20_016_0, NPCRegions.EmeraldSlime),
    "Gallerysanity: Ogress":                    ScarletMaidenLocation("Gallery", 20_017_0, NPCRegions.Ogress),
    "Gallerysanity: Poison Ivy":                ScarletMaidenLocation("Gallery", 20_018_0, NPCRegions.PoisonIvy),
    "Gallerysanity: Corrupted Witch":           ScarletMaidenLocation("Gallery", 20_019_0, NPCRegions.CorruptedWitch),
    "Gallerysanity: Goblin Bomber":             ScarletMaidenLocation("Gallery", 20_020_0, NPCRegions.GoblinBomber),
    "Gallerysanity: Triboob Dragonslayer":      ScarletMaidenLocation("Gallery", 20_021_0, NPCRegions.TriboobDragonslayer),
    "Gallerysanity: Mama Slime":                ScarletMaidenLocation("Gallery", 20_022_0, NPCRegions.MamaSlime),
    "Gallerysanity: Shield Maiden":             ScarletMaidenLocation("Gallery", 20_023_0, NPCRegions.ShieldMaiden),
    "Gallerysanity: Snake Girl":                ScarletMaidenLocation("Gallery", 20_024_0, NPCRegions.SnakeGirl),
    "Gallerysanity: Dark Succubus":             ScarletMaidenLocation("Gallery", 20_025_0, NPCRegions.DarkSuccubus),
    "Gallerysanity: Orcish Gimp":               ScarletMaidenLocation("Gallery", 20_026_0, NPCRegions.OrcishGimp),
    "Gallerysanity: Foxy":                      ScarletMaidenLocation("Gallery", 20_027_0, NPCRegions.Foxy),
    "Gallerysanity: Orc Axe Thrower (Pink)":    ScarletMaidenLocation("Gallery", 20_028_0, NPCRegions.OrcAxeThrowerPink),
    "Gallerysanity: Snake Huntress":            ScarletMaidenLocation("Gallery", 20_029_0, NPCRegions.SnakeHuntress),
    "Gallerysanity: Imp Girl":                  ScarletMaidenLocation("Gallery", 20_030_0, NPCRegions.ImpGirl),
    "Gallerysanity: Golden Slime":              ScarletMaidenLocation("Gallery", 20_031_0, NPCRegions.GoldenSlime),
    "Gallerysanity: Count Cockula":             ScarletMaidenLocation("Gallery", 20_032_0, NPCRegions.CountCockula),
    "Gallerysanity: Elven Fountain":            ScarletMaidenLocation("Gallery", 20_033_0, NPCRegions.ElvenFountain),
    "Gallerysanity: Mysterious Djinn":          ScarletMaidenLocation("Gallery", 20_034_0, NPCRegions.MysteriousDjinn),
    "Gallerysanity: Horny Ogre":                ScarletMaidenLocation("Gallery", 20_035_0, NPCRegions.HornyOgre),
    "Gallerysanity: Underworld Nurse":          ScarletMaidenLocation("Gallery", 20_036_0, NPCRegions.UnderworldNurse),
    "Gallerysanity: Lactating Goddess":         ScarletMaidenLocation("Gallery", 20_037_0, NPCRegions.LactatingGoddess),
    "Gallerysanity: Shy Elf":                   ScarletMaidenLocation("Gallery", 20_038_0, NPCRegions.ShyElf),
    "Gallerysanity: Lusty Giant":               ScarletMaidenLocation("Gallery", 20_039_0, NPCRegions.LustyGiant),
    "Gallerysanity: Handsome Adventurer":       ScarletMaidenLocation("Gallery", 20_040_0, NPCRegions.HandsomeAdventurer),
    "Gallerysanity: Hungry Gnome":              ScarletMaidenLocation("Gallery", 20_041_0, NPCRegions.HungryGnome),
    "Gallerysanity: Creepy Bush":               ScarletMaidenLocation("Gallery", 20_042_0, NPCRegions.CreepyBush),
    "Gallerysanity: Frozen Maiden":             ScarletMaidenLocation("Gallery", 20_043_0, NPCRegions.FrozenMaiden),
    "Gallerysanity: Frosty Traveler":           ScarletMaidenLocation("Gallery", 20_044_0, NPCRegions.FrostyTraveler),
    "Gallerysanity: Mimic":                     ScarletMaidenLocation("Gallery", 20_045_0, NPCRegions.Mimic),
    "Gallerysanity: Rigor Mortis":              ScarletMaidenLocation("Gallery", 20_046_0, NPCRegions.RigorMortis),
    "Gallerysanity: Goblin King":               ScarletMaidenLocation("Gallery", 20_047_0, NPCRegions.GoblinKing),
    "Gallerysanity: Monument of Strength":      ScarletMaidenLocation("Gallery", 20_048_0, NPCRegions.MonumentOfStrength),
    "Gallerysanity: Friendly Witch":            ScarletMaidenLocation("Gallery", 20_049_0, NPCRegions.FriendlyWitch),
    "Gallerysanity: Stoned Wizard":             ScarletMaidenLocation("Gallery", 20_050_0, NPCRegions.StonedWizard),
    "Gallerysanity: Magic Dragon":              ScarletMaidenLocation("Gallery", 20_051_0, NPCRegions.MagicDragon),
    "Gallerysanity: Muscle Goddess":            ScarletMaidenLocation("Gallery", 20_052_0, NPCRegions.MuscleGoddess),
    "Gallerysanity: Medicine Man":              ScarletMaidenLocation("Gallery", 20_053_0, NPCRegions.MedicineMan),
    "Gallerysanity: Marvin's Mana Pump":        ScarletMaidenLocation("Gallery", 20_054_0, NPCRegions.MarvinsManaPump),
    "Gallerysanity: Steamy Contraptions":       ScarletMaidenLocation("Gallery", 20_055_0),
    "Gallerysanity: Masquerade":                ScarletMaidenLocation("Gallery", 20_056_0),
    "Gallerysanity: Magic Flute":               ScarletMaidenLocation("Gallery", 20_057_0),
    "Gallerysanity: Green Delight":             ScarletMaidenLocation("Gallery", 20_058_0),
    "Gallerysanity: It's a Strap!":             ScarletMaidenLocation("Gallery", 20_059_0),
    "Gallerysanity: Gold Digger":               ScarletMaidenLocation("Gallery", 20_060_0),
    "Gallerysanity: Portal of Pleasure":        ScarletMaidenLocation("Gallery", 20_061_0),
    "Gallerysanity: Earth Mother":              ScarletMaidenLocation("Gallery", 20_062_0, [RegionNames.MinesBoss]),
    "Gallerysanity: The Grand Archivist":       ScarletMaidenLocation("Gallery", 20_063_0, [RegionNames.LibraryBoss]),
    "Gallerysanity: Queen Lily":                ScarletMaidenLocation("Gallery", 20_064_0, [RegionNames.ForestBoss]),
    "Gallerysanity: Cold Hearted Bitch":        ScarletMaidenLocation("Gallery", 20_065_0, [RegionNames.FrozenBoss]),
    "Gallerysanity: Corrupted Maiden":          ScarletMaidenLocation("Gallery - Postgame", 20_066_0, [RegionNames.SanctumBoss]),
    "Gallerysanity: Scarlet & Faelina":         ScarletMaidenLocation("Gallery - Postgame", 20_067_0, [RegionNames.SanctumBoss]),
    "Gallerysanity: Misty & Candy":             ScarletMaidenLocation("Gallery - Postgame", 20_068_0, [RegionNames.SanctumBoss]),
    "Gallerysanity: Roman & Tiger":             ScarletMaidenLocation("Gallery - Postgame", 20_069_0, [RegionNames.SanctumBoss]),

    # Fucksanity
    "Fucksanity: Orc Warrior":                  ScarletMaidenLocation("Fucksanity", 30_001_0, NPCRegions.OrcWarrior),
    "Fucksanity: Goblin Shooter":               ScarletMaidenLocation("Fucksanity", 30_002_0, NPCRegions.GoblinShooter),
    "Fucksanity: Plant Girl":                   ScarletMaidenLocation("Fucksanity", 30_003_0, NPCRegions.PlantGirl),
    "Fucksanity: Triboob Huntress":             ScarletMaidenLocation("Fucksanity", 30_004_0, NPCRegions.TriboobHuntress),
    "Fucksanity: Wicked Witch":                 ScarletMaidenLocation("Fucksanity", 30_005_0, NPCRegions.WickedWitch),
    "Fucksanity: Aqua Slime":                   ScarletMaidenLocation("Fucksanity", 30_006_0, NPCRegions.AquaSlime),
    "Fucksanity: Dark Elf Assassin":            ScarletMaidenLocation("Fucksanity", 30_007_0, NPCRegions.DarkElfAssassin),
    "Fucksanity: Ruby Slime":                   ScarletMaidenLocation("Fucksanity", 30_008_0, NPCRegions.RubySlime),
    "Fucksanity: Orc Axe Thrower (Green)":      ScarletMaidenLocation("Fucksanity", 30_009_0, NPCRegions.OrcAxeThrowerGreen),
    "Fucksanity: Succubus":                     ScarletMaidenLocation("Fucksanity", 30_010_0, NPCRegions.Succubus),
    "Fucksanity: Wolfina":                      ScarletMaidenLocation("Fucksanity", 30_011_0, NPCRegions.Wolfina),
    "Fucksanity: Gimp":                         ScarletMaidenLocation("Fucksanity", 30_012_0, NPCRegions.Gimp),
    "Fucksanity: Fairie":                       ScarletMaidenLocation("Fucksanity", 30_013_0, NPCRegions.Fairie),
    "Fucksanity: Triboob Crusher":              ScarletMaidenLocation("Fucksanity", 30_014_0, NPCRegions.TriboobCrusher),
    "Fucksanity: Dark Elf Goddess":             ScarletMaidenLocation("Fucksanity", 30_015_0, NPCRegions.DarkElfGoddess),
    "Fucksanity: Emerald Slime":                ScarletMaidenLocation("Fucksanity", 30_016_0, NPCRegions.EmeraldSlime),
    "Fucksanity: Ogress":                       ScarletMaidenLocation("Fucksanity", 30_017_0, NPCRegions.Ogress),
    "Fucksanity: Poison Ivy":                   ScarletMaidenLocation("Fucksanity", 30_018_0, NPCRegions.PoisonIvy),
    "Fucksanity: Corrupted Witch":              ScarletMaidenLocation("Fucksanity", 30_019_0, NPCRegions.CorruptedWitch),
    "Fucksanity: Goblin Bomber":                ScarletMaidenLocation("Fucksanity", 30_020_0, NPCRegions.GoblinBomber),
    "Fucksanity: Triboob Dragonslayer":         ScarletMaidenLocation("Fucksanity", 30_021_0, NPCRegions.TriboobDragonslayer),
    "Fucksanity: Mama Slime":                   ScarletMaidenLocation("Fucksanity", 30_022_0, NPCRegions.MamaSlime),
    "Fucksanity: Shield Maiden":                ScarletMaidenLocation("Fucksanity", 30_023_0, NPCRegions.ShieldMaiden),
    "Fucksanity: Snake Girl":                   ScarletMaidenLocation("Fucksanity", 30_024_0, NPCRegions.SnakeGirl),
    "Fucksanity: Dark Succubus":                ScarletMaidenLocation("Fucksanity", 30_025_0, NPCRegions.DarkSuccubus),
    "Fucksanity: Orcish Gimp":                  ScarletMaidenLocation("Fucksanity", 30_026_0, NPCRegions.OrcishGimp),
    "Fucksanity: Foxy":                         ScarletMaidenLocation("Fucksanity", 30_027_0, NPCRegions.Foxy),
    "Fucksanity: Orc Axe Thrower (Pink)":       ScarletMaidenLocation("Fucksanity", 30_028_0, NPCRegions.OrcAxeThrowerPink),
    "Fucksanity: Snake Huntress":               ScarletMaidenLocation("Fucksanity", 30_029_0, NPCRegions.SnakeHuntress),
    "Fucksanity: Imp Girl":                     ScarletMaidenLocation("Fucksanity", 30_030_0, NPCRegions.ImpGirl),
    "Fucksanity: Golden Slime":                 ScarletMaidenLocation("Fucksanity", 30_031_0, NPCRegions.GoldenSlime),
    "Fucksanity: Earth Mother":                 ScarletMaidenLocation("Fucksanity - Boss", 30_062_0, [RegionNames.MinesBoss]),
    "Fucksanity: The Grand Archivist":          ScarletMaidenLocation("Fucksanity - Boss", 30_063_0, [RegionNames.LibraryBoss]),
    "Fucksanity: Queen Lily":                   ScarletMaidenLocation("Fucksanity - Boss", 30_064_0, [RegionNames.ForestBoss]),
    "Fucksanity: Cold Hearted Bitch":           ScarletMaidenLocation("Fucksanity - Boss", 30_065_0, [RegionNames.FrozenBoss]),
    "Fucksanity: Corrupted Maiden":             ScarletMaidenLocation("Fucksanity - Postgame", 30_066_0, [RegionNames.SanctumBoss]),
    
    # Heartsanity/Pumpsanity
    **{f"Heartsanity: Orc Warrior {i}":             ScarletMaidenLocationData("Heartsanity", 30_001_0 + i, NPCRegions.OrcWarrior) for i in range(1, 4)},
    **{f"Heartsanity: Goblin Shooter {i}":          ScarletMaidenLocationData("Heartsanity", 30_002_0 + i, NPCRegions.GoblinShooter) for i in range(1, 4)},
    **{f"Heartsanity: Plant Girl {i}":              ScarletMaidenLocationData("Heartsanity", 30_003_0 + i, NPCRegions.PlantGirl) for i in range(1, 4)},
    **{f"Heartsanity: Triboob Huntress {i}":        ScarletMaidenLocationData("Heartsanity", 30_004_0 + i, NPCRegions.TriboobHuntress) for i in range(1, 4)},
    **{f"Heartsanity: Wicked Witch {i}":            ScarletMaidenLocationData("Heartsanity", 30_005_0 + i, NPCRegions.WickedWitch) for i in range(1, 4)},
    **{f"Heartsanity: Aqua Slime {i}":              ScarletMaidenLocationData("Heartsanity", 30_006_0 + i, NPCRegions.AquaSlime) for i in range(1, 4)},
    **{f"Heartsanity: Dark Elf Assassin {i}":       ScarletMaidenLocationData("Heartsanity", 30_007_0 + i, NPCRegions.DarkElfAssassin) for i in range(1, 4)},
    **{f"Heartsanity: Ruby Slime {i}":              ScarletMaidenLocationData("Heartsanity", 30_008_0 + i, NPCRegions.RubySlime) for i in range(1, 4)},
    **{f"Heartsanity: Orc Axe Thrower (Green) {i}": ScarletMaidenLocationData("Heartsanity", 30_009_0 + i, NPCRegions.OrcAxeThrowerGreen) for i in range(1, 4)},
    **{f"Heartsanity: Succubus {i}":                ScarletMaidenLocationData("Heartsanity", 30_010_0 + i, NPCRegions.Succubus) for i in range(1, 4)},
    **{f"Heartsanity: Wolfina {i}":                 ScarletMaidenLocationData("Heartsanity", 30_011_0 + i, NPCRegions.Wolfina) for i in range(1, 4)},
    **{f"Heartsanity: Gimp {i}":                    ScarletMaidenLocationData("Heartsanity", 30_012_0 + i, NPCRegions.Gimp) for i in range(1, 4)},
    **{f"Heartsanity: Fairie {i}":                  ScarletMaidenLocationData("Heartsanity", 30_013_0 + i, NPCRegions.Fairie) for i in range(1, 4)},
    **{f"Heartsanity: Triboob Crusher {i}":         ScarletMaidenLocationData("Heartsanity", 30_014_0 + i, NPCRegions.TriboobCrusher) for i in range(1, 4)},
    **{f"Heartsanity: Dark Elf Goddess {i}":        ScarletMaidenLocationData("Heartsanity", 30_015_0 + i, NPCRegions.DarkElfGoddess) for i in range(1, 4)},
    **{f"Heartsanity: Emerald Slime {i}":           ScarletMaidenLocationData("Heartsanity", 30_016_0 + i, NPCRegions.EmeraldSlime) for i in range(1, 4)},
    **{f"Heartsanity: Ogress {i}":                  ScarletMaidenLocationData("Heartsanity", 30_017_0 + i, NPCRegions.Ogress) for i in range(1, 4)},
    **{f"Heartsanity: Poison Ivy {i}":              ScarletMaidenLocationData("Heartsanity", 30_018_0 + i, NPCRegions.PoisonIvy) for i in range(1, 4)},
    **{f"Heartsanity: Corrupted Witch {i}":         ScarletMaidenLocationData("Heartsanity", 30_019_0 + i, NPCRegions.CorruptedWitch) for i in range(1, 4)},
    **{f"Heartsanity: Goblin Bomber {i}":           ScarletMaidenLocationData("Heartsanity", 30_020_0 + i, NPCRegions.GoblinBomber) for i in range(1, 4)},
    **{f"Heartsanity: Triboob Dragonslayer {i}":    ScarletMaidenLocationData("Heartsanity", 30_021_0 + i, NPCRegions.TriboobDragonslayer) for i in range(1, 4)},
    **{f"Heartsanity: Mama Slime {i}":              ScarletMaidenLocationData("Heartsanity", 30_022_0 + i, NPCRegions.MamaSlime) for i in range(1, 4)},
    **{f"Heartsanity: Shield Maiden {i}":           ScarletMaidenLocationData("Heartsanity", 30_023_0 + i, NPCRegions.ShieldMaiden) for i in range(1, 4)},
    **{f"Heartsanity: Snake Girl {i}":              ScarletMaidenLocationData("Heartsanity", 30_024_0 + i, NPCRegions.SnakeGirl) for i in range(1, 4)},
    **{f"Heartsanity: Dark Succubus {i}":           ScarletMaidenLocationData("Heartsanity", 30_025_0 + i, NPCRegions.DarkSuccubus) for i in range(1, 4)},
    **{f"Heartsanity: Orcish Gimp {i}":             ScarletMaidenLocationData("Heartsanity", 30_026_0 + i, NPCRegions.OrcishGimp) for i in range(1, 4)},
    **{f"Heartsanity: Foxy {i}":                    ScarletMaidenLocationData("Heartsanity", 30_027_0 + i, NPCRegions.Foxy) for i in range(1, 4)},
    **{f"Heartsanity: Orc Axe Thrower (Pink) {i}":  ScarletMaidenLocationData("Heartsanity", 30_028_0 + i, NPCRegions.OrcAxeThrowerPink) for i in range(1, 4)},
    **{f"Heartsanity: Snake Huntress {i}":          ScarletMaidenLocationData("Heartsanity", 30_029_0 + i, NPCRegions.SnakeHuntress) for i in range(1, 4)},
    **{f"Heartsanity: Imp Girl {i}":                ScarletMaidenLocationData("Heartsanity", 30_030_0 + i, NPCRegions.ImpGirl) for i in range(1, 4)},
    **{f"Heartsanity: Golden Slime {i}":            ScarletMaidenLocationData("Heartsanity", 30_031_0 + i, NPCRegions.GoldenSlime) for i in range(1, 4)},
    **{f"Heartsanity: Earth Mother {i}":            ScarletMaidenLocationData("Heartsanity - Boss", 30_062_0 + i, [RegionNames.MinesBoss]) for i in range(1, 4)},
    **{f"Heartsanity: The Grand Archivist {i}":     ScarletMaidenLocationData("Heartsanity - Boss", 30_063_0 + i, [RegionNames.LibraryBoss]) for i in range(1, 4)},
    **{f"Heartsanity: Queen Lily {i}":              ScarletMaidenLocationData("Heartsanity - Boss", 30_064_0 + i, [RegionNames.ForestBoss]) for i in range(1, 4)},
    **{f"Heartsanity: Cold Hearted Bitch {i}":      ScarletMaidenLocationData("Heartsanity - Boss", 30_065_0 + i, [RegionNames.FrozenBoss]) for i in range(1, 4)},
}