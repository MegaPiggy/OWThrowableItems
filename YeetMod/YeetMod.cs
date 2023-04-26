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
        private ScreenPrompt yeetPrompt;


        private void Awake() => Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        private void Start()
        {
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene is not (OWScene.SolarSystem or OWScene.EyeOfTheUniverse)) return;
                ModHelper.Events.Unity.FireOnNextUpdate(() =>
                {
                    yeetPrompt = new(InputLibrary.interact, "<CMD> " + "<color=orange>(x2) (Hold) </color> " + "Throw Item");
                    Locator.GetPromptManager().AddScreenPrompt(yeetPrompt, PromptPosition.LowerLeft);
                });
            };
        }

        private void Update()
        {
            if (Locator.GetPromptManager() == null) return;
            yeetPrompt.SetVisibility(false);

            if (!OWInput.IsInputMode(InputMode.Character)) return;
            if (Locator.GetToolModeSwapper()?.GetToolMode() != ToolMode.Item) return;
            yeetPrompt.SetVisibility(true);

            if (OWInput.IsNewlyPressed(InputLibrary.interact))
            {
                if (Time.time - lastButtonPressTime <= doublePressTimeLimit) isDoublePressing = true;
                lastButtonPressTime = Time.time;
            }
            else if (OWInput.IsNewlyReleased(InputLibrary.interact) && isDoublePressing)
            {
                isDoublePressing = false;
                Yeet(Time.time - lastButtonPressTime);
            }

            if (CheckForObstructed()) yeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.GrayedOut);
            else if (isDoublePressing) yeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.Attention);
            else yeetPrompt.SetDisplayState(ScreenPrompt.DisplayState.Normal);
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
