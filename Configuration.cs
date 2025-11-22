using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;

namespace ZeldaScraps.Configuration
{
    public class PluginConfig
    {
        public ConfigEntry<int> MegaFlashlightStorePrice;
        public ConfigEntry<int> DancingCactusSpawnWeight;
        public ConfigEntry<int> PlainBaubleSpawnWeight;
        public ConfigEntry<int> FacetedbaubleSpawnWeight;
        public ConfigEntry<int> FancyBaubleSpawnWeight;
        public ConfigEntry<int> RareBaubleChance;
        // For more info on custom configs, see https://lethal.wiki/dev/intermediate/custom-configs

        public PluginConfig(ConfigFile cfg)
        {
            DancingCactusSpawnWeight = cfg.Bind("Spawn Weights", "Dancing Cactus", 20, "The spawn weight for the dancing cactus, higher value means the item will spawn more often");
            PlainBaubleSpawnWeight = cfg.Bind("Spawn Weights", "Plain Christmas Bauble", 20, "The spawn weight for the plain christmas bauble, higher value means the item will spawn more often");
            FacetedbaubleSpawnWeight = cfg.Bind("Spawn Weights", "Faceted Christmas Bauble", 20, "The spawn weight for the faceted christmas bauble, higher value means the item will spawn more often");
            FancyBaubleSpawnWeight = cfg.Bind("Spawn Weights", "Fancy Christmas Bauble", 20, "The spawn weight for the fancy christmas bauble, higher value means the item will spawn more often");
            MegaFlashlightStorePrice = cfg.Bind("Store Items", "10k lumen flashlight", 75, "The price of the 10k lumen flashlight in the store");
            RareBaubleChance = cfg.Bind("Christmas Items", "Rare Bauble Chance", 15, new ConfigDescription("How likely it is for a bauble to spawn as 'rare', quadrupling its value", new AcceptableValueRange<int>(0, 100)));
            ClearUnusedEntries(cfg);
        }

        private void ClearUnusedEntries(ConfigFile cfg)
        {
            // Normally, old unused config entries don't get removed, so we do it with this piece of code. Credit to Kittenji.
            PropertyInfo orphanedEntriesProp = cfg.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg, null);
            orphanedEntries.Clear(); // Clear orphaned entries (Unbinded/Abandoned entries)
            cfg.Save(); // Save the config file to save these changes
        }
    }
}
