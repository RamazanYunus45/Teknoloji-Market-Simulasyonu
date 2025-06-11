using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UrunBilgi : MonoBehaviour
{
    public string urunAdi;  
    public float fiyat;     

    private bool isScanned = false;

    private TextMeshProUGUI PlaceUrunAdText;
    private TextMeshProUGUI PlaceUrunFiyatText;

    private void Start()
    {
        PlaceUrunAdText = GameObject.Find("PlaceÜrünAd_Text").GetComponent<TextMeshProUGUI>();
        PlaceUrunFiyatText = GameObject.Find("PlaceÜrünFiyat_Text").GetComponent<TextMeshProUGUI>();
    }
    public void Scan()
    {
        if (isScanned) return; 
        isScanned = true;
        
        Debug.Log("Okutulan ürün: " + urunAdi + " - " + fiyat + "");

        PlaceUrunAdText.text = urunAdi;
        PlaceUrunFiyatText.text = fiyat.ToString(); ;

        StartCoroutine(MoveAndDestroy());
    }

    IEnumerator MoveAndDestroy()
    {
        Vector3 targetPos = transform.position + Vector3.left * 2.5f; 

        float time = 0;
        float duration = 0.5f;
        Vector3 startPos = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        Destroy(gameObject);
    }
}
