using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CartItem : MonoBehaviour
{

    public TextMeshProUGUI itemNameText; // Prefab�n ad�n� g�sterecek Text
    public TextMeshProUGUI itemCountText; // Adet bilgisini g�sterecek Text

    public string ItemName => itemNameText.text; // Prefab�n ad�
    public int ItemCount => int.Parse(itemCountText.text); // Adet say�s�









    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}