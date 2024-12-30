using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfInteraction : MonoBehaviour
{
    public float raycastRange = 5.0f; // Raycast mesafesi
    public Transform holdPoint; // Kutunun ta��nd��� nokta
    public Camera playerCamera; // Oyuncunun kameras�
    public KeyCode placeItemsKey = KeyCode.K; // Nesneleri yerle�tirme tu�u

    private GameObject pickedBox = null; // Tutulan kutu
    private List<GameObject> itemsInBox = new List<GameObject>(); // Kutunun i�indeki nesneler
    private RaycastHit hit;

    void Update()
    {
        if (pickedBox != null)
        {
            // Raflara nesne yerle�tirme
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

                // Kutunun i�indeki nesneleri listeye ekle
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

                Debug.Log("Kutu al�nd� ve nesneler listeye eklendi.");
            }
        }
    }

    void TryPlaceItemsOnShelf()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            GameObject targetObject = hit.collider.gameObject;

            // Raf�n shelfRow1 child'�na eri�im
            Transform shelfRow = targetObject.transform.Find("ShelfRow1");
            if (shelfRow != null)
            {
                foreach (Transform slot in shelfRow)
                {
                    if (slot.childCount == 0) // Bo� bir pozisyon bul
                    {
                        if (itemsInBox.Count > 0) // Kutuda nesne varsa
                        {
                            GameObject item = itemsInBox[0];
                            itemsInBox.RemoveAt(0); // Kutudan ��kar

                            // Nesneyi yeni pozisyona ta��
                            item.transform.SetParent(slot);
                            item.transform.position = slot.position;
                            item.transform.rotation = slot.rotation;

                            Debug.Log($"{item.name} raf�n pozisyonuna yerle�tirildi.");
                        }
                        else
                        {
                            Debug.Log("Kutuda yerle�tirilecek nesne kalmad�.");
                        }

                        break; // Sadece bir nesneyi yerle�tir
                    }
                }
            }
            else
            {
                Debug.Log("Raycast �arpan raf�n ShelfRow1 child'� yok.");
            }
        }
        else
        {
            Debug.Log("Raycast hi�bir rafa �arpmad�.");
        }
    }
}
