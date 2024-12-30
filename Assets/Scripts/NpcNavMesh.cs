using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NpcVavMesh : MonoBehaviour
{
    public string[] availableProductTags; // Maðazadaki ürünlerin tag'larý
    public Transform target1; // Ýlk hedef raf
    public Transform target2; // Ýkinci hedef raf
    public Transform target3; // Kasa hedefi
    public Transform target4; // Çýkýþ hedefi
    public Transform hand; // NPC'nin elindeki hand GameObject'i

    private string selectedProduct1;
    private string selectedProduct2;
    private NavMeshAgent agent;
    private Animator animator; // Animator referansý
    public TextMeshProUGUI textMesh;

    public Button exitButton;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent referansý
        animator = GetComponent<Animator>(); // Animator referansý

        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro bileþeni atanmamýþ! Lütfen inspector üzerinden atayýn.");
            return;
        }

        if (hand == null)
        {
            Debug.LogError("Hand GameObject'i atanmamýþ! Lütfen inspector üzerinden atayýn.");
            return;
        }

        SelectRandomProducts();
        DisplaySelectedProducts();
        GoToRandomShelf();


    }

    // Rastgele 2 ürün seç
    public void SelectRandomProducts()
    {
        selectedProduct1 = availableProductTags[Random.Range(0, availableProductTags.Length)];
        selectedProduct2 = availableProductTags[Random.Range(0, availableProductTags.Length)];
    }

    // Seçilen ürünleri TextMeshPro'da göster
    public void DisplaySelectedProducts()
    {
        textMesh.text = $"Ürünler:\n{selectedProduct1}\n{selectedProduct2}";
    }

    // Rastgele bir rafýn önüne git
    public void GoToRandomShelf()
    {
        Transform selectedTarget = Random.Range(0, 2) == 0 ? target1 : target2;
        agent.SetDestination(selectedTarget.position);
        StartCoroutine(CheckForProductsAtShelf(selectedTarget));
    }

    // Raflarda ürün kontrolü yap
    public IEnumerator CheckForProductsAtShelf(Transform shelf)
    {
        // NPC hedefe ulaþtýðýnda kontrol yapýlýr
        while (Vector3.Distance(transform.position, shelf.position) > 1f)
        {
            yield return null;
        }

        // Raflara geldiðinde hýzýný sýfýrlýyoruz
        agent.speed = 0f;  // Hýz sýfýrlanýr

        // Animasyon hýzýný 0 yapýyoruz
        if (animator != null)
        {
            animator.speed = 0f;  // Animasyon hýzýný sýfýrla
            animator.SetTrigger("Idle"); // Idle animasyonunu tetikle
        }

        // Animasyonu durdur
        agent.isStopped = true;

        Collider[] colliders = Physics.OverlapSphere(shelf.position, 2f); // Rafýn çevresindeki collider'larý kontrol et
        foreach (var col in colliders)
        {
            Debug.Log("Bulunan nesne: " + col.name);  // Tüm bulunan nesnelerin ismini logluyoruz
            if (col.CompareTag("Shelf"))  // Eðer nesne Shelf tag'ine sahipse
            {
                foreach (Transform child in col.transform)
                {
                    foreach (Transform grandChild in child)
                    {
                        foreach (Transform greatGrandChild in grandChild)
                        {
                            // Eðer child'ýn child'ý ve onun child'ý belirtilen ürünlerden biriyle eþleþiyorsa
                            if (greatGrandChild.CompareTag(selectedProduct1) || greatGrandChild.CompareTag(selectedProduct2))
                            {
                                // Ürün bulundu, "hand" GameObject'inin child'ý olarak taþý
                                greatGrandChild.SetParent(hand); // "hand" nesnesinin child'ý yap
                                greatGrandChild.localPosition = Vector3.zero; // Elin içinde sýfýr pozisyona yerleþtir

                                Debug.Log($"Ürün bulundu ve elin içine eklendi: {greatGrandChild.tag}");

                                // 3 saniye bekle
                                yield return new WaitForSeconds(3f);

                                // Hýz tekrar eski deðerine döndürülebilir
                                agent.speed = 3.5f; // Hýzý eski haline döndür (örnek hýz deðeri)

                                // Animasyon hýzýný eski haline getir
                                if (animator != null)
                                {
                                    animator.speed = 1f; // Animasyon hýzýný tekrar 1 yap
                                }

                                // Kasa hedefine git
                                GoToCheckout();
                                yield break; // Ürün bulunduysa iþlem sonlanýr
                            }
                        }
                    }
                }
            }
        }
        agent.isStopped = false;
        agent.speed = 3.5f;
        animator.speed = 1f;
        // Eðer ürün bulunamadýysa çýkýþa git
        GoToExit();
    }

    // Kasa hedefine git
    public void GoToCheckout()
    {
        agent.isStopped = false;
        agent.SetDestination(target3.position);
        Debug.Log("Kasa önüne gidildi.");
        StartCoroutine(StopAnimationWhenArrived(target3)); // Kasa hedefine varýnca animasyonu durdurmak için coroutine baþlatýyoruz
    }

    // Çýkýþa git
    public void GoToExit()
    {

        agent.isStopped = false;
        agent.speed = 3.5f;
        animator.speed = 1f;
        agent.SetDestination(target4.position);
        Debug.Log("Çýkýþa gidildi.");
        StartCoroutine(StopAnimationWhenArrived(target4)); // Çýkýþ hedefine varýnca animasyonu durdurmak için coroutine baþlatýyoruz
    }

    // Hedefe varýldýðýnda animasyon hýzýný sýfýrla
    public IEnumerator StopAnimationWhenArrived(Transform target)
    {
        // Hedefe yaklaþýrken kontrol et
        while (Vector3.Distance(transform.position, target.position) > 1f)
        {
            yield return null;
        }

        // Hedefe varýldýðýnda animasyon hýzýný sýfýrla
        if (animator != null)
        {
            animator.speed = 0f;  // Animasyon hýzýný sýfýrla
            Debug.Log("Hedefe varýldý, animasyon hýzý sýfýrlandý.");
        }

        // Hedefe varýldýðýnda agent'ý durdur
        agent.isStopped = true;

        // Hedefe varýldýðýnda çýkýþ veya kasa iþlemi tamamlandýðýnda devam etmesini saðla
        if (target == target3)
        {
            // Kasa hedefine varýldýysa, ödeme iþlemini baþlatabiliriz veya baþka bir iþlem yapýlabilir.
            Debug.Log("Kasa iþlemi tamamlandý.");
        }
        else if (target == target4)
        {
            // Çýkýþa varýldýðýnda çýkýþ iþlemi yapýlabilir.
            Debug.Log("Çýkýþa varýldý.");
        }
    }

    public void OnExitButtonPressed()
    {
        // Eðer NPC kasa noktasýna ulaþtýysa, çýkýþa gitmesini saðla
        if (Vector3.Distance(transform.position, target3.position) <= 1f)
        {
            Debug.Log("Kasa hedefine ulaþýldý, çýkýþa gidiliyor...");
            GoToExit();  // Çýkýþ noktasýna git
        }
    }
}
