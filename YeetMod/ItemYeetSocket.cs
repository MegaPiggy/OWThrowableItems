using System.Collections.Generic;
using UnityEngine;

namespace YeetMod
{
    public class ItemYeetSocket : MonoBehaviour
    {
        private OWItem attachedItem;
        private bool wasOnInteractible;

        private OWRigidbody oWRigidbody;
        private float initialVelocity = 0;


        public static ItemYeetSocket Create(OWItem item, Vector3 startingPosition, float startingVelocity)
        {
            var socketObj = new GameObject("ItemYeetBody");
            socketObj.transform.position = startingPosition;
            var newSocket = socketObj.AddComponent<ItemYeetSocket>();
            newSocket.attachedItem = item;
            newSocket.initialVelocity = startingVelocity;
            return newSocket;
        }

        private void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("AdvancedDetector");
            gameObject.tag = "DynamicPropDetector";

            oWRigidbody = gameObject.AddComponent<OWRigidbody>();
            gameObject.AddComponent<SphereCollider>();
            gameObject.AddComponent<DynamicForceDetector>();
            gameObject.AddComponent<DynamicFluidDetector>();
        }

        private void Start()
        {
            wasOnInteractible = attachedItem.gameObject.layer == LayerMask.NameToLayer("Interactible");
            if (wasOnInteractible) attachedItem.gameObject.layer = LayerMask.NameToLayer("Default");
            attachedItem.onPickedUp += OnPickUpItem;

            oWRigidbody._childColliders = gameObject.GetComponentsInChildren<Collider>();
            oWRigidbody._attachedForceDetector._activeVolumes = new List<EffectVolume>(Locator.GetPlayerBody()._attachedForceDetector._activeVolumes);
            oWRigidbody._attachedFluidDetector._activeVolumes = new List<EffectVolume>(Locator.GetPlayerBody()._attachedFluidDetector._activeVolumes);
            oWRigidbody._attachedFluidDetector._buoyancy = Locator.GetPlayerBody()._attachedFluidDetector._buoyancy;

            oWRigidbody._rigidbody.angularDrag = 5;
            oWRigidbody.SetMass(0.001f);
            oWRigidbody.SetVelocity(Locator.GetPlayerBody().GetPointVelocity(gameObject.transform.position) + Locator.GetPlayerCamera().gameObject.transform.forward * initialVelocity);
        }

        private void OnPickUpItem(OWItem item)
        {
            attachedItem.onPickedUp -= OnPickUpItem;
            if (wasOnInteractible) attachedItem.gameObject.layer = LayerMask.NameToLayer("Interactible");
            Destroy(gameObject);
        }
    }
}
