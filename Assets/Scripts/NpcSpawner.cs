using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    // NPC prefab'ý ve spawn noktasý
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 15f; // NPC spawn süresi (saniye)

    private bool canSpawn = true; // Spawn kontrolü

    private void Start()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("NPC prefab atanmamýþ! Lütfen inspector üzerinden atayýn.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn noktasý atanmamýþ! Lütfen inspector üzerinden bir nokta atayýn.");
            return;
        }

        // NPC spawn döngüsünü baþlat
        StartCoroutine(SpawnNPCsAtIntervals());
    }

    private IEnumerator SpawnNPCsAtIntervals()
    {
        while (true)
        {
            if (canSpawn)
            {
                // NPC prefab'ýný spawn noktasýnda oluþtur
                Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
                canSpawn = false; // Bir kez spawn ettikten sonra spawn'ý durdur

                // 15 saniye bekle
                yield return new WaitForSeconds(spawnInterval);
                canSpawn = true; // 15 saniye sonra tekrar spawn yapýlabilir
            }
            else
            {
                yield return null; // Spawn yapýlmayacaksa bir sonraki frame bekle
            }
        }
    }
}
