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

        public static readonly ScreenPrompt YeetPrompt = new(InputLibrary.interact, "<CMD> " + "<color=orange>(x2) (Hold) </color> " + "Throw Item");


        private void Awake() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        private void Update()
        {
            // yes its unoptimized. in practice it doesnt matter
            YeetPrompt.SetVisibility(false);
            if (Locator.GetToolModeSwapper()?.GetToolMode() != ToolMode.Item) return;
            YeetPrompt.SetVisibility(true);

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

            if (CheckForObstructed()) YeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.GrayedOut);
            else if (isDoublePressing) YeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.Attention);
            else YeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.Normal);
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
