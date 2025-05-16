using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZoomScript : MonoBehaviour
{
    public Camera playerCamera; // Oyuncunun ana kamerasý
    public Camera computerCamera; // Bilgisayar ekraný için özel kamera

    public float zoomSpeed = 5f; // Yakýnlaþtýrma hýzý
   // public KeyCode interactionKey = KeyCode.E; // Etkileþim tuþu
    public KeyCode exitKey = KeyCode.Escape; // Çýkýþ tuþu
   

    private bool isInFocusMode = false;
    private Vector3 originalCameraPosition; // Kameranýn orijinal pozisyonu
    private Quaternion originalCameraRotation; // Kameranýn orijinal rotasý
    private FirstPersonMovement playerController; // Oyuncu hareketini kontrol eden script
    private FirstPersonLook mouseLook; // Mouse hareketini kontrol eden script
    private bool wasCursorLocked; // Fare kilitleme durumu

    private GameObject computer; // Bilgisayar nesnesi

    public float interactionDistance = 8f; // Etkileþim mesafesi

    private Outline computerOutline; // Bilgisayarýn outline bileþeni

    public Image MouseÝnteract;
    public Image EscBack;

    void Start()
    {
        // Oyuncu hareket kontrolünü al
        playerController = FindObjectOfType<FirstPersonMovement>();

        // Mouse hareket kontrolünü al
        mouseLook = playerCamera.GetComponent<FirstPersonLook>();

        // Bilgisayar kamerasýný baþta devre dýþý býrak
        computerCamera.enabled = false;

        // Bilgisayar nesnesini bul
        computer = GameObject.Find("Computer");

        // Bilgisayar nesnesindeki Outline bileþenini al
        computerOutline = computer.GetComponent<Outline>();

        // Baþlangýçta outline'ý kapalý yap
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
            Debug.Log("E tuþuna basýldý ve bilgisayarýn yakýnýndasýnýz! Fokus moduna giriliyor.");
            EnterFocusMode();
        }

        // Çýkýþ tuþuna basýlýnca normal moda dön
        if (isInFocusMode && Input.GetKeyDown(exitKey))
        {
            Debug.Log("Escape tuþuna basýldý! Fokus modundan çýkýlýyor.");
            ExitFocusMode();
            
        }

        
    }

    

    private bool IsCrosshairOnComputer()
    {
        // Raycast ile crosshair'in bilgisayarýn colliderýna bakýp bakmadýðýný kontrol eder
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Raycast'in çarptýðý nesne "Computer" mu?
            if (hit.collider.name == "Computer")
            {
                // Bilgisayarýn oyuncuya olan mesafesini kontrol et
                float distance = Vector3.Distance(playerCamera.transform.position, hit.collider.transform.position);

                // Mesafe, etkileþim mesafesinden küçükse etkileþim yapýlabilir
                if (distance <= interactionDistance)
                {
                    // Outline'ý aktif et
                    if (computerOutline != null)
                    {
                        computerOutline.enabled = true;  // Outline aktif
                        MouseÝnteract.enabled = true;
                    }
                    return true;
                }
            }
        }

        // Mesafe uygun deðilse outline'ý kapat
        if (computerOutline != null)
        {
            computerOutline.enabled = false; // Outline'ý kapat
            MouseÝnteract.enabled = false;
        }

        return false;
    }

    private void EnterFocusMode()
    {
        MouseÝnteract.enabled = false;
        EscBack.enabled = true;

        // Fokus moduna geçiþ
        isInFocusMode = true;

        // Kameranýn mevcut pozisyonunu ve rotasýný kaydedin
        originalCameraPosition = playerCamera.transform.position;
        originalCameraRotation = playerCamera.transform.rotation;

        // Oyuncu hareketini kapat ve hareketi sýfýrla
        playerController.enabled = false;
        StopMovement();

        // Mouse imlecini göster
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Mouse kontrolünü devre dýþý býrak (kamera dönüþünü engeller)
        mouseLook.enabled = false;

        // Eðer fare kilitlendi ise, o durumu kaydet
        wasCursorLocked = Cursor.lockState == CursorLockMode.Locked;

        Debug.Log("Focus moduna geçildi. Þimdi bilgisayar ekranýna odaklanýyor.");

        // Bilgisayar kamerayý etkinleþtir
        playerCamera.enabled = false;
        computerCamera.enabled = true;
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

    private void ExitFocusMode()
    {
        // Mouse imlecini gizle ve fareyi kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EscBack.enabled = false;

        // Fokus modundan çýkýþ
        isInFocusMode = false;

        // Kamerayý eski pozisyona ve rotasýna geri döndür
        playerCamera.transform.position = originalCameraPosition;
        playerCamera.transform.rotation = originalCameraRotation;

        // Oyuncu hareketini tekrar etkinleþtir
        playerController.enabled = true;

        

        // Mouse kontrolünü yeniden etkinleþtir
        mouseLook.enabled = true;

        // Kameralar arasýnda geçiþ yap
        playerCamera.enabled = true;
        computerCamera.enabled = false;

       

        Debug.Log("Focus modundan çýkýldý ve kamera eski pozisyona döndürüldü.");
    }


   

}
