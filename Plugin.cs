using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using ZeldaScraps.Configuration;

namespace ZeldaScraps
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class Plugin : BaseUnityPlugin
    {
        const string PLUGIN_GUID = "Zeldahu.ZeldaScraps";
        const string PLUGIN_NAME = "Stupid Collection of Random Articles for the Players";
        const string PLUGIN_VERSION = "1.0.0";
        internal static new ManualLogSource Logger = null!;
        internal static PluginConfig BoundConfig { get; private set; } = null!;
        public static AssetBundle? ModAssets;
        private readonly Harmony harmony = new Harmony(PLUGIN_GUID);
        private void Awake()
        {
            Logger = base.Logger;

            BoundConfig = new PluginConfig(base.Config);

            InitializeNetworkBehaviours();

            var bundleName = "zeldascraps";
            ModAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), bundleName));
            if (ModAssets == null)
            {
                Logger.LogError($"Failed to load custom assets.");
                return;
            }
            Item MegaFlashlight = ModAssets.LoadAsset<Item>("MegaFlashlight");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(MegaFlashlight.spawnPrefab);
            TerminalNode MegaFlashlightTerminalNode = ModAssets.LoadAsset<TerminalNode>("MFITerminalNode");
            LethalLib.Modules.Items.RegisterShopItem(MegaFlashlight, null, null, MegaFlashlightTerminalNode, Plugin.BoundConfig.MegaFlashlightStorePrice.Value);

            Item DancingCactus = ModAssets.LoadAsset<Item>("DancingCactus");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(DancingCactus.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(DancingCactus, Plugin.BoundConfig.DancingCactusSpawnWeight.Value, LethalLib.Modules.Levels.LevelTypes.All);

            Item PlainBauble = ModAssets.LoadAsset<Item>("PlainBauble");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(PlainBauble.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(PlainBauble, Plugin.BoundConfig.PlainBaubleSpawnWeight.Value, LethalLib.Modules.Levels.LevelTypes.All);

            Item FacetedBauble = ModAssets.LoadAsset<Item>("FacetedBauble");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(FacetedBauble.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(FacetedBauble, Plugin.BoundConfig.FacetedbaubleSpawnWeight.Value, LethalLib.Modules.Levels.LevelTypes.All);

            Item FancyBauble = ModAssets.LoadAsset<Item>("FancyBauble");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(FancyBauble.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(FancyBauble, Plugin.BoundConfig.FancyBaubleSpawnWeight.Value, LethalLib.Modules.Levels.LevelTypes.All);
            harmony.PatchAll();
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

        }
        private static void InitializeNetworkBehaviours()
        {
            IEnumerable<System.Type> types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
