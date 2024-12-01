using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using HarmonyLib;
using System.Reflection;


namespace ScorePercentage
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static string PluginName => "ScorePercentage";
        internal static Harmony harmony;

        public static Logger log { get; private set; }

        [Init]
        public void Init(Logger logger, Config cfgProvider)
        {
            log = logger;
            PluginConfig.Instance = cfgProvider.Generated<PluginConfig>();
            harmony = new Harmony("com.Idlebob.BeatSaber.ScorePercentage");
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchSelf();
        }
    }
}
