using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickUpSystem : MonoBehaviour
{
    public float raycastRange = 10.0f; // Raycast mesafesi
    public Transform holdPoint;
    public Camera playerCamera;
    public TextMeshProUGUI interactionText; // UI Text i�in referans

    private GameObject pickedObject = null; // �u anda tutulan kutu
    private Animation boxAnimation; // Kutunun Animation bile�eni
    private Vector3 originalScale; // Nesnenin orijinal �l�e�i
    private List<GameObject> itemsInBox = new List<GameObject>(); // Kutunun i�indeki nesneler

    private RaycastHit hit;

    void Update()
    {
        if (pickedObject == null)
        {
            // Raycast yap ve etkile�im g�stergesi
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, raycastRange))
            {
                GameObject targetObject = hit.collider.transform.root.gameObject;

                // Pickup etiketi kontrol�
                if (targetObject.CompareTag("Pickup"))
                {
                    interactionText.enabled = true; // Etkile�im mesaj�n� g�ster
                    interactionText.text = "F: Nesneyi Al";

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        Debug.Log("F tu�una bas�ld�. Bir nesne al�nmaya �al���l�yor.");
                        TryPickupObject(targetObject);
                    }
                }
                else
                {
                    interactionText.enabled = false; // Mesaj� gizle
                }
            }
            else
            {
                interactionText.enabled = false; // Raycast hi�bir �ey bulamazsa mesaj� gizle
            }
        }
        else
        {
            // Kutuyu b�rakma ve animasyon
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F tu�una bas�ld�. Tutulan nesne b�rak�l�yor.");
                DropObject();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                boxAnimation.Play();
            }

            // Raflara nesne yerle�tirme
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("K tu�una bas�ld�. Raflara nesne yerle�tiriliyor.");
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
        pickedObject.transform.SetParent(holdPoint); // Kutuyu ili�kilendir
        pickedObject.transform.position = holdPoint.transform.position;
        pickedObject.transform.rotation = holdPoint.transform.rotation;

        // Kutunun i�indeki nesneleri listeye ekle
        Transform itemsParent = pickedObject.transform.Find("Content");
        if (itemsParent != null)
        {
            itemsInBox.Clear(); // �nceki kutudan gelen nesneleri s�f�rla
            foreach (Transform child in itemsParent)
            {
                itemsInBox.Add(child.gameObject); // Yeni kutunun i�eri�ini listeye ekle
            }
        }

        Physics.IgnoreCollision(pickedObject.GetComponent<Collider>(), GetComponent<Collider>());
        Debug.Log("Nesne al�nd� ve holdPoint'e yerle�tirildi.");
        interactionText.enabled = false;
    }

    void DropObject()
    {
        if (pickedObject != null)
        {
            Debug.Log("Nesne b�rak�l�yor: " + pickedObject.name);

            Physics.IgnoreCollision(pickedObject.GetComponent<Collider>(), GetComponent<Collider>(), false);

            pickedObject.transform.parent = null; // Kutunun elden ayr�lmas�
            pickedObject.GetComponent<Rigidbody>().isKinematic = false; // Fizik tekrar aktif
            pickedObject.GetComponent<Rigidbody>().useGravity = true;
            pickedObject.transform.localScale = originalScale; // Orijinal �l�e�i geri y�kle
            pickedObject = null; // �u anda tutulan kutu yok

            // Kutuyu b�rakt���n�zda listeyi s�f�rlay�n
            itemsInBox.Clear();

            Debug.Log("Nesne ba�ar�yla b�rak�ld�.");
        }
        else
        {
            Debug.Log("B�rak�lacak bir nesne yok.");
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
                Debug.LogWarning($"Bilinmeyen �r�n tag'�: {itemTag}");
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
                        Debug.Log("Di�er layer dolu. �r�n yerle�tirilemez.");
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

                    Debug.Log($"Rafta uygun bo� slot bulunamad�. Layer: {LayerMask.LayerToName(layerToCheck)}");
                }
            }
            else
            {
                Debug.Log("Raycast uygun layer'da bir rafa �arpmad�.");
            }
        }
        else
        {
            Debug.Log("Kutuda yerle�tirilecek nesne kalmad�.");
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
        otherLayers.Remove(currentLayer); // Kendi layer'�n� listeden ��kar

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
                        Debug.Log($"Di�er layer'daki slot dolu: {slot.name}, Layer: {LayerMask.LayerToName(slot.gameObject.layer)}");
                        return true;
                    }
                }
            }
        }

        Debug.Log("Di�er layer'larda bo� slot bulundu.");
        return false;
    }

    void PlaceItemInSlot(GameObject item, Transform slot)
    {
        // Nesneyi yeni pozisyona ta�� (world space'te)
        item.transform.position = slot.position;
        item.transform.rotation = slot.rotation;

        // Nesneyi slot'un child'� yap
        item.transform.SetParent(slot);

        // Kutudan ��kar
        itemsInBox.RemoveAt(0);

        Debug.Log($"{item.name} nesnesi {slot.name} pozisyonuna yerle�tirildi.");
    }
}

    
