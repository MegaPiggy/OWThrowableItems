using OWML.ModHelper;
using UnityEngine;

namespace YeetMod
{
    public class YeetMod : ModBehaviour
    {
        private static ScreenPrompt yeetPrompt;
        // not const so theyre editable in UE
        private static float
            doublePressTimeLimit = 0.5f,
            itemDropTimeLimit = 0.25f,
            yeetSpeedIncreaseRate = 20,
            yeetSpeedLimit = 50;
        private float lastButtonPressTime = float.NegativeInfinity;
        private bool isDoublePressing;


        private void Start()
        {
            yeetPrompt = new(InputLibrary.interact, "<CMD> " + "<color=orange>(x2) (Hold) </color> " + "Throw Item");

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene is OWScene.SolarSystem or OWScene.EyeOfTheUniverse) FindObjectOfType<PromptManager>().AddScreenPrompt(yeetPrompt, PromptPosition.LowerLeft);
            };
        }

        private void Update()
        {
            if (Locator.GetPromptManager() == null) return;

            yeetPrompt.SetVisibility(OWInput.IsInputMode(InputMode.Character) && Locator.GetToolModeSwapper().GetToolMode() == ToolMode.Item);
            if (yeetPrompt.IsVisible())
            {
                yeetPrompt.SetDisplayState(
                    CheckForObstructed() ? ScreenPrompt.DisplayState.GrayedOut :
                    isDoublePressing ? ScreenPrompt.DisplayState.Attention :
                    ScreenPrompt.DisplayState.Normal
                );
            }
            else
            {
                // reset everything so we dont accidentally count it if we arent holding an item
                lastButtonPressTime = float.NegativeInfinity;
                isDoublePressing = false;
            }

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
        }

        private bool CheckForObstructed() => Physics.SphereCast(new Ray(Locator.GetPlayerCamera().transform.position, Locator.GetPlayerCamera().transform.forward), 0.5f, 2, OWLayerMask.physicalMask);

        private static void Yeet(float heldButtonTime)
        {
            //this is directly tied to the check result and we set display state first so this works fine as a return condition to prevent dropping items through walls
            if (yeetPrompt.IsDisplayState(ScreenPrompt.DisplayState.GrayedOut)) return;

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
