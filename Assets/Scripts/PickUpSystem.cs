using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpSystem : MonoBehaviour
{
    public float raycastRange = 10.0f; // Raycast mesafesi
    public Transform holdPoint;
    public Camera playerCamera;
    public TextMeshProUGUI interactionText; // UI Text için referans

    private GameObject pickedObject = null; // Þu anda tutulan kutu
    private Animation boxAnimation; // Kutunun Animation bileþeni
    private Vector3 originalScale; // Nesnenin orijinal ölçeði
    private List<GameObject> itemsInBox = new List<GameObject>(); // Kutunun içindeki nesneler

    private RaycastHit hit;

    void Update()
    {
        if (pickedObject == null)
        {
            // Raycast yap ve etkileþim göstergesi
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, raycastRange))
            {
                GameObject targetObject = hit.collider.transform.root.gameObject;

                // Pickup etiketi kontrolü
                if (targetObject.CompareTag("Pickup"))
                {
                    interactionText.enabled = true; // Etkileþim mesajýný göster
                    interactionText.text = "F: Nesneyi Al";

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        Debug.Log("F tuþuna basýldý. Bir nesne alýnmaya çalýþýlýyor.");
                        TryPickupObject(targetObject);
                    }
                }
                else
                {
                    interactionText.enabled = false; // Mesajý gizle
                }
            }
            else
            {
                interactionText.enabled = false; // Raycast hiçbir þey bulamazsa mesajý gizle
            }
        }
        else
        {
            // Kutuyu býrakma ve animasyon
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F tuþuna basýldý. Tutulan nesne býrakýlýyor.");
                DropObject();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                boxAnimation.Play();
            }

            // Raflara nesne yerleþtirme
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("K tuþuna basýldý. Raflara nesne yerleþtiriliyor.");
                TryPlaceItemsOnShelf();
            }
        }
    }

    void TryPickupObject(GameObject targetObject)
    {
        Debug.Log("Pickup etiketi olan bir nesne bulundu: " + targetObject.name);

        originalScale = targetObject.transform.localScale;
        pickedObject = targetObject; // Kutuyu kaydet
        boxAnimation = pickedObject.GetComponent<Animation>();
        pickedObject.GetComponent<Rigidbody>().isKinematic = true; // Fizik hareketini durdur
        pickedObject.GetComponent<Rigidbody>().useGravity = false;
        pickedObject.transform.SetParent(holdPoint); // Kutuyu iliþkilendir
        pickedObject.transform.position = holdPoint.transform.position;
        pickedObject.transform.rotation = holdPoint.transform.rotation;

        // Kutunun içindeki nesneleri listeye ekle
        Transform itemsParent = pickedObject.transform.Find("Content");
        if (itemsParent != null)
        {
            itemsInBox.Clear(); // Önceki kutudan gelen nesneleri sýfýrla
            foreach (Transform child in itemsParent)
            {
                itemsInBox.Add(child.gameObject); // Yeni kutunun içeriðini listeye ekle
            }
        }

        Physics.IgnoreCollision(pickedObject.GetComponent<Collider>(), GetComponent<Collider>());
        Debug.Log("Nesne alýndý ve holdPoint'e yerleþtirildi.");
        interactionText.enabled = false;
    }

    void DropObject()
    {
        if (pickedObject != null)
        {
            Debug.Log("Nesne býrakýlýyor: " + pickedObject.name);

            Physics.IgnoreCollision(pickedObject.GetComponent<Collider>(), GetComponent<Collider>(), false);

            pickedObject.transform.parent = null; // Kutunun elden ayrýlmasý
            pickedObject.GetComponent<Rigidbody>().isKinematic = false; // Fizik tekrar aktif
            pickedObject.GetComponent<Rigidbody>().useGravity = true;
            pickedObject.transform.localScale = originalScale; // Orijinal ölçeði geri yükle
            pickedObject = null; // Þu anda tutulan kutu yok

            // Kutuyu býraktýðýnýzda listeyi sýfýrlayýn
            itemsInBox.Clear();

            Debug.Log("Nesne baþarýyla býrakýldý.");
        }
        else
        {
            Debug.Log("Býrakýlacak bir nesne yok.");
        }
    }

    void TryPlaceItemsOnShelf()
    {
        GameObject item = itemsInBox.Count > 0 ? itemsInBox[0] : null;

        if (item != null)
        {
            string itemTag = item.tag;

            int layerToCheck = 0;
            LayerMask raycastMask = 0;

            if (itemTag == "CableBox")
            {
                layerToCheck = LayerMask.NameToLayer("cableboxLayer");
            }
            else if (itemTag == "HeadPhone")
            {
                layerToCheck = LayerMask.NameToLayer("headphoneLayer");
            }
            else if (itemTag == "Speaker")
            {
                layerToCheck = LayerMask.NameToLayer("speakerLayer");
            }
            else if (itemTag == "MobilPhone")
            {
                layerToCheck = LayerMask.NameToLayer("mobilphoneLayer");
            }
            else if (itemTag == "Gamepad")
            {
                layerToCheck = LayerMask.NameToLayer("gamepadLayer");
            }
            else if (itemTag == "CoffeMachine")
            {
                layerToCheck = LayerMask.NameToLayer("coffemachineLayer");
            }
            else
            {
                Debug.LogWarning($"Bilinmeyen ürün tag'ý: {itemTag}");
                return;
            }

            raycastMask = 1 << layerToCheck;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, raycastRange, raycastMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.layer == layerToCheck)
                {
                    if (CheckOtherLayerOccupied(hitObject, layerToCheck))
                    {
                        Debug.Log("Diðer layer dolu. Ürün yerleþtirilemez.");
                        return;
                    }

                    foreach (Transform slot in hitObject.transform)
                    {
                        if (slot.childCount == 0)
                        {
                            PlaceItemInSlot(item, slot);
                            return;
                        }
                    }

                    Debug.Log($"Rafta uygun boþ slot bulunamadý. Layer: {LayerMask.LayerToName(layerToCheck)}");
                }
            }
            else
            {
                Debug.Log("Raycast uygun layer'da bir rafa çarpmadý.");
            }
        }
        else
        {
            Debug.Log("Kutuda yerleþtirilecek nesne kalmadý.");
        }
    }

    bool CheckOtherLayerOccupied(GameObject hitObject, int currentLayer)
    {
        int cableboxLayer = LayerMask.NameToLayer("cableboxLayer");
        int headphoneLayer = LayerMask.NameToLayer("headphoneLayer");
        int speakerLayer = LayerMask.NameToLayer("speakerLayer");
        int mobilephoneLayer = LayerMask.NameToLayer("mobilephoneLayer");
        int gamepadLayer = LayerMask.NameToLayer("gamepadLayer");
        int coffemachineLayer = LayerMask.NameToLayer("coffemachineLayer");

        List<int> otherLayers = new List<int> { cableboxLayer, headphoneLayer, speakerLayer, mobilephoneLayer, gamepadLayer, coffemachineLayer };
        otherLayers.Remove(currentLayer); // Kendi layer'ýný listeden çýkar

        foreach (int otherLayer in otherLayers)
        {
            LayerMask otherLayerMask = 1 << otherLayer;

            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, raycastRange, otherLayerMask))
            {
                GameObject hitObject2 = hit.collider.gameObject;
                foreach (Transform slot in hitObject2.transform)
                {
                    if (slot.gameObject.layer == otherLayer && slot.childCount > 0)
                    {
                        Debug.Log($"Diðer layer'daki slot dolu: {slot.name}, Layer: {LayerMask.LayerToName(slot.gameObject.layer)}");
                        return true;
                    }
                }
            }
        }

        Debug.Log("Diðer layer'larda boþ slot bulundu.");
        return false;
    }

    void PlaceItemInSlot(GameObject item, Transform slot)
    {
        // Nesneyi yeni pozisyona taþý (world space'te)
        item.transform.position = slot.position;
        item.transform.rotation = slot.rotation;

        // Nesneyi slot'un child'ý yap
        item.transform.SetParent(slot);

        // Kutudan çýkar
        itemsInBox.RemoveAt(0);

        Debug.Log($"{item.name} nesnesi {slot.name} pozisyonuna yerleþtirildi.");
    }
}

    
