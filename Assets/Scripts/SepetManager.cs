using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SepetManager : MonoBehaviour
{
    public GameObject content; // Sepetteki �r�nlerin listelendi�i alan
    public GameObject productRowPrefab; // �r�n sat�rlar�n� temsil eden Prefab

    public TextMeshProUGUI adText; // �r�n ad� input
    public TextMeshProUGUI fiyatText;
    public TextMeshProUGUI adetText;

    public TextMeshProUGUI totalPriceText; // Toplam fiyat�n g�sterildi�i TextMeshPro
    public TextMeshProUGUI totalSepet;
    private float totalPrice = 0; // Toplam fiyat

    public TextMeshProUGUI ToplamText;
    public TextMeshProUGUI KargoText;

    public TextMeshProUGUI BakiyeText;
    public TextMeshProUGUI KalanText;
    public TextMeshProUGUI MevcutBakiyeText;

    private TextMeshProUGUI siparisFiyatText;

    public void AddToCart()
    {
        int urunFiyat = int.Parse(fiyatText.text);
        int urunAdet = int.Parse(adetText.text);
        int urunToplamFiyat = urunAdet * urunFiyat;
        float KargoFiyat = float.Parse(KargoText.text);
        float Bakiye = float.Parse(BakiyeText.text);
        float ToplamFiyat;
        float YeniBakiye;

        totalPrice = float.Parse(totalPriceText.text);
        totalPrice += urunToplamFiyat;
        totalPriceText.text = totalPrice.ToString();
        totalSepet.text = totalPrice.ToString();
        ToplamFiyat = (KargoFiyat + totalPrice);
        ToplamText.text = ToplamFiyat.ToString();
        MevcutBakiyeText.text = Bakiye.ToString();
        YeniBakiye = Bakiye - ToplamFiyat;
        KalanText.text = YeniBakiye.ToString();
        adetText.text = "1";
    }

    // Sepete �r�n ekleme fonksiyonu
    public void SepeteUrunEkle()
    {
        // Input alanlar�ndaki de�erleri al
        string urunAdi = adText.text;
        int urunFiyat = int.Parse(fiyatText.text);
        int urunAdet = int.Parse(adetText.text);
        int urunToplamFiyat = urunAdet * urunFiyat;

        // Sepette ayn� �r�n var m� kontrol et
        bool urunVarMi = false;
        foreach (Transform child in content.transform)
        {
            TextMeshProUGUI[] textComponents = child.GetComponentsInChildren<TextMeshProUGUI>();
            string mevcutUrunAdi = "";
            TextMeshProUGUI adetTextComponent = null;

            // Prefab i�indeki bile�enleri kontrol et
            foreach (TextMeshProUGUI textComp in textComponents)
            {
                if (textComp.name == "�r�nAd_Text")
                {
                    mevcutUrunAdi = textComp.text;
                }
                else if (textComp.name == "AdetSay�s�")
                {
                    adetTextComponent = textComp;
                }
            }

            // �r�n ad� ayn�ysa, adet bilgisini g�ncelle
            if (mevcutUrunAdi == urunAdi)
            {
                urunVarMi = true;

                // Mevcut adet de�erini al ve g�ncelle
                int mevcutAdet = int.Parse(adetTextComponent.text);
                int yeniAdet = mevcutAdet + urunAdet;
                adetTextComponent.text = yeniAdet.ToString();

                // �r�n�n toplam fiyat�n� g�ncelle
                foreach (TextMeshProUGUI textComp in textComponents)
                {
                    if (textComp.name == "�r�nToplamFiyat_Text")
                    {
                        int yeniToplamFiyat = yeniAdet * urunFiyat;
                        textComp.text = yeniToplamFiyat.ToString("F2");
                    }
                }
                break;
            }
        }

        // E�er ayn� �r�n yoksa yeni bir sat�r olu�tur
        if (!urunVarMi)
        {
            // Prefab'dan yeni bir �r�n sat�r� olu�tur
            GameObject yeniSatir = Instantiate(productRowPrefab, content.transform);

            // Yeni sat�r�n i�indeki TextMeshProUGUI bile�enlerini bul ve atamalar yap
            TextMeshProUGUI[] textComponents = yeniSatir.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI textComp in textComponents)
            {
                if (textComp.name == "�r�nAd_Text")
                {
                    textComp.text = urunAdi;
                }
                else if (textComp.name == "AdetSay�s�")
                {
                    textComp.text = urunAdet.ToString();
                }
                else if (textComp.name == "�r�nFiyat_Text")
                {
                    textComp.text = urunFiyat.ToString("F2");
                }
                else if (textComp.name == "�r�nToplamFiyat_Text")
                {
                    textComp.text = urunToplamFiyat.ToString("F2");
                }
            }
        }
    }
    public void Bakiye ()
    {
        siparisFiyatText = GameObject.Find("Siparis_Text").GetComponent<TextMeshProUGUI>();

        if (float.Parse(KalanText.text) >= 0 && float.Parse(siparisFiyatText.text) >0)
        {
            float Bakiye = float.Parse(BakiyeText.text);

            float ToplamFiyat = float.Parse(ToplamText.text);

            float newBakiye = Bakiye - ToplamFiyat;

            BakiyeText.text = newBakiye.ToString();

            MevcutBakiyeText.text = newBakiye.ToString();
            KalanText.text = (newBakiye - ToplamFiyat).ToString();
        }             
    }
}

