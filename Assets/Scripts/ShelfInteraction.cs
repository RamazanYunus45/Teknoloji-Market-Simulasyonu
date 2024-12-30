using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfInteraction : MonoBehaviour
{
    public float raycastRange = 5.0f; // Raycast mesafesi
    public Transform holdPoint; // Kutunun taþýndýðý nokta
    public Camera playerCamera; // Oyuncunun kamerasý
    public KeyCode placeItemsKey = KeyCode.K; // Nesneleri yerleþtirme tuþu

    private GameObject pickedBox = null; // Tutulan kutu
    private List<GameObject> itemsInBox = new List<GameObject>(); // Kutunun içindeki nesneler
    private RaycastHit hit;

    void Update()
    {
        if (pickedBox != null)
        {
            // Raflara nesne yerleþtirme
            if (Input.GetKeyDown(placeItemsKey))
            {
                TryPlaceItemsOnShelf();
            }
        }
        else
        {
            // Kutuyu alma
            if (Input.GetKeyDown(KeyCode.F))
            {
                TryPickupBox();
            }
        }
    }

    void TryPickupBox()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            GameObject targetObject = hit.collider.transform.root.gameObject;

            if (targetObject.CompareTag("Pickup"))
            {
                pickedBox = targetObject;

                // Kutunun içindeki nesneleri listeye ekle
                Transform itemsParent = pickedBox.transform.Find("Items");
                if (itemsParent != null)
                {
                    foreach (Transform child in itemsParent)
                    {
                        itemsInBox.Add(child.gameObject);
                    }
                }

                pickedBox.transform.SetParent(holdPoint);
                pickedBox.transform.position = holdPoint.position;
                pickedBox.GetComponent<Rigidbody>().isKinematic = true;
                pickedBox.GetComponent<Rigidbody>().useGravity = false;

                Debug.Log("Kutu alýndý ve nesneler listeye eklendi.");
            }
        }
    }

    void TryPlaceItemsOnShelf()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            GameObject targetObject = hit.collider.gameObject;

            // Rafýn shelfRow1 child'ýna eriþim
            Transform shelfRow = targetObject.transform.Find("ShelfRow1");
            if (shelfRow != null)
            {
                foreach (Transform slot in shelfRow)
                {
                    if (slot.childCount == 0) // Boþ bir pozisyon bul
                    {
                        if (itemsInBox.Count > 0) // Kutuda nesne varsa
                        {
                            GameObject item = itemsInBox[0];
                            itemsInBox.RemoveAt(0); // Kutudan çýkar

                            // Nesneyi yeni pozisyona taþý
                            item.transform.SetParent(slot);
                            item.transform.position = slot.position;
                            item.transform.rotation = slot.rotation;

                            Debug.Log($"{item.name} rafýn pozisyonuna yerleþtirildi.");
                        }
                        else
                        {
                            Debug.Log("Kutuda yerleþtirilecek nesne kalmadý.");
                        }

                        break; // Sadece bir nesneyi yerleþtir
                    }
                }
            }
            else
            {
                Debug.Log("Raycast çarpan rafýn ShelfRow1 child'ý yok.");
            }
        }
        else
        {
            Debug.Log("Raycast hiçbir rafa çarpmadý.");
        }
    }
}
