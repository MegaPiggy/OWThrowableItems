using HarmonyLib;

namespace YeetMod
{
    [HarmonyPatch]
    public class Patches
    {
        public static ScreenPrompt YeetPrompt = new(InputLibrary.interact, "<CMD> " + "<color=orange>(x2) (Hold) </color> " + "Throw Item");

        [HarmonyPostfix] 
        [HarmonyPatch(typeof(PlayerTool), nameof(PlayerTool.EquipTool))]
        public static void PlayerTool_EquipTool_PostFix(PlayerTool __instance)
        {
            if (__instance is ItemTool) Locator.GetPromptManager().AddScreenPrompt(YeetPrompt, PromptPosition.LowerLeft, OWInput.IsInputMode(InputMode.Character));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OWItem), nameof(OWItem.DropItem))]
        public static void OWItem_DropItem_PostFix()
        {
            Locator.GetPromptManager().RemoveScreenPrompt(YeetPrompt);
        }
    }
}
