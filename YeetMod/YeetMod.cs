using OWML.ModHelper;
using UnityEngine;

namespace YeetMod
{
    public class YeetMod : ModBehaviour
    {
        private const float doublePressTimeLimit = 0.5f;
        private float lastButtonPressTime = float.NegativeInfinity;
        private bool isDoublePressing;


        private void Update()
        {
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

        private static void Yeet(float heldButtonTime)
        {
            if (Locator.GetToolModeSwapper().GetToolMode() != ToolMode.Item) return;

            var yeetSpeed = heldButtonTime <= 0.25f ? 0 : Mathf.Clamp(heldButtonTime * 20, 0, 50);
            var playerCameraTransform = Locator.GetPlayerCamera().transform;
            var itemTool = Locator.GetToolModeSwapper().GetItemCarryTool();
            var socket = ItemYeetSocket.Create(itemTool.GetHeldItem(), playerCameraTransform.position + playerCameraTransform.forward * 2, yeetSpeed);
            itemTool.DropItemInstantly(null, socket.transform);
        }
    }
}
