using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CounterManager : MonoBehaviour
{
    public TextMeshProUGUI targetText; // Baðlanacak TextMeshPro nesnesi
    private int maxValue = 10; // Maksimum deðer
    private int minValue = 1;   // Minimum deðer

    public TextMeshProUGUI fiyatText;
    public TextMeshProUGUI toplamText;

    private TextMeshProUGUI siparisFiyat;
    private TextMeshProUGUI sepetFiyatText;
    private TextMeshProUGUI kargoFiyat;
    private TextMeshProUGUI Toplam_Text;
    private TextMeshProUGUI MevcutBakiyeText;
    //private TextMeshProUGUI ToplamText;
    private TextMeshProUGUI KalanText;



    void Start()
    {
        sepetFiyatText = GameObject.Find("SepetFiyat_Text").GetComponent<TextMeshProUGUI>();
        siparisFiyat = GameObject.Find("Siparis_Text").GetComponent<TextMeshProUGUI>();
        kargoFiyat = GameObject.Find("Kargo_Text").GetComponent<TextMeshProUGUI>();
        Toplam_Text = GameObject.Find("Toplam_Text").GetComponent<TextMeshProUGUI>();
        MevcutBakiyeText = GameObject.Find("MevcutBakiye_Text").GetComponent<TextMeshProUGUI>();
       // ToplamText = GameObject.Find("Toplam_Text").GetComponent<TextMeshProUGUI>();
        KalanText = GameObject.Find("Kalan_Text").GetComponent<TextMeshProUGUI>();
    }

    public void Increase()
    {
        // Mevcut deðerini al ve kontrol et
        int currentValue = int.Parse(targetText.text);

        if (currentValue < maxValue) // Eðer mevcut deðer maxValue'dan küçükse artýr
        {
            currentValue++;
            targetText.text = currentValue.ToString();
        }
    }

    public void Decrease()
    {
        // Mevcut deðerini al ve kontrol et
        int currentValue = int.Parse(targetText.text);

        if (currentValue > minValue) // Eðer mevcut deðer minValue'dan büyükse azalt
        {
            currentValue--;
            targetText.text = currentValue.ToString();
        }

    }

    public void TotalPrice()
    {
        int currentValue = int.Parse(targetText.text);
        float fiyat = float.Parse(fiyatText.text);

        targetText.text = currentValue.ToString();
        toplamText.text = (fiyat * currentValue).ToString("F2");

    }

    public void SepetTotalNegative()
    {
       
        int adet = int.Parse(targetText.text);
        
        if (adet > minValue) {
            float kargo = float.Parse(kargoFiyat.text);
            float fiyat = float.Parse(fiyatText.text);
            float SepetTotal = float.Parse(sepetFiyatText.text);
            float MevcutBakiye = float.Parse(MevcutBakiyeText.text);          
            float YeniSepet = SepetTotal -= fiyat;
            float Toplam = YeniSepet + kargo;
            float KalanFiyat = MevcutBakiye - Toplam;
            sepetFiyatText.text = YeniSepet.ToString(); // Ondalýklý format
            siparisFiyat.text = YeniSepet.ToString();
            Toplam_Text.text = Toplam.ToString();
            KalanText.text = KalanFiyat.ToString();
        }
    }
    public void SepetTotalPlus()
    {
        int adet = int.Parse(targetText.text);

        if (adet < maxValue) {
            float kargo = float.Parse(kargoFiyat.text);
            float fiyat = float.Parse(fiyatText.text);
            float SepetTotal = float.Parse(sepetFiyatText.text);
            float MevcutBakiye = float.Parse(MevcutBakiyeText.text);
            float YeniSepet = SepetTotal += fiyat;
            float Toplam = YeniSepet + kargo;
            float KalanFiyat = MevcutBakiye - Toplam;
            sepetFiyatText.text = YeniSepet.ToString(); // Ondalýklý format
            siparisFiyat.text = YeniSepet.ToString();
            Toplam_Text.text = Toplam.ToString();
            KalanText.text = KalanFiyat.ToString();
        }
    }

    public void RemoveItem(GameObject item)
    {
        // Ýlgili GameObject'i yok et
        Destroy(item);
        float SepetTotal = float.Parse(sepetFiyatText.text);
        float toplamFiyat = float.Parse(toplamText.text);
        float KalanFiyat = float.Parse(KalanText.text);
        sepetFiyatText.text = (SepetTotal - toplamFiyat).ToString();
        siparisFiyat.text = (SepetTotal - toplamFiyat).ToString();
        KalanText.text = (KalanFiyat + toplamFiyat).ToString();
    }

    void Update()
    {

    }
}
