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
        // Kutudaki ilk ürünü al
        GameObject item = itemsInBox.Count > 0 ? itemsInBox[0] : null;

        if (item != null)
        {
            string itemTag = item.tag;

            // Item tag'ine göre raycast yapýlacak layer mask oluþtur
            int layerToCheck = 0;
            LayerMask raycastMask = 0;

            // Nesnenin tag'ine göre raycast yapýlacak layer'ý belirleyelim
            if (itemTag == "CableBox")
            {
                layerToCheck = LayerMask.NameToLayer("cableboxLayer");
                raycastMask = 1 << layerToCheck; // Sadece CableBox layer'ýna raycast gönder
            }
            else if (itemTag == "HeadPhone")
            {
                layerToCheck = LayerMask.NameToLayer("headphoneLayer");
                raycastMask = 1 << layerToCheck; // Sadece HeadPhone layer'ýna raycast gönder
            }
            else
            {
                Debug.LogWarning($"Bilinmeyen ürün tag'ý: {itemTag}");
                return;
            }

            // Kamera merkezinden ray gönder
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, raycastRange, raycastMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log($"Raycast çarptý: {hitObject.name}, Pozisyon: {hit.point}, Layer: {LayerMask.LayerToName(hitObject.layer)}");

                // Çarpýlan nesneye ait layer kontrol ediliyor
                if (hitObject.layer == layerToCheck)
                {
                    Debug.Log("Hedef layer'a çarptý, diðer layer'ý kontrol ediyorum...");

                    // Diðer layer'daki slotlarý kontrol et (baþka bir layer'da bir þey yerleþtirilmiþse, engelle)
                    if (CheckOtherLayerOccupied(hitObject, layerToCheck))
                    {
                        Debug.LogWarning("Diðer layer dolu. Ürün yerleþtirilemez.");
                        return; // Diðer layer doluysa, iþlem sonlanýr
                    }

                    Debug.Log("Diðer layer boþ, kendi layer'da slot aramaya devam ediyorum...");
                   
                    // Hedef layer'daki boþ slotlarý kontrol et
                    foreach (Transform slot in hitObject.transform)
                    {
                        if (slot.childCount == 0) // Eðer slot boþsa
                        {
                            PlaceItemInSlot(item, slot);
                            return; // Ürün baþarýyla yerleþtirildi, iþlem sonlandýrýlýr
                        }
                        else
                        {
                            Debug.Log($"Slot {slot.name} dolu, baþka bir boþ slot arýyorum.");
                        }
                    }

                    Debug.Log($"Rafta uygun boþ slot bulunamadý. Layer: {LayerMask.LayerToName(layerToCheck)}");
                }
                else
                {
                    Debug.Log("Raycast uygun layer'da bir slot bulamadý.");
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
        // Diðer layer'ý belirle
        int otherLayer = (currentLayer == LayerMask.NameToLayer("cableboxLayer"))
            ? LayerMask.NameToLayer("headphoneLayer")
            : LayerMask.NameToLayer("cableboxLayer");

        LayerMask otherLayerMask = 1 << otherLayer;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, raycastRange, otherLayerMask))
        {
            Debug.Log($"Diðer layer ({LayerMask.LayerToName(otherLayer)})'ý kontrol ediyorum...");

            GameObject hitObject2 = hit.collider.gameObject;


            // HitObject içindeki tüm slotlarý kontrol et
            foreach (Transform slot in hitObject2.transform)
            {
                if (slot.gameObject.layer == otherLayer) // Eðer slot diðer layer'daysa
                {
                    Debug.Log($"Slot {slot.name}, diðer layer'da bulunuyor: {LayerMask.LayerToName(otherLayer)}");

                    if (slot.childCount > 0) // Ve slot doluysa
                    {
                        Debug.Log($"Diðer layer'daki slot dolu: {slot.name}, Layer: {LayerMask.LayerToName(slot.gameObject.layer)}");
                        return true; // Diðer layer dolu, ürün yerleþtirilemez
                    }
                }
            }


           
        }
        Debug.Log("Diðer layer'da boþ slot bulundu.");
        return false; // Diðer layer boþ
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

    
