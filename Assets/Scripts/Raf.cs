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
        List<Transform> rafKonum = new List<Transform>(); // ShelfPoint konumlar�n� saklamak i�in
        int kalanMiktar = gerekliMiktar;

        Debug.Log($"[YeterliUrunBul] {urunAdi} i�in {gerekliMiktar} adet aran�yor...");

        // �lk rafta ne kadar �r�n vard� ve nerede saklayal�m
        int ilkRafMevcutAdet = 0;
        List<Transform> ilkRafKonumlar = new List<Transform>();

        // �nce ilk raftaki bilgileri alal�m
        kendiRafi.UpdateUrunBilgileri();
        if (kendiRafi.urunBilgileri.ContainsKey(urunAdi))
        {
            ilkRafMevcutAdet = kendiRafi.urunBilgileri[urunAdi];
            Debug.Log($"[YeterliUrunBul] �lk rafta {urunAdi} mevcut: {ilkRafMevcutAdet} adet.");

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

        kalanMiktar -= Mathf.Min(ilkRafMevcutAdet, kalanMiktar); // �lk raftan ne kadar alabildikse ��kar

        // �imdi di�er raflar� kontrol et
        foreach (Raf rafScript in TumRaflar)
        {
            if (rafScript != kendiRafi) // Kendini kontrol etme
            {
                rafScript.UpdateUrunBilgileri();
                Debug.Log($"[YeterliUrunBul] {rafScript.name} raf� kontrol ediliyor...");

                if (rafScript.urunBilgileri.ContainsKey(urunAdi))
                {
                    int mevcutAdet = rafScript.urunBilgileri[urunAdi];
                    Debug.Log($"[YeterliUrunBul] {rafScript.name} raf�nda {urunAdi} mevcut: {mevcutAdet} adet.");

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

                        // Gerekli miktar tamamland�ysa ��k
                        if (kalanMiktar <= 0)
                        {
                            Debug.Log($"[YeterliUrunBul] {urunAdi} �r�n� i�in di�er raflardan yeterli miktar al�nd�.");
                            // �lk raftan da �r�n ald�ysak onu da ekle
                            rafKonum.AddRange(ilkRafKonumlar);
                            return rafKonum;
                        }
                    }
                }
            }
        }

        // Di�er raflar bitti  ne kadar toplayabildiysek onu d�nd�r
        Debug.Log($"[YeterliUrunBul] T�m raflardan toplanan �r�n adedi yeterli de�il. Ne bulduysak al�yoruz.");

        // �lk raftan da �r�n ald�ysak onlar� da ekle
        rafKonum.AddRange(ilkRafKonumlar);

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

            // �r�n� al�rken, YeterliUrunBul fonksiyonu ile t�m raflarda arama yapaca��z
            List<Transform> rafKonumlar = YeterliUrunBul(this, secilenUrun, secilenAdet);

            if (rafKonumlar != null && rafKonumlar.Count > 0)
            {
                secilenUrunler[secilenUrun] = secilenAdet;
                Debug.Log($"{secilenUrun} �r�n� {secilenAdet} adet ba�ar�yla se�ildi.");
            }
            else
            {
                Debug.LogWarning($"{secilenUrun} �r�n� i�in yeterli stok hi�bir rafta bulunamad�! Se�im iptal.");
            }
        }
    }


}


