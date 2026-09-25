using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace BetterHPBar
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class BetterHPBarBase : BaseUnityPlugin
    {
        public const string modGUID = "SpiralMods." + modName;
        private const string modName = "BetterHPBar";
        private const string modVersion = "1.0.3";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static BetterHPBarBase Instance;

        private ManualLogSource mls;

        public static BepInEx.Configuration.ConfigEntry<bool> showBossHP;
        public static BepInEx.Configuration.ConfigEntry<bool> healthPercent;

        void Awake()
        {
            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo(modName + " has loaded (ModVersion: " + modVersion + ", ModGUID: " + modGUID + ")!");

            createConfig();
            harmony.PatchAll(typeof(HUDElement));
        }

        void createConfig()
        {
            showBossHP = Config.Bind<bool>("Settings", "Show Boss HP", true, "Show/Hide Bosses' HP.");
            healthPercent = Config.Bind<bool>("Settings", "Health Percentage", false, "Makes it so the text shows a percentage instead.");
        }
    }
}
