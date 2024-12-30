using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CartItem : MonoBehaviour
{

    public TextMeshProUGUI itemNameText; // Prefabýn adýný gösterecek Text
    public TextMeshProUGUI itemCountText; // Adet bilgisini gösterecek Text

    public string ItemName => itemNameText.text; // Prefabýn adý
    public int ItemCount => int.Parse(itemCountText.text); // Adet sayýsý









    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
