using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public Camera mainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol t�k
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.name);

                // E�er UI bir butona �arpt�ysak
                if (hit.collider.CompareTag("UIButton"))
                {
                    // UnityEngine.UI Button'a eri�im
                    var button = hit.collider.GetComponentInParent<UnityEngine.UI.Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke(); // Butonun click event'ini �a��r
                        Debug.Log("Button clicked: " + button.name);
                    }
                }
            }
        }
    }
}
