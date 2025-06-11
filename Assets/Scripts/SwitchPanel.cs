using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPanel : MonoBehaviour
{
    public GameObject panel1; // �lk panel
    public GameObject panel2; // �kinci panel
    public GameObject panel3; // ���nc� panel

    public void SwitchToPanel2()
    {
        panel1.SetActive(false); // �lk paneli gizle
        panel2.SetActive(true);  // �kinci paneli g�ster
    }

    public void SwitchToPanel1()
    {
        panel2.SetActive(false); // �lk paneli gizle
        panel1.SetActive(true);  // �kinci paneli g�ster
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
