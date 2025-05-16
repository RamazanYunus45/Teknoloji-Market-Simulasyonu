using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public Transform cartContainer; // View Layout Group'un Transform'u

    public GameObject prefab1; // Prefablar
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;
    public GameObject prefab5;
    public GameObject prefab6;

    private float xPos = -122f;
    private float yPos = 5f;
    private float zPos = -22.5f;
    private float zPos2 = -25.5f;

    private Dictionary<string, GameObject> prefabDictionary;

    private TextMeshProUGUI KalanText;

    private void Start()
    {
        Debug.Log("CartManager ba�lat�ld�. Prefab e�lemeleri yap�l�yor...");

        // Prefab e�lemelerini olu�turuyoruz
        prefabDictionary = new Dictionary<string, GameObject>
        {
            { "Telefon", prefab1 },
            { "Usb Kablo", prefab2 },
            { "Kahve Makinas�", prefab3 },
            { "Kulakl�k", prefab4 },
            { "Gamepad", prefab5 },
            { "Hoparl�r", prefab6 }
        };

        Debug.Log($"Toplam {prefabDictionary.Count} prefab e�lemesi yap�ld�.");

        
    }

    public void SpawnCartItems()
    {
        if (cartContainer == null)
        {
            Debug.LogError("cartContainer bulunamad�! L�tfen bir Transform atay�n.");
            return;
        }

        Debug.Log("SpawnCartItems fonksiyonu �a�r�ld�. CartContainer taran�yor...");

        foreach (Transform child in cartContainer)
        {
            Debug.Log($"CartContainer alt�ndaki child bulundu: {child.name}");

            CartItem cartItem = child.GetComponent<CartItem>();
            if (cartItem != null)
            {
                Debug.Log($"CartItem bile�eni bulundu. �r�n: {cartItem.ItemName}, Adet: {cartItem.ItemCount}");

                string itemName = cartItem.ItemName;
                int itemCount = cartItem.ItemCount;

                SpawnItems(itemName, itemCount);
            }
            else
            {
                Debug.LogWarning($"CartItem bile�eni child'da bulunamad�: {child.name}");
            }
        }
    }

    private void SpawnItems(string itemName, int count)
    {
        KalanText = GameObject.Find("Kalan_Text").GetComponent<TextMeshProUGUI>();

        float Kalan = float.Parse(KalanText.text);
        if(Kalan >= 0)
        {
            Debug.Log($"SpawnItems fonksiyonu �a�r�ld�. �r�n: {itemName}, Adet: {count}");

            if (prefabDictionary.TryGetValue(itemName, out GameObject prefabToSpawn))
            {
                Debug.Log($"Prefab bulundu: {itemName}. Spawn i�lemi ba�l�yor...");

                for (int i = 0; i < count; i++)
                {
                    float zValue = Random.Range(0, 2) == 0 ? zPos : zPos2;
                    Vector3 spawnPosition = new Vector3(xPos, yPos, zValue);
                    Instantiate(prefabToSpawn, spawnPosition, prefabToSpawn.transform.rotation);
                    Debug.Log($"Prefab sahneye eklendi: {prefabToSpawn.name}");
                    Debug.Log($"Prefab spawn edildi: {itemName}, Pozisyon: {spawnPosition}");

                    if (yPos < 7)
                    {
                        yPos += 2.0f;
                    }
                }
            }
            else
            {
                Debug.LogError($"Prefab bulunamad�: {itemName}");
            }
        }
    }
       
}
