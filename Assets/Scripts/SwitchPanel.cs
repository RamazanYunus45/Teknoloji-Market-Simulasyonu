using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPanel : MonoBehaviour
{
    public GameObject panel1; // Ýlk panel
    public GameObject panel2; // Ýkinci panel
    public GameObject panel3; // Üçüncü panel

    public void SwitchToPanel2()
    {
        panel1.SetActive(false); // Ýlk paneli gizle
        panel2.SetActive(true);  // Ýkinci paneli göster
    }

    public void SwitchToPanel1()
    {
        panel2.SetActive(false); // Ýlk paneli gizle
        panel1.SetActive(true);  // Ýkinci paneli göster
    }

    public void SwitchToPanel3()
    {       
        panel3.SetActive(true);  
    }

    public void SwitchToPanel4()
    {
        panel3.SetActive(false);
        panel2.SetActive(true);
    }
}
