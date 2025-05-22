using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;


public class NpcNavMesh : MonoBehaviour
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
    public List<GameObject> kasayaKonulanUrunler = new List<GameObject>();

    private static Queue<NpcNavMesh> kasaSirasi = new Queue<NpcNavMesh>();
    private int kendiKasaIndex = -1;

    public bool Dolu = false;

    public GameObject MoneyStack;

    private float toplamFiyat = 0f;
    private float npcPara = 0f;
    private Camera CheckoutCam;
    private  int scanCount = 0;
    private TextMeshProUGUI Rece�ved_Text;
    private TextMeshProUGUI Total_Text;
    private TextMeshProUGUI Change_Text;
    private TextMeshProUGUI G�ve_Text;
    private TextMeshProUGUI BakiyeText;

    private bool MoneyBas�ld�;
    private Collider NpcCollider;

    int verilenPara = 0;
    bool paraUstuDogru = false;
    private Animation animasyon;

    public Transform[] cikisNoktalari;

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

         CheckoutCam = GameObject.Find("KasaCamera").GetComponent<Camera>();

        Rece�ved_Text = GameObject.Find("Rece�ved_Text").GetComponent<TextMeshProUGUI>();
        Total_Text = GameObject.Find("Total_Text").GetComponent<TextMeshProUGUI>();
        Change_Text = GameObject.Find("Change_Text").GetComponent<TextMeshProUGUI>();
        G�ve_Text = GameObject.Find("G�ve_Text").GetComponent<TextMeshProUGUI>();
        BakiyeText = GameObject.Find("Bakiye_Text").GetComponent<TextMeshProUGUI>();
        NpcCollider = GetComponent<Collider>();

        animasyon = GameObject.Find("�ekmece").GetComponent<Animation>();
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
        yield return StartCoroutine(KasayaGitRutini());
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

      //  StartCoroutine(KasayaGitRutini());
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

            yield return new WaitUntil(() => kasaSirasi.Peek() == this);

            // S�ras� gelen ki�i �r�nleri yerle�tirir
            yield return StartCoroutine(UrunleriKasayaKoy());
            
           

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

    public IEnumerator UrunleriKasayaKoy()
    {
        transform.rotation = Quaternion.Euler(0, 180, 0);

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
                    hedefPozisyon.x += -0.2f;
                    hedefPozisyon.z += -0.2f;
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

        npcUrunBilgileri.Clear();
        scanCount = 0;
        Rece�ved_Text.text = 0.ToString();
        Total_Text.text = 0.ToString();
        Change_Text.text = 0.ToString();
        G�ve_Text.text = 0.ToString();
        verilenPara = 0;
        toplamFiyat = 0;
        npcPara = 0;
        MoneyBas�ld� = false;
        paraUstuDogru = false;
        urunSecildi = false;
    }
  

    //public void S�raDoluMu()
   // {
    //    Dolu = true;
    //}

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = CheckoutCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                
                   
                
                if (hit.collider.CompareTag("Money")) // NPC para verir
                {
                    MoneyVerildi(hit);
                }
                else if (hit.collider.CompareTag("MoneyValue")) // Oyuncu para �st� verir
                {
                    ParaUstuIslemi(hit);
                }
                ScanUrun(hit);
            }
        }

        if (paraUstuDogru && Input.GetKeyDown(KeyCode.Space))
        {
            float bakiye = float.Parse(BakiyeText.text);
            float toplam = bakiye + toplamFiyat;
            BakiyeText.text = toplam.ToString();
            Debug.LogWarning("��lem tamamland�. NPC ��kabilir.");
            Dolu = true;
            NpcCikisiniBaslat();
           
            kasaSirasi.Dequeue();
            doluKasaIndexleri.Remove(kendiKasaIndex);
            kendiKasaIndex = -1;
            if (kasaSirasi.Count > 0)
            {
                NpcNavMesh siradaki = kasaSirasi.Peek();
                siradaki.StartCoroutine(siradaki.KasayaGitRutini());
            }
            animasyon.Play("MoneyClose");
            Rece�ved_Text.text = 0.ToString();
            Total_Text.text = 0.ToString();
            Change_Text.text = 0.ToString();
            G�ve_Text.text = 0.ToString();
            verilenPara = 0;         
            toplamFiyat = 0;
            scanCount = 0;
            npcPara = 0;
            MoneyBas�ld� = false;
            paraUstuDogru = false;
            MoneyStack.SetActive(false);
            kasayaKonulanUrunler.Clear();
           
            // NPC ��k�� i�lemleri vs...
        }
    }

    public void ScanUrun(RaycastHit hit)
    {
       
        UrunBilgi urun = hit.collider.GetComponent<UrunBilgi>();
        if (urun != null)
        {
            urun.Scan();
            toplamFiyat += urun.fiyat;
            scanCount++;
        }
        Debug.Log($"Kontrol: scanCount = {scanCount}, kasayaKonulanUrunler.Count = {kasayaKonulanUrunler.Count}");
        if (scanCount == kasayaKonulanUrunler.Count && kasayaKonulanUrunler.Count > 0)
        {
            if (!MoneyBas�ld�)
            {
                NpcCollider.enabled = false;
                MoneyStack.SetActive(true);
                animator.SetTrigger("ParaUzat");
                MoneyBas�ld� = true;
            }
        }
    }

    public void MoneyVerildi(RaycastHit hit)
    {
        if (MoneyBas�ld�)
        {
            HesaplaNpcPara();
            MoneyStack.SetActive(false);
            animator.SetTrigger("�dleGec");
            NpcCollider.enabled = true;
            animasyon.Play("MoneyOpen");
            Debug.Log("T�m �r�nler tarand�.");
        }
    }

    public void HesaplaNpcPara()
    {

        int ekstraPara = UnityEngine.Random.Range(1, 51);
        npcPara = toplamFiyat + ekstraPara;

        Rece�ved_Text.text = npcPara.ToString();
        Total_Text.text = toplamFiyat.ToString();
        Change_Text.text = (npcPara - toplamFiyat).ToString();
        



    }

    public void ParaUstuIslemi(RaycastHit hit)
    {
        MoneyValue money = hit.collider.GetComponent<MoneyValue>();
        if (money != null)
        {
            verilenPara += money.value;
            Debug.Log("Verilen toplam para: " + verilenPara);
            G�ve_Text.text = verilenPara.ToString();
            money.ParayiAnimasyonlaG�tur();
        }

        if (!paraUstuDogru && npcPara > 0 && verilenPara == (npcPara - toplamFiyat))
        {
            paraUstuDogru = true;
            Debug.LogWarning("Do�ru para �st� verildi!");
        }
    }

    void NpcCikisiniBaslat()
    {
        if (cikisNoktalari.Length == 0)
        {
            Debug.LogError("��k�� noktas� tan�ml� de�il!");
            return;
        }

        int rastgeleIndex = UnityEngine.Random.Range(0, cikisNoktalari.Length);
        Transform hedefNokta = cikisNoktalari[rastgeleIndex];

        if (agent != null && hedefNokta != null)
        {
            agent.isStopped = false;
            agent.updateRotation = true;
            animator.SetFloat("Speed", 1f);
            agent.speed = 3.5f;
            agent.SetDestination(hedefNokta.position);          
            Debug.Log("NPC ��k�� noktas�na y�nlendirildi.");
        }
        kasayaKonulanUrunler.Clear();

    }

    

}


