using HarmonyLib;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace YeetMod
{
    public class YeetMod : ModBehaviour
    {
        // not const so theyre editable in UE
        private static float
            doublePressTimeLimit = 0.5f,
            itemDropTimeLimit = 0.25f,
            yeetSpeedIncreaseRate = 20,
            yeetSpeedLimit = 50;
        private float lastButtonPressTime = float.NegativeInfinity;
        private bool isDoublePressing;


        private void Awake() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        private void Update()
        {
            if (Locator.GetToolModeSwapper()?.GetToolMode() != ToolMode.Item) return;

            if (OWInput.IsNewlyPressed(InputLibrary.interact, InputMode.Character))
            {
                if (Time.time - lastButtonPressTime <= doublePressTimeLimit) isDoublePressing = true;
                lastButtonPressTime = Time.time;
            }
            else if (OWInput.IsNewlyReleased(InputLibrary.interact, InputMode.Character) && isDoublePressing)
            {
                isDoublePressing = false;
                Yeet(Time.time - lastButtonPressTime);
            }

            if (CheckForObstructed() && !Patches.YeetPrompt.IsDisplayState(ScreenPrompt.DisplayState.GrayedOut)) Patches.YeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.GrayedOut);
            else if (isDoublePressing && !Patches.YeetPrompt.IsDisplayState(ScreenPrompt.DisplayState.Attention)) Patches.YeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.Attention);
            else if (!Patches.YeetPrompt.IsDisplayState(ScreenPrompt.DisplayState.Normal)) Patches.YeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.Normal);
        }

        private bool CheckForObstructed()
        {
            //something with ray- or spherecast, whatever goes here has to be made to possibly actually prevent a throw too but this is basically a placeholder at this point
            return false;
        }

        private static void Yeet(float heldButtonTime)
        {
            var itemTool = Locator.GetToolModeSwapper().GetItemCarryTool();
            if (itemTool.GetHeldItem().IsAnimationPlaying()) return;

            var yeetSpeed = heldButtonTime <= itemDropTimeLimit ? 0 : Mathf.Clamp(heldButtonTime * yeetSpeedIncreaseRate, 0, yeetSpeedLimit);
            var playerCameraTransform = Locator.GetPlayerCamera().transform;
            var socket = ItemYeetSocket.Create(itemTool.GetHeldItem(), playerCameraTransform.position + playerCameraTransform.forward * 2, yeetSpeed);
            socket.transform.rotation = playerCameraTransform.rotation;
            itemTool.DropItemInstantly(null, socket.transform);

            Locator.GetToolModeSwapper().UnequipTool();
        }
    }
}
