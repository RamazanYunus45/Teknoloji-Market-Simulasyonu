using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SepetManager : MonoBehaviour
{
    public GameObject content; // Sepetteki ürünlerin listelendiði alan
    public GameObject productRowPrefab; // Ürün satýrlarýný temsil eden Prefab

    public TextMeshProUGUI adText; // Ürün adý input
    public TextMeshProUGUI fiyatText;
    public TextMeshProUGUI adetText;

    public TextMeshProUGUI totalPriceText; // Toplam fiyatýn gösterildiði TextMeshPro
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

    // Sepete ürün ekleme fonksiyonu
    public void SepeteUrunEkle()
    {
        // Input alanlarýndaki deðerleri al
        string urunAdi = adText.text;
        int urunFiyat = int.Parse(fiyatText.text);
        int urunAdet = int.Parse(adetText.text);
        int urunToplamFiyat = urunAdet * urunFiyat;

        // Sepette ayný ürün var mý kontrol et
        bool urunVarMi = false;
        foreach (Transform child in content.transform)
        {
            TextMeshProUGUI[] textComponents = child.GetComponentsInChildren<TextMeshProUGUI>();
            string mevcutUrunAdi = "";
            TextMeshProUGUI adetTextComponent = null;

            // Prefab içindeki bileþenleri kontrol et
            foreach (TextMeshProUGUI textComp in textComponents)
            {
                if (textComp.name == "ÜrünAd_Text")
                {
                    mevcutUrunAdi = textComp.text;
                }
                else if (textComp.name == "AdetSayýsý")
                {
                    adetTextComponent = textComp;
                }
            }

            // Ürün adý aynýysa, adet bilgisini güncelle
            if (mevcutUrunAdi == urunAdi)
            {
                urunVarMi = true;

                // Mevcut adet deðerini al ve güncelle
                int mevcutAdet = int.Parse(adetTextComponent.text);
                int yeniAdet = mevcutAdet + urunAdet;
                adetTextComponent.text = yeniAdet.ToString();

                // Ürünün toplam fiyatýný güncelle
                foreach (TextMeshProUGUI textComp in textComponents)
                {
                    if (textComp.name == "ÜrünToplamFiyat_Text")
                    {
                        int yeniToplamFiyat = yeniAdet * urunFiyat;
                        textComp.text = yeniToplamFiyat.ToString("F2");
                    }
                }
                break;
            }
        }

        // Eðer ayný ürün yoksa yeni bir satýr oluþtur
        if (!urunVarMi)
        {
            // Prefab'dan yeni bir ürün satýrý oluþtur
            GameObject yeniSatir = Instantiate(productRowPrefab, content.transform);

            // Yeni satýrýn içindeki TextMeshProUGUI bileþenlerini bul ve atamalar yap
            TextMeshProUGUI[] textComponents = yeniSatir.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (TextMeshProUGUI textComp in textComponents)
            {
                if (textComp.name == "ÜrünAd_Text")
                {
                    textComp.text = urunAdi;
                }
                else if (textComp.name == "AdetSayýsý")
                {
                    textComp.text = urunAdet.ToString();
                }
                else if (textComp.name == "ÜrünFiyat_Text")
                {
                    textComp.text = urunFiyat.ToString("F2");
                }
                else if (textComp.name == "ÜrünToplamFiyat_Text")
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

