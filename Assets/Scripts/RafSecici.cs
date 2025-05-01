using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RafSecici : MonoBehaviour
{

    /* Raf olan konumlar� buluyor �r�n olan raflar� ayr� t�m raf konumalr�n� ayr� olarak buluyor ve rastgele konum d�nd�r�yor */

    private Transform[] uygunKonumlar; // �r�n olan raflar�n konumlar�
    private Transform[] tumKonumlar;   // T�m raflar�n konumlar�

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
            raf.UpdateUrunBilgileri(); // G�ncel verileri al

            foreach (Transform child in raf.transform)
            {
                if (child.CompareTag("ShelfPoint")) // E�er ShelfPoint ise
                {
                    geciciTumKonumlar.Add(child); // Her durumda t�m konumlara ekle

                    if (raf.urunBilgileri.Count > 0)
                    {
                        geciciUygunKonumlar.Add(child); // �r�n varsa uygun konumlara da ekle
                    }
                }
            }
        }

        uygunKonumlar = geciciUygunKonumlar.ToArray();
        tumKonumlar = geciciTumKonumlar.ToArray();

        Debug.Log("Uygun (�r�n Olan) ShelfPoint Konumlar�: " + uygunKonumlar.Length);
        Debug.Log("T�m ShelfPoint Konumlar�: " + tumKonumlar.Length);
    }

    public Transform GetRastgeleKonum()
    {
        Transform[] kaynak = uygunKonumlar.Length > 0 ? uygunKonumlar : tumKonumlar;

        if (kaynak.Length == 0)
        {
            Debug.LogWarning("Hi�bir ShelfPoint bulunamad�!");
            return null;
        }

        if (kaynak.Length == 1)
        {
            return kaynak[0];
        }

        return kaynak[Random.Range(0, kaynak.Length)];
    }
}
