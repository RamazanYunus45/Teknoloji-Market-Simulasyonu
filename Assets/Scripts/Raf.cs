using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf : MonoBehaviour
{



    /* Raftaki ürün bilgilerinin alýyor adet ve çeþit olarak , rastgele ürün ve adet seçiyor ,
     yeterli ürün yoksa diðer raflarý kontrol edip yeterli ürün olan rafýn konumunu döndürüyor */

    public Dictionary<string, int> urunBilgileri = new Dictionary<string, int>(); // Ürün adetleri
    public List<string> urunCesitleri = new List<string>(); // Ürün çeþitleri
    public static List<Raf> TumRaflar = new List<Raf>(); // Tüm raflarý takip etmek için

    // Seçilen ürünleri dýþarýdan eriþilebilir hale getirdik
    public Dictionary<string, int> secilenUrunler = new Dictionary<string, int>();

    private void Awake()
    {
        TumRaflar.Add(this); // Bu rafý listeye ekle
    }

    private void OnDestroy()
    {
        TumRaflar.Remove(this); // Silindiðinde listeden çýkar
    }

    public void UpdateUrunBilgileri()
    {
        urunBilgileri.Clear();
        urunCesitleri.Clear();

        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                foreach (Transform greatGrandChild in grandChild)
                {
                    string urunTag = greatGrandChild.tag;
                    if (!string.IsNullOrEmpty(urunTag))
                    {
                        if (!urunBilgileri.ContainsKey(urunTag))
                        {
                            urunBilgileri.Add(urunTag, 1);
                            urunCesitleri.Add(urunTag);
                        }
                        else
                        {
                            urunBilgileri[urunTag]++;
                        }
                    }
                }
            }
        }
    }

    public List<Transform> YeterliUrunBul(Raf kendiRafi, string urunAdi, int gerekliMiktar)
    {
        List<Transform> rafKonum = new List<Transform>(); // ShelfPoint konumlarýný saklamak için
        int kalanMiktar = gerekliMiktar;

        Debug.Log($"[YeterliUrunBul] {urunAdi} için {gerekliMiktar} adet aranýyor...");

        // Ýlk rafta ne kadar ürün vardý ve nerede saklayalým
        int ilkRafMevcutAdet = 0;
        List<Transform> ilkRafKonumlar = new List<Transform>();

        // Önce ilk raftaki bilgileri alalým
        kendiRafi.UpdateUrunBilgileri();
        if (kendiRafi.urunBilgileri.ContainsKey(urunAdi))
        {
            ilkRafMevcutAdet = kendiRafi.urunBilgileri[urunAdi];
            Debug.Log($"[YeterliUrunBul] Ýlk rafta {urunAdi} mevcut: {ilkRafMevcutAdet} adet.");

            if (ilkRafMevcutAdet > 0)
            {
                foreach (Transform child in kendiRafi.transform)
                {
                    if (child.CompareTag("ShelfPoint"))
                    {
                        ilkRafKonumlar.Add(child);
                    }
                }
            }
        }

        kalanMiktar -= Mathf.Min(ilkRafMevcutAdet, kalanMiktar); // Ýlk raftan ne kadar alabildikse çýkar

        // Þimdi diðer raflarý kontrol et
        foreach (Raf rafScript in TumRaflar)
        {
            if (rafScript != kendiRafi) // Kendini kontrol etme
            {
                rafScript.UpdateUrunBilgileri();
                Debug.Log($"[YeterliUrunBul] {rafScript.name} rafý kontrol ediliyor...");

                if (rafScript.urunBilgileri.ContainsKey(urunAdi))
                {
                    int mevcutAdet = rafScript.urunBilgileri[urunAdi];
                    Debug.Log($"[YeterliUrunBul] {rafScript.name} rafýnda {urunAdi} mevcut: {mevcutAdet} adet.");

                    if (mevcutAdet > 0)
                    {
                        foreach (Transform child in rafScript.transform)
                        {
                            if (child.CompareTag("ShelfPoint"))
                            {
                                rafKonum.Add(child);
                            }
                        }

                        kalanMiktar -= mevcutAdet;

                        // Gerekli miktar tamamlandýysa çýk
                        if (kalanMiktar <= 0)
                        {
                            Debug.Log($"[YeterliUrunBul] {urunAdi} ürünü için diðer raflardan yeterli miktar alýndý.");
                            // Ýlk raftan da ürün aldýysak onu da ekle
                            rafKonum.AddRange(ilkRafKonumlar);
                            return rafKonum;
                        }
                    }
                }
            }
        }

        // Diðer raflar bitti  ne kadar toplayabildiysek onu döndür
        Debug.Log($"[YeterliUrunBul] Tüm raflardan toplanan ürün adedi yeterli deðil. Ne bulduysak alýyoruz.");

        // Ýlk raftan da ürün aldýysak onlarý da ekle
        rafKonum.AddRange(ilkRafKonumlar);

        return rafKonum;
    }

    public void RastgeleUrunSec()
    {
        secilenUrunler.Clear();  // Önceki seçimleri temizle

        if (urunCesitleri.Count == 0)
        {
            Debug.LogWarning("Ürün çeþitleri listesi boþ, seçim yapýlamadý!");
            return;
        }

        System.Random rand = new System.Random();
        int urunSecimAdedi = Mathf.Min(rand.Next(1, 3), urunCesitleri.Count); // 1 veya 2 çeþit ürün seç
        List<string> kopyaListe = new List<string>(urunCesitleri); // Ürün listesinin kopyasýný al

        for (int i = 0; i < urunSecimAdedi; i++)
        {
            if (kopyaListe.Count == 0) break;

            int rastgeleIndex = rand.Next(kopyaListe.Count);
            string secilenUrun = kopyaListe[rastgeleIndex];
            kopyaListe.RemoveAt(rastgeleIndex);

            int secilenAdet = rand.Next(1, 4); // 1-3 adet seç

            // Ürünü alýrken, YeterliUrunBul fonksiyonu ile tüm raflarda arama yapacaðýz
            List<Transform> rafKonumlar = YeterliUrunBul(this, secilenUrun, secilenAdet);

            if (rafKonumlar != null && rafKonumlar.Count > 0)
            {
                secilenUrunler[secilenUrun] = secilenAdet;
                Debug.Log($"{secilenUrun} ürünü {secilenAdet} adet baþarýyla seçildi.");
            }
            else
            {
                Debug.LogWarning($"{secilenUrun} ürünü için yeterli stok hiçbir rafta bulunamadý! Seçim iptal.");
            }
        }
    }


}


