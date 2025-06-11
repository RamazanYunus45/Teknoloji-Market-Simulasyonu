using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScanSystem : MonoBehaviour 
{
    public Camera CheckoutKamera;
    public float toplamFiyat = 0f;
    public float npcPara = 0f;

    int scanCount = 2;

    public TextMeshProUGUI Receýved_Text;
    public TextMeshProUGUI Total_Text;
    public TextMeshProUGUI Change_Text;
          
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ScanUrun();
        }
    }

    public void ScanUrun()
    {            
        Ray ray = CheckoutKamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            UrunBilgi urun = hit.collider.GetComponent<UrunBilgi>();
            if (urun != null)
            {
                urun.Scan();
                toplamFiyat += urun.fiyat;
                scanCount++;            
            }                     
               // if (hit.collider.CompareTag("Money") && Input.GetMouseButtonDown(0))
               // {
                    HesaplaNpcPara();
                    Debug.Log("Tüm ürünler tarandý.");
               // }                      
        }
    }

    public void HesaplaNpcPara()
    {       
        int ekstraPara = Random.Range(1, 51);
        npcPara = toplamFiyat + ekstraPara;

        Receýved_Text.text = npcPara.ToString();
        Total_Text.text = toplamFiyat.ToString();
        Change_Text.text = (npcPara - toplamFiyat).ToString();
    }
}
