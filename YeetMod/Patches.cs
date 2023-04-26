using HarmonyLib;

namespace YeetMod
{
    [HarmonyPatch]
    public class Patches
    {
        // a bit silly

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.LocateSceneObjects))]
        public static void Locator_LocateSceneObjects_Postfix()
        {
            if (LoadManager.GetCurrentScene() != OWScene.SolarSystem) return;
            Locator.GetPromptManager().AddScreenPrompt(YeetMod.YeetPrompt, PromptPosition.LowerLeft);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Locator), nameof(Locator.ClearReferences))]
        public static void Locator_ClearReferences_Prefix()
        {
            if (LoadManager.GetCurrentScene() != OWScene.SolarSystem) return;
            Locator.GetPromptManager().RemoveScreenPrompt(YeetMod.YeetPrompt);
        }
    }
}
