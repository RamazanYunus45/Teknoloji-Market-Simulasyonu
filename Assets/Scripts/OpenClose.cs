using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenClose : MonoBehaviour
{
    public Camera playerCamera;
    public float interactDistance = 3f;
    private Animation anim;
    public Image Mouse›nteract;
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
                Mouse›nteract.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {                   
                    anim.Play("Open");
                    SpawnetmeDurumu = true;
                }
            }


            if (hit.collider.CompareTag("Open"))
            {
                Mouse›nteract.enabled = true;
                if (Input.GetMouseButtonDown(0))
                {                   
                    anim.Play("Close");
                    SpawnetmeDurumu = false;
                }

            }     

        }

        else
        {
            Mouse›nteract.enabled = false;
        }

    }
}
    
