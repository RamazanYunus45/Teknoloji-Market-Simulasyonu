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
    private FirstPersonMovement playerController;
    private CheckoutCameraLook checkoutLook;

    public Image Mouse›nteract;
    public Image BackandScan;
    private Image space;

    private GameObject Cashier;
    private Outline CashierOutline;
    private Collider[] cashierColliders;

    void Start()
    {
        playerController = FindObjectOfType<FirstPersonMovement>();
        checkoutLook = checkoutCamera.GetComponent<CheckoutCameraLook>();

        Cashier = GameObject.Find("Checkout");
        CashierOutline = Cashier.GetComponent<Outline>();
        CashierOutline.enabled = false;
        Mouse›nteract.enabled = false;

        checkoutCamera.enabled = false;
        cashierColliders = Cashier.GetComponentsInChildren<Collider>();

        space = GameObject.Find("Back").GetComponent<Image>();
        space.enabled = false;
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
                    Mouse›nteract.enabled = true;
                    CashierOutline.enabled = true;

                    if (Input.GetMouseButtonDown(0))
                    {
                        EnterCheckoutMode();
                    }
                }
            }
            else
            {
                Mouse›nteract.enabled = false;
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
        Mouse›nteract.enabled = false;
        BackandScan.enabled = true;
        space.enabled = true;
        playerCamera.enabled = false;
        checkoutCamera.enabled = true;

        isInCheckoutMode = true;
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
        space.enabled = false;
        checkoutCamera.enabled = false;
        playerCamera.enabled = true;

        isInCheckoutMode = false;
        playerController.enabled = true;
        checkoutLook.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var col in cashierColliders)
        {
            col.enabled = true;
        }
    }

    private void StopMovement()
    {
        Rigidbody rb = playerController.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
