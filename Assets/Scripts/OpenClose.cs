using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenClose : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 3f;
    private Animation anim;
    public Image MouseŻnteract;
    public bool SpawnetmeDurumu = false;

    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Close"))
            {
                MouseŻnteract.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {                   
                    anim.Play("Open");
                    SpawnetmeDurumu = true;
                }
            }
            if (hit.collider.CompareTag("Open"))
            {
                MouseŻnteract.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {                   
                    anim.Play("Close");
                    SpawnetmeDurumu = false;
                }
            }     
        }
        else
        {
            MouseŻnteract.enabled = false;
        }
    }
}
    
