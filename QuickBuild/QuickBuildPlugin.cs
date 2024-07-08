using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EquinoxsDebuggingTools;
using EquinoxsModUtils;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace QuickBuild
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class QuickBuildPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.QuickBuild";
        private const string PluginName = "QuickBuild";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Objects & Variables

        // Config Entires

        public static ConfigEntry<KeyCode> showMenuKey;

        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            CreateConfigEntries();
            ApplyPatches();

            QuickBuildGUI.LoadImages();

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            if (!ModUtils.hasGameLoaded) return;

            QuickBuildGUI.sSinceClose += Time.deltaTime;
            QuickBuildGUI.sSinceClick += Time.deltaTime;

            if (!UnityInput.Current.GetKeyDown(showMenuKey.Value)) return;
            if (ShouldShowMenu()) {
                QuickBuildGUI.shouldShowGUI = true;
                ModUtils.FreeCursor(true);
            }
            else {
                QuickBuildGUI.CloseAndReset();
            }
        }

        private void OnGUI() {
            if (QuickBuildGUI.shouldShowGUI) {
                QuickBuildGUI.Draw();
            }
        }

        // Public Functions

        public static void AddItem(string path, MenuItem item) {
            if (!QuickBuildGUI.categoryOptions.ContainsKey(path)) {
                Log.LogError($"Path '{path}' is not a valid path, please add it first");
                return;
            }

            QuickBuildGUI.categoryOptions[path].Add(item);
        }

        public static void InsertItem(string path, int index, MenuItem item) {
            if (!QuickBuildGUI.categoryOptions.ContainsKey(path)) {
                Log.LogError($"Path '{path}' is not a valid path, please add it first");
                return;
            }

            if(index < 0) {
                Log.LogError($"Index '{index}' cannot be < 0, you lemon");
                return;
            }

            int count = QuickBuildGUI.categoryOptions[path].Count;
            if(index >= count) {
                Log.LogWarning($"Index was >= than num options ({count}) for path '{path}'. Adding to end instead");
                AddItem(path, item);
            }

            QuickBuildGUI.categoryOptions[path].Insert(index, item);
        }

        // Private Functions

        private void CreateConfigEntries() {
            showMenuKey = Config.Bind("General", "Show Menu Key", KeyCode.BackQuote, new ConfigDescription("Hold this key to open the Quick Build menu"));
        }

        private void ApplyPatches() {

        }

        private bool ShouldShowMenu() {
            return !QuickBuildGUI.shouldShowGUI &&
                    QuickBuildGUI.sSinceClose > 0.2f &&
                    Player.instance.builder.CurrentConstructionMode == PlayerBuilder.ConstructionMode.Construction;
        }
    }
}
