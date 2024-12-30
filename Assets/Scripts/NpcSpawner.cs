using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    // NPC prefab'� ve spawn noktas�
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 15f; // NPC spawn s�resi (saniye)

    private bool canSpawn = true; // Spawn kontrol�

    private void Start()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("NPC prefab atanmam��! L�tfen inspector �zerinden atay�n.");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn noktas� atanmam��! L�tfen inspector �zerinden bir nokta atay�n.");
            return;
        }

        // NPC spawn d�ng�s�n� ba�lat
        StartCoroutine(SpawnNPCsAtIntervals());
    }

    private IEnumerator SpawnNPCsAtIntervals()
    {
        while (true)
        {
            if (canSpawn)
            {
                // NPC prefab'�n� spawn noktas�nda olu�tur
                Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
                canSpawn = false; // Bir kez spawn ettikten sonra spawn'� durdur

                // 15 saniye bekle
                yield return new WaitForSeconds(spawnInterval);
                canSpawn = true; // 15 saniye sonra tekrar spawn yap�labilir
            }
            else
            {
                yield return null; // Spawn yap�lmayacaksa bir sonraki frame bekle
            }
        }
    }
}
