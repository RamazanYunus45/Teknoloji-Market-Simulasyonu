using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class NPC : MonoBehaviour
{
    /* Npc leri 2 noktadan spawn etmeye yarýyor */
    public float spawnSuresi = 60f; // Spawn süresi
    public GameObject[] npcPrefabs; // Farklý NPC prefablarýný buraya ekle
    public Transform[] spawnNoktalari; // NPC'lerin spawn olacaðý noktalar 
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
    }
}
