using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RafSecici : MonoBehaviour
{

    /* Raf olan konumlarý buluyor ürün olan raflarý ayrý tüm raf konumalrýný ayrý olarak buluyor ve rastgele konum döndürüyor */

    private Transform[] uygunKonumlar; // Ürün olan raflarýn konumlarý
    private Transform[] tumKonumlar;   // Tüm raflarýn konumlarý

    private void Start()
    {
        RaflariKontrolEt();
    }

    public void RaflariKontrolEt()
    {
        List<Transform> geciciUygunKonumlar = new List<Transform>();
        List<Transform> geciciTumKonumlar = new List<Transform>();

        foreach (Raf raf in Raf.TumRaflar)
        {
            raf.UpdateUrunBilgileri(); // Güncel verileri al

            foreach (Transform child in raf.transform)
            {
                if (child.CompareTag("ShelfPoint")) // Eðer ShelfPoint ise
                {
                    geciciTumKonumlar.Add(child); // Her durumda tüm konumlara ekle

                    if (raf.urunBilgileri.Count > 0)
                    {
                        geciciUygunKonumlar.Add(child); // Ürün varsa uygun konumlara da ekle
                    }
                }
            }
        }

        uygunKonumlar = geciciUygunKonumlar.ToArray();
        tumKonumlar = geciciTumKonumlar.ToArray();

        Debug.Log("Uygun (Ürün Olan) ShelfPoint Konumlarý: " + uygunKonumlar.Length);
        Debug.Log("Tüm ShelfPoint Konumlarý: " + tumKonumlar.Length);
    }

    public Transform GetRastgeleKonum()
    {
        Transform[] kaynak = uygunKonumlar.Length > 0 ? uygunKonumlar : tumKonumlar;

        if (kaynak.Length == 0)
        {
            Debug.LogWarning("Hiçbir ShelfPoint bulunamadý!");
            return null;
        }

        if (kaynak.Length == 1)
        {
            return kaynak[0];
        }

        return kaynak[Random.Range(0, kaynak.Length)];
    }
}
