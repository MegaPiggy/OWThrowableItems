using OWML.ModHelper;
using UnityEngine;

namespace YeetMod
{
    public class YeetMod : ModBehaviour
    {
        private float doublePressTimeLimit = 0.5f;
        private float lastButtonPressTime = float.NegativeInfinity;
        private bool isDoublePressing;


        private void Update()
        {
            if (OWInput.IsNewlyPressed(InputLibrary.interact, InputMode.Character)) 
            {
                if (Time.time - lastButtonPressTime <= doublePressTimeLimit) isDoublePressing = true;
                lastButtonPressTime = Time.time;
            }
            if (OWInput.IsNewlyReleased(InputLibrary.interact, InputMode.Character) && isDoublePressing)
            {
                isDoublePressing = false;
                Yeet(Time.time - lastButtonPressTime);
            }
        }

        private void Yeet(float heldButtonTime)
        {
            if (Locator.GetToolModeSwapper().GetToolMode() != ToolMode.Item) return;

            var yeetSpeed = heldButtonTime <= 0.25f ? 0 : Mathf.Clamp(heldButtonTime * 20, 0, 50);
            var playerTransform = Locator.GetPlayerTransform();
            var itemTool = Locator.GetToolModeSwapper().GetItemCarryTool();
            var socket = ItemYeetSocket.Create(itemTool.GetHeldItem(), playerTransform.position + playerTransform.up * 0.5f + playerTransform.forward * 2, yeetSpeed);
            itemTool.DropItemInstantly(null, socket.gameObject.transform);
        }
    }
}