from BaseClasses import Region, Location, Item, ItemClassification, Tutorial
from worlds.AutoWorld import World, WebWorld
from .Locations import ScarletMaidenLocation, location_table


class ScarletMaidenWeb(WebWorld):
    theme = "stone"
    setup_en = Tutorial(
        "Multiworld Setup Guide",
        "A guide to setting up Scarlet Maiden for Archipelago",
        "English",
        "setup_en.md",
        "setup/en",
        ["luciher666"]
    )
    tutorials = [setup_en]


class ScarletMaidenWorld(World):
    game = "Scarlet Maiden"
    web = ScarletMaidenWeb()
    location_id_to_name = {}
    item_id_to_name = {}

    def create_regions(self) -> None:
        self.multiworld.regions.append(Region("Menu", self.player, self.multiworld))

    def create_items(self) -> None:
        pass

    def create_item(self, name: str) -> "Item":
        item_class = self.get_item_classification(name)
        return ScarletMaidenItem(name, item_class, self.item_id_to_name.get(name, None), self.player)

    def get_item_classification(self, name: str) -> ItemClassification:
        return ItemClassification.progression


class ScarletMaidenItem(Item):
    game = "Scarlet Maiden"