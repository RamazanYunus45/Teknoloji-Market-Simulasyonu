using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class NPC : MonoBehaviour
{
    /* Npc leri 2 noktadan spawn etmeye yar�yor */
    public float spawnSuresi = 60f; // Spawn s�resi
    public GameObject[] npcPrefabs; // Farkl� NPC prefablar�n� buraya ekle
    public Transform[] spawnNoktalari; // NPC'lerin spawn olaca�� noktalar 
    private RafSecici rafKontrol; // Raflar� kontrol eden script
    private OpenClose Sign;

    void Start()
    {
        rafKontrol = FindObjectOfType<RafSecici>(); // Sahnedeki RafKontrol scriptini bul

        if (npcPrefabs.Length == 0 || spawnNoktalari.Length == 0)
        {
            Debug.LogError("NPC prefablar� veya spawn noktalar� eksik! L�tfen Inspector'da kontrol et.");
            return;
        }

        Sign = FindObjectOfType<OpenClose>();

        Debug.Log("NPCSpawner �al���yor! Spawn i�lemi ba�layacak...");
        StartCoroutine(NPCSpawnRutini());
    }

    IEnumerator NPCSpawnRutini()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnSuresi);

            //  Sadece SpawnetmeDurumu true ise �al��
            if (Sign != null && Sign.SpawnetmeDurumu)
            {
                NPCSpawnEt();
            }
            else
            {
                Debug.Log("Spawn i�lemi devre d���.");
            }
        }
    }

    void NPCSpawnEt()
    {
        if (npcPrefabs.Length == 0 || spawnNoktalari.Length == 0)
        {
            Debug.LogError("NPC prefablar� veya spawn noktalar� eksik!");
            return;
        }
        if (rafKontrol != null)
        {
            rafKontrol.RaflariKontrolEt();
        }
        else
        {
            Debug.LogError("RafKontrol scripti bulunamad�!");
            return;
        }

        int randomNPCIndex = Random.Range(0, npcPrefabs.Length);
        int randomSpawnIndex = Random.Range(0, spawnNoktalari.Length);

        GameObject yeniNPC = Instantiate(npcPrefabs[randomNPCIndex], spawnNoktalari[randomSpawnIndex].position, Quaternion.identity);        
    }
}
