using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScarletMaidenAP.Managers
{
    public class SkillManager
    {
        public SkillManager()
        {
            On.SkillTreeGrid.Start += SkillTreeGrid_Start;
            On.SkillTreeGrid.BuildGrid += SkillTreeGrid_BuildGrid;
            On.SkillTreeGrid.UpgradeSkill += SkillTreeGrid_UpgradeSkill;
        }

        private void SkillTreeGrid_UpgradeSkill(On.SkillTreeGrid.orig_UpgradeSkill orig, SkillTreeGrid self)
        {
            orig(self);
        }

        private void SkillTreeGrid_Start(On.SkillTreeGrid.orig_Start orig, SkillTreeGrid self)
        {
            self.DEBUG = true;
            orig(self);
        }

        public void SkillTreeGrid_BuildGrid(On.SkillTreeGrid.orig_BuildGrid orig, SkillTreeGrid self)
        {
            self.scarlet = GameObject.FindGameObjectWithTag("Scarlet").GetComponent<Scarlet>();
            self.layoutRows = ScarletStatManager.instance.skillTree.rows.Length;
            self.layoutColumns = ScarletStatManager.instance.skillTree.rows[0].row.Length;
            self.itemGrid = new SkillGridItem[self.layoutRows, self.layoutColumns];
            self.pointerR = self.startCellRow;
            self.pointerC = self.startCellColumn;
            foreach (Component component in self.transform)
                Object.Destroy(component.gameObject);
            var locations = new List<Tuple<int, int, string, int>>();
            Plugin.BepinLogger.LogWarning("Building Skill Tree");
            for (var r = 0; r < self.layoutRows; ++r)
            {
                var row = ScarletStatManager.instance.skillTree.rows[r];
                for (var c = 0; c < self.layoutColumns; ++c)
                {
                    var skill = row.row[c];
                    var skillGridItem = Object.Instantiate(self.gridItemPrefab, Vector3.zero, Quaternion.identity, self.transform);
                    skillGridItem.SetSelected(false);
                    self.itemGrid[r, c] = skillGridItem;
                    if (!skill)
                    {
                        skillGridItem.SetSkillInstance(null, 0, 0);
                        Plugin.BepinLogger.LogError($"Null skill at [{r},{c}]");
                    }
                    else
                    {
                        var instanceWithSkill = self.scarlet.GetSkillInstanceWithSkill(skill);
                        skillGridItem.SetSkillInstance(instanceWithSkill, r, c);
                        Plugin.BepinLogger.LogWarning($"{skill.title.GetLocalizedString()} at [{r},{c}]");
                        locations.Add(new Tuple<int, int, string, int>(r, c, skill.title.GetLocalizedString(), skill.maxLevel));
                    }
                }
            }
            self.SetUpItems();
            var skillsCode = "    # Skill Tree (Non-Progressive)\n";
            locations = locations.OrderBy(o => o.Item2).ToList();
            var longestSkillName = locations.OrderByDescending(o => o.Item3.Length).First();
            foreach (var location in locations)
            {
                var spaces = string.Empty;
                for (var i = 0; i < longestSkillName.Item3.Length - location.Item3.Length + 4; i++)
                {
                    spaces += " ";
                }
                skillsCode += $"    \"{location.Item3}\":{spaces}ScarletMaidenLocation(\"Skill Tree\", 1_{location.Item2}_{location.Item1}_00),\n";
            }
            skillsCode += "\n    # Skill Tree (Progressive)\n";
            foreach (var location in locations)
            {
                var spaces = string.Empty;
                for (var i = 0; i < longestSkillName.Item3.Length - location.Item3.Length + 2; i++)
                {
                    spaces += " ";
                }
                skillsCode += $"    **{{f\"{location.Item3} Level {{i}}\":{spaces}ScarletMaidenLocation(\"Progressive Skill Tree\", 1_{location.Item2}_{location.Item1}_00 + i) for i in range(1, {location.Item4})}},\n";
            }
            skillsCode += "\n    # Skill Tree (Insanity)\n";
            for (var i = 0; i < self.layoutRows; i++)
            {
                for (var j = 0; j < self.layoutColumns; j++)
                {
                    skillsCode += $"    **{{f\"Skill Tree [{i},{j}] Level {{i}}\":  ScarletMaidenLocation(\"Skill Tree Insanity\", 1_{j}_{i}_00 + i) for i in range(1, 99)}},\n";
                }
            }
            GUIUtility.systemCopyBuffer = skillsCode;
        }
    }
}
