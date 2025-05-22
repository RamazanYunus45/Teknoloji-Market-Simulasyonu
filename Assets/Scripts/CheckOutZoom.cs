using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckOutZoom : MonoBehaviour
{
    public Camera playerCamera;
    public Camera checkoutCamera;
    public float interactDistance = 5f;    
    public KeyCode exitKey = KeyCode.Escape;

    private bool isInCheckoutMode = false;
    private FirstPersonMovement playerController; // Oyuncu hareketini kontrol eden script
    private CheckoutCameraLook checkoutLook;

    public Image Mouse�nteract;
    public Image BackandScan;

    private GameObject Cashier;
    private Outline CashierOutline;

    private Collider[] cashierColliders;

    void Start()
    {
        playerController = FindObjectOfType<FirstPersonMovement>();
        checkoutLook = checkoutCamera.GetComponent<CheckoutCameraLook>(); // Bunu ekle

        Cashier = GameObject.Find("Checkout");
        CashierOutline = Cashier.GetComponent<Outline>();
        CashierOutline.enabled = false;
        Mouse�nteract.enabled = false;

        // Checkout kameras�n� ba�ta kapatal�m
        checkoutCamera.enabled = false;

        cashierColliders = Cashier.GetComponentsInChildren<Collider>();
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
                    Mouse�nteract.enabled = true;
                    CashierOutline.enabled = true;
                    if (Input.GetMouseButtonDown(0))
                    {
                        EnterCheckoutMode();
                    }                  
                }
            }
            else
            {
                Mouse�nteract.enabled = false;
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
        Mouse�nteract.enabled = false;
        BackandScan.enabled = true;

        playerCamera.enabled = false;
        checkoutCamera.enabled = true;

        isInCheckoutMode = true;

        // Hareketi kapat (FirstPersonMovement)
        playerController.enabled = false;
        StopMovement();

        checkoutLook.enabled = true;

        foreach (var col in cashierColliders)
        {
            col.enabled = false;
        }
    }

    void ExitCheckoutMode()
    {
        BackandScan.enabled = false;

        checkoutCamera.enabled = false;
        playerCamera.enabled = true;
       
        isInCheckoutMode = false;

        // Hareketi a�
        playerController.enabled = true;

        checkoutLook.enabled = false;

        // Mouse imlecini kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var col in cashierColliders)
        {
            col.enabled = true;
        }      
    }

    private void StopMovement()
    {
        // Rigidbody bile�eni varsa h�z�n� s�f�rla
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
