using UnityEngine;

namespace YeetMod
{
    public class ItemYeetSocket : MonoBehaviour
    {
        private OWItem attachedItem;
        private float initialVelocity;

        private OWRigidbody owRigidbody;
        private bool wasOnInteractible;


        public static ItemYeetSocket Create(OWItem item, Vector3 startingPosition, float startingVelocity)
        {
            var socketObj = new GameObject($"{item.name}_Body");
            socketObj.SetActive(false);
            socketObj.transform.position = startingPosition;
            var socket = socketObj.AddComponent<ItemYeetSocket>();
            socket.attachedItem = item;
            socket.initialVelocity = startingVelocity;
            socketObj.SetActive(true);
            return socket;
        }

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("AdvancedDetector");
            gameObject.tag = "DynamicPropDetector";

            owRigidbody = gameObject.AddComponent<OWRigidbody>();
            gameObject.AddComponent<SphereCollider>();
            gameObject.AddComponent<SphereShape>()._collisionMode = Shape.CollisionMode.Detector;
            gameObject.AddComponent<DynamicForceDetector>();
            gameObject.AddComponent<DynamicFluidDetector>();

            wasOnInteractible = attachedItem.gameObject.layer == LayerMask.NameToLayer("Interactible");
            if (wasOnInteractible) attachedItem.gameObject.layer = LayerMask.NameToLayer("Default");
            attachedItem.onPickedUp += OnPickUpItem;

            foreach (var volume in Locator.GetPlayerBody()._attachedForceDetector._activeVolumes)
            {
                owRigidbody._attachedForceDetector.AddVolume(volume);
                owRigidbody._attachedFluidDetector.AddVolume(volume);
            }
            owRigidbody._attachedFluidDetector._buoyancy = Locator.GetProbe().GetOWRigidbody()._attachedFluidDetector._buoyancy;
            owRigidbody._attachedFluidDetector._splashEffects = Locator.GetProbe().GetOWRigidbody()._attachedFluidDetector._splashEffects;

            owRigidbody._rigidbody.angularDrag = 5;
            owRigidbody.SetMass(0.001f);
            owRigidbody.SetVelocity(Locator.GetPlayerBody().GetPointVelocity(transform.position) + Locator.GetPlayerCamera().transform.forward * initialVelocity);
        }

        private void OnPickUpItem(OWItem item)
        {
            attachedItem.onPickedUp -= OnPickUpItem;
            if (wasOnInteractible) attachedItem.gameObject.layer = LayerMask.NameToLayer("Interactible");
            Destroy(gameObject);
        }
    }
}
