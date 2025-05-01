using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raf : MonoBehaviour
{

    /* Raftaki �r�n bilgilerinin al�yor adet ve �e�it olarak , rastgele �r�n ve adet se�iyor ,
     yeterli �r�n yoksa di�er raflar� kontrol edip yeterli �r�n olan raf�n konumunu d�nd�r�yor */

    public Dictionary<string, int> urunBilgileri = new Dictionary<string, int>(); // �r�n adetleri
    public List<string> urunCesitleri = new List<string>(); // �r�n �e�itleri
    public static List<Raf> TumRaflar = new List<Raf>(); // T�m raflar� takip etmek i�in

    // Se�ilen �r�nleri d��ar�dan eri�ilebilir hale getirdik
    public Dictionary<string, int> secilenUrunler = new Dictionary<string, int>();

    private void Awake()
    {
        TumRaflar.Add(this); // Bu raf� listeye ekle
    }

    private void OnDestroy()
    {
        TumRaflar.Remove(this); // Silindi�inde listeden ��kar
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
        List<Transform> rafKonum = new List<Transform>(); // ShelfPoint konumlar�n� saklamak i�in liste
        Debug.Log($"[YeterliUrunBul] {urunAdi} i�in {gerekliMiktar} adet aran�yor...");

        foreach (Raf rafScript in TumRaflar) // T�m raflar �zerinde d�n
        {
            if (rafScript != kendiRafi) // Kendi raf�n� kontrol etme
            {
                rafScript.UpdateUrunBilgileri(); // Raf bilgilerini g�ncelle
                Debug.Log($"[YeterliUrunBul] {rafScript.name} raf� kontrol ediliyor...");

                // Rafta istenen �r�n var m� ve yeterli mi kontrol et
                if (rafScript.urunBilgileri.ContainsKey(urunAdi))
                {
                    int mevcutAdet = rafScript.urunBilgileri[urunAdi];
                    Debug.Log($"[YeterliUrunBul] {rafScript.name} raf�nda {urunAdi} �r�n� mevcut: {mevcutAdet} adet.");

                    if (mevcutAdet >= gerekliMiktar)
                    {
                        Debug.Log($"[YeterliUrunBul] {rafScript.name} raf�nda {gerekliMiktar} adet {urunAdi} bulundu!");

                        // Raf�n i�indeki ShelfPoint'leri bul ve ekle
                        foreach (Transform child in rafScript.transform)
                        {
                            if (child.CompareTag("ShelfPoint"))
                            {
                                rafKonum.Add(child);
                            }
                        }

                        Debug.Log($"[YeterliUrunBul] {rafKonum.Count} ShelfPoint bulundu. Bu raf se�ildi.");
                        return rafKonum; // Yeterli �r�n bulundu�unda hemen d�ner
                    }
                    else
                    {
                        Debug.LogWarning($"[YeterliUrunBul] {rafScript.name} raf�nda {urunAdi} yetersiz. Gereken: {gerekliMiktar}, Mevcut: {mevcutAdet}.");
                    }
                }
                else
                {
                    Debug.Log($"[YeterliUrunBul] {rafScript.name} raf�nda {urunAdi} bulunamad�.");
                }
            }
        }

        // Hi�bir rafta yeterli �r�n bulunamazsa
        Debug.LogWarning($"[YeterliUrunBul] Hi�bir rafta yeterli {urunAdi} bulunamad�!");
        return rafKonum;
    }



    public void RastgeleUrunSec()
    {
        secilenUrunler.Clear();  // �nceki se�imleri temizle

        if (urunCesitleri.Count == 0)
        {
            Debug.LogWarning("�r�n �e�itleri listesi bo�, se�im yap�lamad�!");
            return;
        }

        System.Random rand = new System.Random();
        int urunSecimAdedi = Mathf.Min(rand.Next(1, 3), urunCesitleri.Count); // 1 veya 2 �e�it �r�n se�
        List<string> kopyaListe = new List<string>(urunCesitleri); // �r�n listesinin kopyas�n� al

        for (int i = 0; i < urunSecimAdedi; i++)
        {
            if (kopyaListe.Count == 0) break;

            int rastgeleIndex = rand.Next(kopyaListe.Count);
            string secilenUrun = kopyaListe[rastgeleIndex];
            kopyaListe.RemoveAt(rastgeleIndex);

            int secilenAdet = rand.Next(1, 4); // 1-3 adet se�

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
                    Debug.Log($"{secilenUrun} �r�n� {secilenAdet} adet gerekiyor. Ancak bu rafta {mevcutAdet} var. Eksik {eksikAdet} adet i�in ba�ka rafa y�nlendirilecek.");
                    secilenUrunler[secilenUrun] = secilenAdet;
                }
                else
                {
                    Debug.LogWarning($"{secilenUrun} �r�n� i�in yeterli stok hi�bir rafta bulunamad�! Se�im iptal.");
                    // secilenUrunler'e ekleme yap�lm�yor
                }
            }
        }
    }


}


