using UnityEngine;

namespace YeetMod
{
    public class ItemYeetSocket : MonoBehaviour
    {
        private OWRigidbody owRigidbody;
        private GameObject detectorObj = new("YeetDetector");
        private OWItem attachedItem;
        private bool switchedFromInteractible;
        private float initialVelocity;


        public static ItemYeetSocket Create(OWItem item, Vector3 startingPosition, float startingVelocity)
        {
            var socketObj = new GameObject("ItemYeetBody");
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
            detectorObj.layer = LayerMask.NameToLayer("PhysicalDetector");
            detectorObj.tag = "DynamicPropDetector";

            detectorObj.transform.SetParent(transform, false);
            owRigidbody = gameObject.AddComponent<OWRigidbody>();
            AddShapesAndColliders();
            detectorObj.AddComponent<DynamicForceDetector>();
            detectorObj.AddComponent<DynamicFluidDetector>();
            detectorObj.transform.SetParent(attachedItem.transform, false);

            attachedItem.onPickedUp += OnPickUpItem;

            foreach (var volume in Locator.GetPlayerBody()._attachedForceDetector._activeVolumes) owRigidbody._attachedForceDetector.AddVolume(volume);
            foreach (var volume in Locator.GetPlayerBody()._attachedFluidDetector._activeVolumes) owRigidbody._attachedFluidDetector.AddVolume(volume);
            owRigidbody._attachedFluidDetector._buoyancy = Locator.GetProbe().GetOWRigidbody()._attachedFluidDetector._buoyancy;
            owRigidbody._attachedFluidDetector._splashEffects = Locator.GetProbe().GetOWRigidbody()._attachedFluidDetector._splashEffects;

            owRigidbody._rigidbody.angularDrag = 5;
            owRigidbody.SetMass(0.001f);
            owRigidbody.SetVelocity(Locator.GetPlayerBody().GetPointVelocity(transform.position) + Locator.GetPlayerCamera().transform.forward * initialVelocity);
        }

        private void AddShapesAndColliders()
        {
            switch (attachedItem._type)
            {
                case ItemType.Scroll:
                    var scrollShape = detectorObj.AddComponent<CapsuleShape>();
                    var scrollCollider = detectorObj.AddComponent<CapsuleCollider>();
                    scrollCollider.height = 0.8f;
                    scrollCollider.radius = 0.2f;
                    scrollShape.CopySettingsFromCollider();
                    scrollShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    OrientDetector(new Vector3(0, 0, -0.37f), Quaternion.Euler(90, 0, 0));
                    break;
                case ItemType.WarpCore:
                    var warpCore = attachedItem as WarpCoreItem;
                    if (warpCore._warpCoreType is WarpCoreType.Vessel or WarpCoreType.VesselBroken)
                    {
                        var awcShape = detectorObj.AddComponent<BoxShape>();
                        var awcCollider = detectorObj.AddComponent<BoxCollider>();
                        awcCollider.size = new Vector3(0.35f, 1.15f, 0.35f);
                        awcShape.CopySettingsFromCollider();
                        awcShape.SetCollisionMode(Shape.CollisionMode.Detector);
                        OrientDetector(Vector3.zero, Quaternion.Euler(0, 45, 0));
                    }
                    else
                    {
                        var wcShape = detectorObj.AddComponent<BoxShape>();
                        var wcCollider = detectorObj.AddComponent<BoxCollider>();
                        wcCollider.size = new Vector3(0.4f, 0.1f, 0.4f);
                        wcShape.CopySettingsFromCollider();
                        wcShape.SetCollisionMode(Shape.CollisionMode.Detector);
                        OrientDetector(new Vector3(0, 10.04f, 0), Quaternion.Euler(45, 0, 90));
                    }
                    break;
                case ItemType.SharedStone:
                    var projStoneShape = detectorObj.AddComponent<BoxShape>();
                    var projStoneCollider = detectorObj.AddComponent<BoxCollider>();
                    projStoneCollider.size = new Vector3(0.4f, 0.1f, 0.4f);
                    projStoneShape.CopySettingsFromCollider();
                    projStoneShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    break;
                case ItemType.ConversationStone:
                    var convStoneShape = detectorObj.AddComponent<SphereShape>();
                    var convStoneCollider = detectorObj.AddComponent<SphereCollider>();
                    convStoneCollider.radius = 0.24f;
                    convStoneShape.CopySettingsFromCollider();
                    convStoneShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    break;
                case ItemType.Lantern:
                    var lanternCapsuleShape = detectorObj.AddComponent<CapsuleShape>();
                    var lanternBoxShape = detectorObj.AddComponent<BoxShape>();
                    var lanternCapsuleCollider = detectorObj.AddComponent<CapsuleCollider>();
                    var lanternBoxCollider = detectorObj.AddComponent<BoxCollider>();
                    lanternCapsuleCollider.center = new Vector3(0, 0.35f, 0);
                    lanternCapsuleCollider.height = 0.4f;
                    lanternCapsuleCollider.radius = 0.2f;
                    lanternBoxCollider.size = new Vector3(0.25f, 0.1f, 0.25f);
                    lanternCapsuleShape.CopySettingsFromCollider();
                    lanternCapsuleShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    lanternBoxShape.CopySettingsFromCollider();
                    lanternBoxShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    break;
                case ItemType.SlideReel:
                    var reelShape = detectorObj.AddComponent<CapsuleShape>();
                    var reelCollider = detectorObj.AddComponent<CapsuleCollider>();
                    reelCollider.center = new Vector3(0, 0.2f, 0);
                    reelCollider.height = 0.37f;
                    reelCollider.radius = 0.4f;
                    reelShape.CopySettingsFromCollider();
                    reelShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    break;
                case ItemType.DreamLantern:
                    var dreamLanternCapsuleShape = detectorObj.AddComponent<CapsuleShape>();
                    var dreamLanternBoxShape = detectorObj.AddComponent<BoxShape>();
                    var dreamLanternCapsuleCollider = detectorObj.AddComponent<CapsuleCollider>();
                    var dreamLanternBoxCollider = detectorObj.AddComponent<BoxCollider>();
                    dreamLanternCapsuleCollider.center = new Vector3(0.35f, 0, 0);
                    dreamLanternCapsuleCollider.height = 0.4f;
                    dreamLanternCapsuleCollider.radius = 0.37f;
                    dreamLanternBoxCollider.size = new Vector3(0.15f, 0.37f, 0.37f);
                    dreamLanternCapsuleShape.CopySettingsFromCollider();
                    dreamLanternCapsuleShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    dreamLanternBoxShape.CopySettingsFromCollider();
                    dreamLanternBoxShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    OrientDetector(Vector3.zero, Quaternion.Euler(0, 0, 90));
                    break;
                case ItemType.VisionTorch:
                    var torchCapsuleShape = detectorObj.AddComponent<CapsuleShape>();
                    var torchSphereShape = detectorObj.AddComponent<SphereShape>();
                    var torchCapsuleCollider = detectorObj.AddComponent<CapsuleCollider>();
                    var torchSphereCollider = detectorObj.AddComponent<SphereCollider>();
                    torchCapsuleCollider.height = 1.8f;
                    torchCapsuleCollider.radius = 0.1f;
                    torchSphereCollider.center = new Vector3(0, 0.9f, 0.05f);
                    torchSphereCollider.radius = 0.4f;
                    torchCapsuleShape.CopySettingsFromCollider();
                    torchCapsuleShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    torchSphereShape.CopySettingsFromCollider();
                    torchSphereShape.SetCollisionMode(Shape.CollisionMode.Detector);
                    break;
                default:
                    detectorObj.layer = LayerMask.NameToLayer("AdvancedDetector");
                    switchedFromInteractible = attachedItem.gameObject.layer == LayerMask.NameToLayer("Interactible");
                    if (switchedFromInteractible) attachedItem.gameObject.layer = LayerMask.NameToLayer("Default");
                    detectorObj.AddComponent<SphereShape>().SetCollisionMode(Shape.CollisionMode.Detector);
                    detectorObj.AddComponent<SphereCollider>();
                    break;
            }
        }

        private void OrientDetector(Vector3 localPos, Quaternion localRot)
        {
            detectorObj.transform.localPosition = localPos;
            detectorObj.transform.localRotation = localRot;
        }

        private void OnPickUpItem(OWItem item)
        {
            attachedItem.onPickedUp -= OnPickUpItem;
            if (switchedFromInteractible) attachedItem.gameObject.layer = LayerMask.NameToLayer("Interactible");
            Destroy(gameObject);
            Destroy(detectorObj);
        }
    }
}
