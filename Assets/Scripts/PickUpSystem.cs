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
        // Kutudaki ilk �r�n� al
        GameObject item = itemsInBox.Count > 0 ? itemsInBox[0] : null;

        if (item != null)
        {
            string itemTag = item.tag;

            // Item tag'ine g�re raycast yap�lacak layer mask olu�tur
            int layerToCheck = 0;
            LayerMask raycastMask = 0;

            // Nesnenin tag'ine g�re raycast yap�lacak layer'� belirleyelim
            if (itemTag == "CableBox")
            {
                layerToCheck = LayerMask.NameToLayer("cableboxLayer");
                raycastMask = 1 << layerToCheck; // Sadece CableBox layer'�na raycast g�nder
            }
            else if (itemTag == "HeadPhone")
            {
                layerToCheck = LayerMask.NameToLayer("headphoneLayer");
                raycastMask = 1 << layerToCheck; // Sadece HeadPhone layer'�na raycast g�nder
            }
            else
            {
                Debug.LogWarning($"Bilinmeyen �r�n tag'�: {itemTag}");
                return;
            }

            // Kamera merkezinden ray g�nder
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, raycastRange, raycastMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log($"Raycast �arpt�: {hitObject.name}, Pozisyon: {hit.point}, Layer: {LayerMask.LayerToName(hitObject.layer)}");

                // �arp�lan nesneye ait layer kontrol ediliyor
                if (hitObject.layer == layerToCheck)
                {
                    Debug.Log("Hedef layer'a �arpt�, di�er layer'� kontrol ediyorum...");

                    // Di�er layer'daki slotlar� kontrol et (ba�ka bir layer'da bir �ey yerle�tirilmi�se, engelle)
                    if (CheckOtherLayerOccupied(hitObject, layerToCheck))
                    {
                        Debug.LogWarning("Di�er layer dolu. �r�n yerle�tirilemez.");
                        return; // Di�er layer doluysa, i�lem sonlan�r
                    }

                    Debug.Log("Di�er layer bo�, kendi layer'da slot aramaya devam ediyorum...");
                   
                    // Hedef layer'daki bo� slotlar� kontrol et
                    foreach (Transform slot in hitObject.transform)
                    {
                        if (slot.childCount == 0) // E�er slot bo�sa
                        {
                            PlaceItemInSlot(item, slot);
                            return; // �r�n ba�ar�yla yerle�tirildi, i�lem sonland�r�l�r
                        }
                        else
                        {
                            Debug.Log($"Slot {slot.name} dolu, ba�ka bir bo� slot ar�yorum.");
                        }
                    }

                    Debug.Log($"Rafta uygun bo� slot bulunamad�. Layer: {LayerMask.LayerToName(layerToCheck)}");
                }
                else
                {
                    Debug.Log("Raycast uygun layer'da bir slot bulamad�.");
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
        // Di�er layer'� belirle
        int otherLayer = (currentLayer == LayerMask.NameToLayer("cableboxLayer"))
            ? LayerMask.NameToLayer("headphoneLayer")
            : LayerMask.NameToLayer("cableboxLayer");

        LayerMask otherLayerMask = 1 << otherLayer;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, raycastRange, otherLayerMask))
        {
            Debug.Log($"Di�er layer ({LayerMask.LayerToName(otherLayer)})'� kontrol ediyorum...");

            GameObject hitObject2 = hit.collider.gameObject;


            // HitObject i�indeki t�m slotlar� kontrol et
            foreach (Transform slot in hitObject2.transform)
            {
                if (slot.gameObject.layer == otherLayer) // E�er slot di�er layer'daysa
                {
                    Debug.Log($"Slot {slot.name}, di�er layer'da bulunuyor: {LayerMask.LayerToName(otherLayer)}");

                    if (slot.childCount > 0) // Ve slot doluysa
                    {
                        Debug.Log($"Di�er layer'daki slot dolu: {slot.name}, Layer: {LayerMask.LayerToName(slot.gameObject.layer)}");
                        return true; // Di�er layer dolu, �r�n yerle�tirilemez
                    }
                }
            }


           
        }
        Debug.Log("Di�er layer'da bo� slot bulundu.");
        return false; // Di�er layer bo�
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

    
