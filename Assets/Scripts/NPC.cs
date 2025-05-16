using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class NPC : MonoBehaviour
{

    /* Npc leri 2 noktadan spawn etmeye yarýyor */

    public GameObject[] npcPrefabs; // Farklý NPC prefablarýný buraya ekle
    public Transform[] spawnNoktalari; // NPC'lerin spawn olacaðý noktalar
    public float spawnSuresi = 60f; // Spawn süresi

    private RafSecici rafKontrol; // Raflarý kontrol eden script
    private OpenClose Sign;

    void Start()
    {
        rafKontrol = FindObjectOfType<RafSecici>(); // Sahnedeki RafKontrol scriptini bul

        if (npcPrefabs.Length == 0 || spawnNoktalari.Length == 0)
        {
            Debug.LogError("NPC prefablarý veya spawn noktalarý eksik! Lütfen Inspector'da kontrol et.");
            return;
        }

        Sign = FindObjectOfType<OpenClose>();

        Debug.Log("NPCSpawner çalýþýyor! Spawn iþlemi baþlayacak...");
        StartCoroutine(NPCSpawnRutini());
    }

    IEnumerator NPCSpawnRutini()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnSuresi);

            //  Sadece SpawnetmeDurumu true ise çalýþ
            if (Sign != null && Sign.SpawnetmeDurumu)
            {
                NPCSpawnEt();
            }
            else
            {
                Debug.Log("Spawn iþlemi devre dýþý.");
            }
        }
    }

    void NPCSpawnEt()
    {
        if (npcPrefabs.Length == 0 || spawnNoktalari.Length == 0)
        {
            Debug.LogError("NPC prefablarý veya spawn noktalarý eksik!");
            return;
        }

        if (rafKontrol != null)
        {
            rafKontrol.RaflariKontrolEt();
        }
        else
        {
            Debug.LogError("RafKontrol scripti bulunamadý!");
            return;
        }

        int randomNPCIndex = Random.Range(0, npcPrefabs.Length);
        int randomSpawnIndex = Random.Range(0, spawnNoktalari.Length);

        GameObject yeniNPC = Instantiate(npcPrefabs[randomNPCIndex], spawnNoktalari[randomSpawnIndex].position, Quaternion.identity);

        // **NavMeshAgent bileþeni var mý kontrol et, yoksa ekle**
        //if (yeniNPC.GetComponent<NavMeshAgent>() == null)
        //{
        //    yeniNPC.AddComponent<NavMeshAgent>();
        //    Debug.LogWarning($"NPC {yeniNPC.name} için eksik NavMeshAgent eklendi.");
        //}

        //Debug.Log($"NPC Spawn Edildi: {yeniNPC.name} - Konum: {spawnNoktalari[randomSpawnIndex].position}");

        //NPCyiKonumaGonder(yeniNPC);
    }

    //void NPCyiKonumaGonder(GameObject npc)
    //{
    //    Transform[] uygunKonumlar = rafKontrol.GetUygunKonumlar(); // Liste yerine dizi kullanýldý

    //    if (uygunKonumlar.Length == 0)
    //    {
    //        Debug.LogWarning("Uygun konum bulunamadý! NPC hareket etmeyecek.");
    //        return;
    //    }

    //    Transform hedefTransform = uygunKonumlar[Random.Range(0, uygunKonumlar.Length)];

    //    // NPC'nin hareket scriptini al ve hedefe gitmesini söyle
    //    NPCHareket hareketScripti = npc.GetComponent<NPCHareket>();
    //    if (hareketScripti != null)
    //    {
    //        hareketScripti.HedefeGit(hedefTransform);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("NPC Hareket scripti bulunamadý! NPC ýþýnlanýyor.");
    //        npc.transform.position = hedefTransform.position; // Eðer hareket scripti yoksa direkt ýþýnla
    //    }

    //    Debug.Log($"NPC {npc.name} þu konuma yürüyor: {hedefTransform.position}");
    //}
}
