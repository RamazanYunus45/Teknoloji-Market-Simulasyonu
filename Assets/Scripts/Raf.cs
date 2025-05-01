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
        List<Transform> rafKonum = new List<Transform>(); // ShelfPoint konumlarýný saklamak için liste
        Debug.Log($"[YeterliUrunBul] {urunAdi} için {gerekliMiktar} adet aranýyor...");

        foreach (Raf rafScript in TumRaflar) // Tüm raflar üzerinde dön
        {
            if (rafScript != kendiRafi) // Kendi rafýný kontrol etme
            {
                rafScript.UpdateUrunBilgileri(); // Raf bilgilerini güncelle
                Debug.Log($"[YeterliUrunBul] {rafScript.name} rafý kontrol ediliyor...");

                // Rafta istenen ürün var mý ve yeterli mi kontrol et
                if (rafScript.urunBilgileri.ContainsKey(urunAdi))
                {
                    int mevcutAdet = rafScript.urunBilgileri[urunAdi];
                    Debug.Log($"[YeterliUrunBul] {rafScript.name} rafýnda {urunAdi} ürünü mevcut: {mevcutAdet} adet.");

                    if (mevcutAdet >= gerekliMiktar)
                    {
                        Debug.Log($"[YeterliUrunBul] {rafScript.name} rafýnda {gerekliMiktar} adet {urunAdi} bulundu!");

                        // Rafýn içindeki ShelfPoint'leri bul ve ekle
                        foreach (Transform child in rafScript.transform)
                        {
                            if (child.CompareTag("ShelfPoint"))
                            {
                                rafKonum.Add(child);
                            }
                        }

                        Debug.Log($"[YeterliUrunBul] {rafKonum.Count} ShelfPoint bulundu. Bu raf seçildi.");
                        return rafKonum; // Yeterli ürün bulunduðunda hemen döner
                    }
                    else
                    {
                        Debug.LogWarning($"[YeterliUrunBul] {rafScript.name} rafýnda {urunAdi} yetersiz. Gereken: {gerekliMiktar}, Mevcut: {mevcutAdet}.");
                    }
                }
                else
                {
                    Debug.Log($"[YeterliUrunBul] {rafScript.name} rafýnda {urunAdi} bulunamadý.");
                }
            }
        }

        // Hiçbir rafta yeterli ürün bulunamazsa
        Debug.LogWarning($"[YeterliUrunBul] Hiçbir rafta yeterli {urunAdi} bulunamadý!");
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

            if (urunBilgileri.ContainsKey(secilenUrun) && urunBilgileri[secilenUrun] >= secilenAdet)
            {
                secilenUrunler[secilenUrun] = secilenAdet;
            }
            else
            {
                int mevcutAdet = urunBilgileri.ContainsKey(secilenUrun) ? urunBilgileri[secilenUrun] : 0;
                int eksikAdet = secilenAdet - mevcutAdet;

                List<Transform> digerRaf = YeterliUrunBul(this, secilenUrun, eksikAdet);
                if (digerRaf != null && digerRaf.Count > 0)
                {
                    Debug.Log($"{secilenUrun} ürünü {secilenAdet} adet gerekiyor. Ancak bu rafta {mevcutAdet} var. Eksik {eksikAdet} adet için baþka rafa yönlendirilecek.");
                    secilenUrunler[secilenUrun] = secilenAdet;
                }
                else
                {
                    Debug.LogWarning($"{secilenUrun} ürünü için yeterli stok hiçbir rafta bulunamadý! Seçim iptal.");
                    // secilenUrunler'e ekleme yapýlmýyor
                }
            }
        }
    }


}


