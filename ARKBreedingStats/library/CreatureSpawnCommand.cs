﻿using System;
using System.Linq;
using System.Windows.Forms;
using ARKBreedingStats.Library;
using ARKBreedingStats.species;

namespace ARKBreedingStats.library
{
    /// <summary>
    /// Creates console commands to spawn creatures.
    /// </summary>
    public static class CreatureSpawnCommand
    {
        /// <summary>
        /// Creates a spawn command that works in the vanilla game, but that command can cause the game to crash if the result of this method is changed. Also the stat values and colors are only correct after cryoing the creature.
        /// </summary>
        public static void InstableCommandToClipboard(Creature cr)
        {
            // see https://ark.fandom.com/wiki/Console_commands#SpawnExactDino for this command in ARK. It's unstable and can crash the game if the format or data is not correct.
            var xp = 0; // TODO
            long arkIdInGame = cr.ArkIdImported ? cr.ArkId : 0;

            var spawnCommand = $"SpawnExactDino \"Blueprint'{cr.speciesBlueprint}'\" \"\" 1 {cr.LevelHatched} {cr.levelsDom.Sum()} "
                               + $"\"{GetLevelStringForExactSpawningCommand(cr.levelsWild)}\" \"{GetLevelStringForExactSpawningCommand(cr.levelsDom)}\" \"{cr.name}\" "
                               + $"0 {(cr.flags.HasFlag(CreatureFlags.Neutered) ? "1" : "0")} \"\" \"\" \"{cr.imprinterName}\" 0 {cr.imprintingBonus} "
                               + $"\"{(cr.colors == null ? string.Empty : string.Join(",", cr.colors))}\" {arkIdInGame} {xp} 0 20 20";



            var cheatPrefix = Properties.Settings.Default.AdminConsoleCommandWithCheat
                ? "cheat "
                : string.Empty;
            Clipboard.SetText(cheatPrefix + spawnCommand);
        }

        private static string GetLevelStringForExactSpawningCommand(int[] levels)
        {
            // stat order for this command is health, stamina, oxygen, food, weight, melee damage, movement speed, crafting skill
            return $"{levels[(int)StatNames.Health]},{levels[(int)StatNames.Stamina]},{levels[(int)StatNames.Oxygen]},{levels[(int)StatNames.Food]},{levels[(int)StatNames.Weight]},{levels[(int)StatNames.MeleeDamageMultiplier]},{levels[(int)StatNames.SpeedMultiplier]},{levels[(int)StatNames.CraftingSpeedMultiplier]}";
        }

        private static string GetLevelStringForExactSpawningCommandDS2(int[] wildlvl, int[] domlvl)
        {
            var statIndices = new[]
            {
                (int)StatNames.Health, (int)StatNames.Stamina, (int)StatNames.Oxygen, (int)StatNames.Food,
                (int)StatNames.Water, (int)StatNames.Weight, (int)StatNames.MeleeDamageMultiplier,
                (int)StatNames.SpeedMultiplier, (int)StatNames.CraftingSpeedMultiplier
            };
            string levelString = string.Empty;
            foreach (var si in statIndices)
                levelString += $"{wildlvl[si]}/{domlvl[si]} ";
            return levelString;
        }

        /// <summary>
        /// Creates a spawn command that works in the vanilla game, but that command can cause the game to crash if the result of this method is changed. Also the stat values and colors are only correct after cryoing the creature.
        /// </summary>
        public static void DinoStorageV2CommandToClipboard(Creature cr)
        {
            var spawnCommand = $"admincheat scriptcommand spawndino_ds {cr.speciesBlueprint} 0 0 50 0 {(cr.isDomesticated ? "1" : "0")} {(cr.sex == Sex.Female ? "1" : cr.sex == Sex.Unknown ? "?" : "0")} "
                               + $"{cr.Maturation} {cr.imprintingBonus} {(cr.flags.HasFlag(CreatureFlags.Neutered) ? "1" : "0")} 0 0 "
                               + GetLevelStringForExactSpawningCommandDS2(cr.levelsWild, cr.levelsDom)
                               + $"{(cr.colors == null ? "0 0 0 0 0 0" : string.Join(" ", cr.colors))}";

            Clipboard.SetText(spawnCommand);
        }
    }
}
