using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckOutZoom : MonoBehaviour
{
    public Camera playerCamera;
    public Camera checkoutCamera;
    public float interactDistance = 3f;    
    public KeyCode exitKey = KeyCode.Escape;

    private bool isInCheckoutMode = false;
    private FirstPersonMovement playerController; // Oyuncu hareketini kontrol eden script
    private CheckoutCameraLook checkoutLook;

    public Image MouseÝnteract;
    public Image BackandScan;

    private GameObject Cashier;
    private Outline CashierOutline;

    void Start()
    {
        playerController = FindObjectOfType<FirstPersonMovement>();
        checkoutLook = checkoutCamera.GetComponent<CheckoutCameraLook>(); // Bunu ekle

        Cashier = GameObject.Find("Checkout");
        CashierOutline = Cashier.GetComponent<Outline>();
        CashierOutline.enabled = false;
        MouseÝnteract.enabled = false;

        // Checkout kamerasýný baþta kapatalým
        checkoutCamera.enabled = false;
    }

    void Update()
    {
        if (!isInCheckoutMode)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                if (hit.collider.CompareTag("Checkout"))
                {
                    MouseÝnteract.enabled = true;
                    CashierOutline.enabled = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        EnterCheckoutMode();
                    }                  
                }
            }
            else
            {
                MouseÝnteract.enabled = false;
                CashierOutline.enabled = false;
            }
        }
        else
        {       
            if (Input.GetKeyDown(exitKey))
            {
                ExitCheckoutMode();
            }
        }
    }

    void EnterCheckoutMode()
    {
        CashierOutline.enabled = false;
        MouseÝnteract.enabled = false;
        BackandScan.enabled = true;

        playerCamera.enabled = false;
        checkoutCamera.enabled = true;

        isInCheckoutMode = true;

        // Hareketi kapat (FirstPersonMovement)
        playerController.enabled = false;
        StopMovement();

       checkoutLook.enabled = true;
    }

    void ExitCheckoutMode()
    {
        BackandScan.enabled = false;

        checkoutCamera.enabled = false;
        playerCamera.enabled = true;
       
        isInCheckoutMode = false;

        // Hareketi aç
        playerController.enabled = true;

        checkoutLook.enabled = false;

        // Mouse imlecini kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void StopMovement()
    {
        // Rigidbody bileþeni varsa hýzýný sýfýrla
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
