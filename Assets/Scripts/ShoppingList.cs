using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShoppingList : MonoBehaviour
{
    public TextMeshProUGUI totalPriceText; // Toplam fiyatýn gösterildiði TextMeshPro
    public TextMeshProUGUI targetText; //   Adet TextMeshPro
    public float itemPrice; // Ürün fiyatý
    public int itemQuantity; // Ürün adedi
    public float totalPrice = 0; // Toplam fiyat

    void Start()
    {
        
    }

    public void AddToCart()
    {
     
        totalPrice = float.Parse(totalPriceText.text);
        totalPrice += itemQuantity * itemPrice;
        totalPriceText.text = totalPrice.ToString();
        targetText.text = "1";
    }


    void Update()
    {
        itemQuantity = int.Parse(targetText.text);
    }
}
