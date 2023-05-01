using HarmonyLib;
using UnityEngine;

namespace YeetMod
{
    [HarmonyPatch]
    public class Patches
    {
        //another UE editable one
        private static float cowerSpeedMinimum = 5;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CowerAnimTriggerVolume), nameof(CowerAnimTriggerVolume.OnEntry))]
        public static void CowerAnimTriggerVolume_OnEntry_Postfix(CowerAnimTriggerVolume __instance, GameObject hitObj)
        {
            if (!hitObj.CompareTag("DynamicPropDetector")) return;

            var objectBody = hitObj.GetAttachedOWRigidbody();
            if (objectBody?.TryGetComponent(out ItemYeetSocket _) ?? false)
            {
                var relativeBody = __instance.GetAttachedOWRigidbody();
                var relativeVelocity = objectBody._currentVelocity - (relativeBody != null ? relativeBody.GetPointVelocity(objectBody.transform.position) : Vector3.zero);
                if (relativeVelocity.magnitude > cowerSpeedMinimum) __instance._animator.SetTrigger("ProbeDodge");
            }
        }
    }
}
