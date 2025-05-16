using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;

public class NpcVavMesh : MonoBehaviour
{

    /* Npc bir raf collider � ile �ak��t�ysa i�leme ba�l�yor , �r�n almaya ba�l�yor raftan tek tek �r�nleri topluyor */


    private Dictionary<string, int> npcUrunBilgileri = new Dictionary<string, int>();
    public Transform Hand;
    private NavMeshAgent agent;
    private bool urunSecildi = false;

    private NPCHareket npcHareket;

    private Animator animator;

    public List<Transform> kasaNoktalari; // 6 konum
    public List<Transform> beklemeNoktalari; // 2 konum

    private static HashSet<int> doluKasaIndexleri = new HashSet<int>(); // hangi kasalar dolu
   
    public List<Transform> urunKoymaNoktalari; // Kasaya yerle�tirme pozisyonlar�
    private List<GameObject> kasayaKonulanUrunler = new List<GameObject>();

    private static Queue<NpcVavMesh> kasaSirasi = new Queue<NpcVavMesh>();
    private int kendiKasaIndex = -1;

    public bool Dolu = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent bile�eni bulunamad�!");
        }

        npcHareket = GetComponent<NPCHareket>();
        if (npcHareket == null)
        {
            Debug.LogError("NPCHareket bile�eni bulunamad�!");
        }

        animator = GetComponent<Animator>(); // Animator bile�enini al
     

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shelf") && !urunSecildi)
        {
            // Kar��la��lan raf�n ad�
            string carpanRafAdi = other.name;

            // Hedef rafla ayn� m�
            if (carpanRafAdi != npcHareket.hedefRaf�smi)
            {
                Debug.Log($"Bu raf hedef de�il: {carpanRafAdi} != {npcHareket.hedefRaf�smi}");
                return;
            }

            Raf rafScript = other.GetComponent<Raf>();
            if (rafScript != null)
            {
                rafScript.UpdateUrunBilgileri();
                rafScript.RastgeleUrunSec();
                npcUrunBilgileri = new Dictionary<string, int>(rafScript.secilenUrunler);
                urunSecildi = true;
                Debug.Log("Raf bilgileri NPC'ye aktar�ld�.");

                StartCoroutine(UrunAlmaRutini(rafScript));
            }
        }
    }

    private IEnumerator UrunAlmaRutini(Raf rafScript)
    {
        foreach (var urun in npcUrunBilgileri.ToList())
        {
            string urunAdi = urun.Key;
            int gerekliAdet = urun.Value;
            int kalanAdet = gerekliAdet;

            Debug.Log($"�r�n: {urunAdi}, Gerekli Adet: {gerekliAdet}");

            int alinanAdet = 0;

            // �lk raftan �r�nleri al
            if (rafScript.urunBilgileri.ContainsKey(urunAdi) && rafScript.urunBilgileri[urunAdi] > 0)
            {
                int alinan = 0;
                yield return StartCoroutine(UrunleriRaftanTopla(rafScript, urunAdi, kalanAdet, (alindi) => alinan = alindi));
                alinanAdet = alinan;
                kalanAdet -= alinanAdet;
                Debug.Log($"{alinanAdet} adet {urunAdi} ilk raftan al�nd�. Kalan: {kalanAdet}");
            }

            if (kalanAdet <= 0)
            {
                continue;
            }

            List<Transform> digerRaflar = rafScript.YeterliUrunBul(rafScript, urunAdi, kalanAdet);
            if (digerRaflar.Count > 0)
            {
                foreach (Transform hedefKonum in digerRaflar)
                {
                    agent.isStopped = false;                   
                    agent.updateRotation = true;
                    animator.SetFloat("Speed", 1f);
                    agent.speed = 3.5f;
                    agent.SetDestination(hedefKonum.position);

                    yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
                    yield return new WaitForSeconds(0.5f);

                    animator.SetFloat("Speed", 0f);
                    agent.speed = 0f;
                    agent.updateRotation = false;
                    Quaternion hedefRotation = hedefKonum.rotation;
                    transform.rotation = Quaternion.Euler(0, hedefRotation.eulerAngles.y, 0); // Sadece Y eksenini al�yoruz

                    Raf yeniRafScript = hedefKonum.GetComponentInParent<Raf>();
                    if (yeniRafScript != null)
                    {
                        int alinan = 0;
                        yield return StartCoroutine(UrunleriRaftanTopla(yeniRafScript, urunAdi, kalanAdet, (alindi) => alinan = alindi));
                        kalanAdet -= alinan;
                        Debug.Log($"{alinan} adet {urunAdi} yeni raftan al�nd�. Kalan: {kalanAdet}");
                    }

                    if (kalanAdet <= 0)
                        break;
                }
            }

            if (kalanAdet > 0)
            {
                Debug.LogWarning($"{urunAdi} �r�n� i�in toplamda hala {kalanAdet} adet eksik!");
            }
        }
    }

    // Raftaki �r�nleri NPC eline al�r ve raftaki sto�u d���r�r
    private IEnumerator UrunleriRaftanTopla(Raf rafScript, string urunAdi, int adetIhtiyac, Action<int> callback)
    {
        int alinanAdet = 0;

        foreach (Transform child in rafScript.transform)
        {
            foreach (Transform grandchild in child)
            {
                foreach (Transform greatGrandchild in grandchild)
                {
                    if (greatGrandchild.CompareTag(urunAdi) && alinanAdet < adetIhtiyac)
                    {
                        animator.SetTrigger("UrunAl");
                        yield return new WaitForSeconds(3f);
                        greatGrandchild.SetParent(Hand);
                        animator.SetFloat("Speed", 0f);
                        greatGrandchild.localPosition = Vector3.zero + new Vector3(0, 0.2f * alinanAdet, 0);
                        alinanAdet++;
                        Debug.Log($"{urunAdi} al�nd� ({alinanAdet}/{adetIhtiyac})");

                        yield return null; // iste�e ba�l�: her bir �r�n i�in frame beklenebilir
                        
                    }

                    if (alinanAdet >= adetIhtiyac)
                        break;
                }
                if (alinanAdet >= adetIhtiyac)
                    break;
            }
            if (alinanAdet >= adetIhtiyac)
                break;
        }

        if (rafScript.urunBilgileri.ContainsKey(urunAdi))
        {
            rafScript.urunBilgileri[urunAdi] -= alinanAdet;
        }
       
        callback?.Invoke(alinanAdet);

        StartCoroutine(KasayaGitRutini());
    }

    private IEnumerator KasayaGitRutini()
    {
        // Bo� kasa var m� kontrol et
        for (int i = 0; i < kasaNoktalari.Count; i++)
        {
            if (!doluKasaIndexleri.Contains(i))
            {
                kendiKasaIndex = i;
                doluKasaIndexleri.Add(i);
                break;
            }
        }

        if (kendiKasaIndex != -1)
        {
            Transform hedefKasa = kasaNoktalari[kendiKasaIndex];

            agent.isStopped = false;
            agent.updateRotation = true;
            animator.SetFloat("Speed", 1f);
            agent.speed = 3.5f;
            agent.SetDestination(hedefKasa.position);

            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
            animator.SetFloat("Speed", 0f);
            agent.speed = 0f;
            agent.updateRotation = false;

            Debug.Log($"NPC kasaya ula�t�: {hedefKasa.name}");

            // Kuyru�a kendini ekle
            kasaSirasi.Enqueue(this);

            // S�ras� gelene kadar bekle
            yield return new WaitUntil(() => kasaSirasi.Peek() == this);

            // S�ras� gelen ki�i �r�nleri yerle�tirir
            yield return StartCoroutine(UrunleriKasayaKoy());

            if (Dolu)
            {
                // �r�nler yerle�tirildi, s�radan ��k
                kasaSirasi.Dequeue();

                // NPC ��kabilir, kasa bo�al�r
                doluKasaIndexleri.Remove(kendiKasaIndex);
                Debug.Log($"{hedefKasa.name} art�k bo�.");

                // NPC sahneyi terk edebilir veya ��k��a gidebilir
            }

        }
        else
        {
            Debug.LogWarning("T�m kasalar dolu! NPC bekleme noktas�na gidiyor.");
            yield return StartCoroutine(BeklemeNoktalariArasiGidisGelis());
        }
    }

    private IEnumerator BeklemeNoktalariArasiGidisGelis()
    {
        int hedefIndex = 0;
        while (true)
        {
            Transform hedefNokta = beklemeNoktalari[hedefIndex];

            agent.isStopped = false;
            agent.updateRotation = true;
            animator.SetFloat("Speed", 1f);
            agent.speed = 3.5f;
            agent.SetDestination(hedefNokta.position);

            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
            animator.SetFloat("Speed", 0f);
            yield return new WaitForSeconds(5f);

            hedefIndex = (hedefIndex + 1) % beklemeNoktalari.Count;

            // Her d�ng�de kasalar bo�ald� m� kontrol edelim
            if (doluKasaIndexleri.Count < kasaNoktalari.Count)
            {
                Debug.Log("Kasada yer a��ld�, tekrar deniyorum.");
                yield return StartCoroutine(KasayaGitRutini());
                break;
            }
        }
    }

    private IEnumerator UrunleriKasayaKoy()
    {
        int i = 0;
        bool coffeIcinCiftAtla = true; // �lk sefer 2 atlayacak

        List<Transform> urunler = new List<Transform>();
        foreach (Transform child in Hand)
        {
            urunler.Add(child);
        }

        foreach (Transform urun in urunler)
        {
            if (i >= urunKoymaNoktalari.Count) break;

            Transform nokta = urunKoymaNoktalari[i];
            urun.SetParent(null);

            // Pozisyonu ayarla (�nce nokta pozisyonunu al, sonra y de�erini de�i�tirece�iz)
            Vector3 hedefPozisyon = nokta.position;

            // Varsay�lan olarak rotation'u kopyala
            urun.rotation = nokta.rotation;

            // TAG'a g�re ROTASYON + Y pozisyonu AYARI
            switch (urun.tag)
            {
                case "CableBox":
                    urun.rotation *= Quaternion.Euler(0, 0, 0);
                    hedefPozisyon.y += 0.675f;
                    break;
                case "HeadPhone":
                    urun.rotation *= Quaternion.Euler(270, 0, 180);
                    hedefPozisyon.y += 0.05f;
                    break;
                case "Speaker":
                    urun.rotation *= Quaternion.Euler(270, 270, 0);
                    hedefPozisyon.y += 0.07f;
                    break;
                case "MobilPhone":
                    urun.rotation *= Quaternion.Euler(45, 270, 0);
                    hedefPozisyon.y += 0.04f;
                    break;
                case "Gamepad":
                    urun.rotation *= Quaternion.Euler(270, 270, 0);
                    hedefPozisyon.y += 0.1f;
                    break;
                case "CoffeMachine":
                    urun.rotation *= Quaternion.Euler(0, 270, 0);
                    hedefPozisyon.y += 0.55f;
                    break;
                default:
                    // E�er tag tan�ms�zsa dokunma
                    break;
            }

            // Ayarlanm�� pozisyonu uygula
            urun.position = hedefPozisyon;

            // Listeye ekle
            kasayaKonulanUrunler.Add(urun.gameObject);

            // �ndeks art�rma mant���
            if (urun.tag == "CoffeMachine")
            {
                if (coffeIcinCiftAtla)
                {
                    i += 2;
                }
                else
                {
                    i += 1;
                }

                coffeIcinCiftAtla = !coffeIcinCiftAtla; // s�ray� de�i�tir
            }
            else
            {
                i += 1;
            }

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("NPC �r�nleri kasaya koydu.");
    }

    public void S�raDoluMu()
    {
        Dolu = true;
    }
    
     
    
}


