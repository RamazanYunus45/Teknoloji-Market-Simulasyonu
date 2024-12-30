using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NpcVavMesh : MonoBehaviour
{
    public string[] availableProductTags; // Ma�azadaki �r�nlerin tag'lar�
    public Transform target1; // �lk hedef raf
    public Transform target2; // �kinci hedef raf
    public Transform target3; // Kasa hedefi
    public Transform target4; // ��k�� hedefi
    public Transform hand; // NPC'nin elindeki hand GameObject'i

    private string selectedProduct1;
    private string selectedProduct2;
    private NavMeshAgent agent;
    private Animator animator; // Animator referans�
    public TextMeshProUGUI textMesh;

    public Button exitButton;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // NavMeshAgent referans�
        animator = GetComponent<Animator>(); // Animator referans�

        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro bile�eni atanmam��! L�tfen inspector �zerinden atay�n.");
            return;
        }

        if (hand == null)
        {
            Debug.LogError("Hand GameObject'i atanmam��! L�tfen inspector �zerinden atay�n.");
            return;
        }

        SelectRandomProducts();
        DisplaySelectedProducts();
        GoToRandomShelf();


    }

    // Rastgele 2 �r�n se�
    public void SelectRandomProducts()
    {
        selectedProduct1 = availableProductTags[Random.Range(0, availableProductTags.Length)];
        selectedProduct2 = availableProductTags[Random.Range(0, availableProductTags.Length)];
    }

    // Se�ilen �r�nleri TextMeshPro'da g�ster
    public void DisplaySelectedProducts()
    {
        textMesh.text = $"�r�nler:\n{selectedProduct1}\n{selectedProduct2}";
    }

    // Rastgele bir raf�n �n�ne git
    public void GoToRandomShelf()
    {
        Transform selectedTarget = Random.Range(0, 2) == 0 ? target1 : target2;
        agent.SetDestination(selectedTarget.position);
        StartCoroutine(CheckForProductsAtShelf(selectedTarget));
    }

    // Raflarda �r�n kontrol� yap
    public IEnumerator CheckForProductsAtShelf(Transform shelf)
    {
        // NPC hedefe ula�t���nda kontrol yap�l�r
        while (Vector3.Distance(transform.position, shelf.position) > 1f)
        {
            yield return null;
        }

        // Raflara geldi�inde h�z�n� s�f�rl�yoruz
        agent.speed = 0f;  // H�z s�f�rlan�r

        // Animasyon h�z�n� 0 yap�yoruz
        if (animator != null)
        {
            animator.speed = 0f;  // Animasyon h�z�n� s�f�rla
            animator.SetTrigger("Idle"); // Idle animasyonunu tetikle
        }

        // Animasyonu durdur
        agent.isStopped = true;

        Collider[] colliders = Physics.OverlapSphere(shelf.position, 2f); // Raf�n �evresindeki collider'lar� kontrol et
        foreach (var col in colliders)
        {
            Debug.Log("Bulunan nesne: " + col.name);  // T�m bulunan nesnelerin ismini logluyoruz
            if (col.CompareTag("Shelf"))  // E�er nesne Shelf tag'ine sahipse
            {
                foreach (Transform child in col.transform)
                {
                    foreach (Transform grandChild in child)
                    {
                        foreach (Transform greatGrandChild in grandChild)
                        {
                            // E�er child'�n child'� ve onun child'� belirtilen �r�nlerden biriyle e�le�iyorsa
                            if (greatGrandChild.CompareTag(selectedProduct1) || greatGrandChild.CompareTag(selectedProduct2))
                            {
                                // �r�n bulundu, "hand" GameObject'inin child'� olarak ta��
                                greatGrandChild.SetParent(hand); // "hand" nesnesinin child'� yap
                                greatGrandChild.localPosition = Vector3.zero; // Elin i�inde s�f�r pozisyona yerle�tir

                                Debug.Log($"�r�n bulundu ve elin i�ine eklendi: {greatGrandChild.tag}");

                                // 3 saniye bekle
                                yield return new WaitForSeconds(3f);

                                // H�z tekrar eski de�erine d�nd�r�lebilir
                                agent.speed = 3.5f; // H�z� eski haline d�nd�r (�rnek h�z de�eri)

                                // Animasyon h�z�n� eski haline getir
                                if (animator != null)
                                {
                                    animator.speed = 1f; // Animasyon h�z�n� tekrar 1 yap
                                }

                                // Kasa hedefine git
                                GoToCheckout();
                                yield break; // �r�n bulunduysa i�lem sonlan�r
                            }
                        }
                    }
                }
            }
        }
        agent.isStopped = false;
        agent.speed = 3.5f;
        animator.speed = 1f;
        // E�er �r�n bulunamad�ysa ��k��a git
        GoToExit();
    }

    // Kasa hedefine git
    public void GoToCheckout()
    {
        agent.isStopped = false;
        agent.SetDestination(target3.position);
        Debug.Log("Kasa �n�ne gidildi.");
        StartCoroutine(StopAnimationWhenArrived(target3)); // Kasa hedefine var�nca animasyonu durdurmak i�in coroutine ba�lat�yoruz
    }

    // ��k��a git
    public void GoToExit()
    {

        agent.isStopped = false;
        agent.speed = 3.5f;
        animator.speed = 1f;
        agent.SetDestination(target4.position);
        Debug.Log("��k��a gidildi.");
        StartCoroutine(StopAnimationWhenArrived(target4)); // ��k�� hedefine var�nca animasyonu durdurmak i�in coroutine ba�lat�yoruz
    }

    // Hedefe var�ld���nda animasyon h�z�n� s�f�rla
    public IEnumerator StopAnimationWhenArrived(Transform target)
    {
        // Hedefe yakla��rken kontrol et
        while (Vector3.Distance(transform.position, target.position) > 1f)
        {
            yield return null;
        }

        // Hedefe var�ld���nda animasyon h�z�n� s�f�rla
        if (animator != null)
        {
            animator.speed = 0f;  // Animasyon h�z�n� s�f�rla
            Debug.Log("Hedefe var�ld�, animasyon h�z� s�f�rland�.");
        }

        // Hedefe var�ld���nda agent'� durdur
        agent.isStopped = true;

        // Hedefe var�ld���nda ��k�� veya kasa i�lemi tamamland���nda devam etmesini sa�la
        if (target == target3)
        {
            // Kasa hedefine var�ld�ysa, �deme i�lemini ba�latabiliriz veya ba�ka bir i�lem yap�labilir.
            Debug.Log("Kasa i�lemi tamamland�.");
        }
        else if (target == target4)
        {
            // ��k��a var�ld���nda ��k�� i�lemi yap�labilir.
            Debug.Log("��k��a var�ld�.");
        }
    }

    public void OnExitButtonPressed()
    {
        // E�er NPC kasa noktas�na ula�t�ysa, ��k��a gitmesini sa�la
        if (Vector3.Distance(transform.position, target3.position) <= 1f)
        {
            Debug.Log("Kasa hedefine ula��ld�, ��k��a gidiliyor...");
            GoToExit();  // ��k�� noktas�na git
        }
    }
}
