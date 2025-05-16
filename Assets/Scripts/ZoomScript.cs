using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZoomScript : MonoBehaviour
{
    public Camera playerCamera; // Oyuncunun ana kameras�
    public Camera computerCamera; // Bilgisayar ekran� i�in �zel kamera

    public float zoomSpeed = 5f; // Yak�nla�t�rma h�z�
   // public KeyCode interactionKey = KeyCode.E; // Etkile�im tu�u
    public KeyCode exitKey = KeyCode.Escape; // ��k�� tu�u
   

    private bool isInFocusMode = false;
    private Vector3 originalCameraPosition; // Kameran�n orijinal pozisyonu
    private Quaternion originalCameraRotation; // Kameran�n orijinal rotas�
    private FirstPersonMovement playerController; // Oyuncu hareketini kontrol eden script
    private FirstPersonLook mouseLook; // Mouse hareketini kontrol eden script
    private bool wasCursorLocked; // Fare kilitleme durumu

    private GameObject computer; // Bilgisayar nesnesi

    public float interactionDistance = 8f; // Etkile�im mesafesi

    private Outline computerOutline; // Bilgisayar�n outline bile�eni

    public Image Mouse�nteract;
    public Image EscBack;

    void Start()
    {
        // Oyuncu hareket kontrol�n� al
        playerController = FindObjectOfType<FirstPersonMovement>();

        // Mouse hareket kontrol�n� al
        mouseLook = playerCamera.GetComponent<FirstPersonLook>();

        // Bilgisayar kameras�n� ba�ta devre d��� b�rak
        computerCamera.enabled = false;

        // Bilgisayar nesnesini bul
        computer = GameObject.Find("Computer");

        // Bilgisayar nesnesindeki Outline bile�enini al
        computerOutline = computer.GetComponent<Outline>();

        // Ba�lang��ta outline'� kapal� yap
        if (computerOutline != null)
        {
            computerOutline.enabled = false;
        }

        EscBack.enabled = false;
    }

    void Update()
    {
        // Bilgisayar nesnesine olan mesafeyi kontrol et
        if (!isInFocusMode && IsCrosshairOnComputer() && Input.GetMouseButtonDown(0))
        {
            Debug.Log("E tu�una bas�ld� ve bilgisayar�n yak�n�ndas�n�z! Fokus moduna giriliyor.");
            EnterFocusMode();
        }

        // ��k�� tu�una bas�l�nca normal moda d�n
        if (isInFocusMode && Input.GetKeyDown(exitKey))
        {
            Debug.Log("Escape tu�una bas�ld�! Fokus modundan ��k�l�yor.");
            ExitFocusMode();
            
        }

        
    }

    

    private bool IsCrosshairOnComputer()
    {
        // Raycast ile crosshair'in bilgisayar�n collider�na bak�p bakmad���n� kontrol eder
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Raycast'in �arpt��� nesne "Computer" mu?
            if (hit.collider.name == "Computer")
            {
                // Bilgisayar�n oyuncuya olan mesafesini kontrol et
                float distance = Vector3.Distance(playerCamera.transform.position, hit.collider.transform.position);

                // Mesafe, etkile�im mesafesinden k���kse etkile�im yap�labilir
                if (distance <= interactionDistance)
                {
                    // Outline'� aktif et
                    if (computerOutline != null)
                    {
                        computerOutline.enabled = true;  // Outline aktif
                        Mouse�nteract.enabled = true;
                    }
                    return true;
                }
            }
        }

        // Mesafe uygun de�ilse outline'� kapat
        if (computerOutline != null)
        {
            computerOutline.enabled = false; // Outline'� kapat
            Mouse�nteract.enabled = false;
        }

        return false;
    }

    private void EnterFocusMode()
    {
        Mouse�nteract.enabled = false;
        EscBack.enabled = true;

        // Fokus moduna ge�i�
        isInFocusMode = true;

        // Kameran�n mevcut pozisyonunu ve rotas�n� kaydedin
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        // Oyuncu hareketini kapat ve hareketi s�f�rla
        playerController.enabled = false;
        StopMovement();

        // Mouse imlecini g�ster
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Mouse kontrol�n� devre d��� b�rak (kamera d�n���n� engeller)
        mouseLook.enabled = false;

        // E�er fare kilitlendi ise, o durumu kaydet
        wasCursorLocked = Cursor.lockState == CursorLockMode.Locked;

        Debug.Log("Focus moduna ge�ildi. �imdi bilgisayar ekran�na odaklan�yor.");

        // Bilgisayar kameray� etkinle�tir
        playerCamera.enabled = false;
        computerCamera.enabled = true;
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

    private void ExitFocusMode()
    {
        // Mouse imlecini gizle ve fareyi kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EscBack.enabled = false;

        // Fokus modundan ��k��
        isInFocusMode = false;

        // Kameray� eski pozisyona ve rotas�na geri d�nd�r
        playerCamera.transform.position = originalCameraPosition;
        playerCamera.transform.rotation = originalCameraRotation;

        // Oyuncu hareketini tekrar etkinle�tir
        playerController.enabled = true;

        

        // Mouse kontrol�n� yeniden etkinle�tir
        mouseLook.enabled = true;

        // Kameralar aras�nda ge�i� yap
        playerCamera.enabled = true;
        computerCamera.enabled = false;

       

        Debug.Log("Focus modundan ��k�ld� ve kamera eski pozisyona d�nd�r�ld�.");
    }


   

}
