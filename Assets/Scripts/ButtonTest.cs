using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol týk
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.name);

                // Eðer UI bir butona çarptýysak
                if (hit.collider.CompareTag("UIButton"))
                {
                    // UnityEngine.UI Button'a eriþim
                    var button = hit.collider.GetComponentInParent<UnityEngine.UI.Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke(); // Butonun click event'ini çaðýr
                        Debug.Log("Button clicked: " + button.name);
                    }
                }
            }
        }
    }
}
