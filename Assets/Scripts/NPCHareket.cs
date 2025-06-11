using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPCHareket : MonoBehaviour
{
    /* Npc spawn olduktan sonra ilk �nce ma�azaya do�ru 2 noktaya gidiyo daha sonra rastgele bir raf konumu se�iyor ve o rafa do�ru haraket ediyor */
    private NavMeshAgent agent;
    private RafSecici rafSecici;
    private Animator animator;

    public Transform[] oncelikliNoktalar;
    public float hedefeYaklasmaMesafesi = 0.5f;
    public string hedefRaf�smi;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rafSecici = FindObjectOfType<RafSecici>();
        animator = GetComponent<Animator>();

        if (agent == null || rafSecici == null || oncelikliNoktalar.Length == 0)
        {
            Debug.LogError("Bile�enlerden biri eksik!");
            return;
        }
        StartCoroutine(HareketRutini());
    }
   
    IEnumerator HareketRutini()
    {
        // �ncelikli noktalara s�rayla git
        for (int i = 0; i < oncelikliNoktalar.Length; i++)
        {
            agent.SetDestination(oncelikliNoktalar[i].position);
            // NPC hedefe ula�ana kadar bekle
            yield return StartCoroutine(HedefeUlasanaKadarBekle());          
        }
        // Raf hedefine git
        Transform rafHedefi = rafSecici.GetRastgeleKonum();
        if (rafHedefi != null)
        {
            hedefRaf�smi = rafHedefi.parent.name;
            Debug.Log(hedefRaf�smi);
            agent.SetDestination(rafHedefi.position);
            yield return StartCoroutine(HedefeUlasanaKadarBekle());

            agent.updateRotation = false;
            Quaternion hedefRotation = rafHedefi.rotation;
            transform.rotation = Quaternion.Euler(0, hedefRotation.eulerAngles.y, 0); // Sadece Y eksenini al�yoruz

        }
        yield return new WaitForSeconds(1f);
        // Son hedefe ula��ld���nda NPC tamamen dursun   
        agent.isStopped = true;
        agent.speed = 0f;
        animator.SetFloat("Speed", 0f);      
    }

    IEnumerator HedefeUlasanaKadarBekle()
    {
        // NPC hedefe tamamen ula�ana kadar bekle
        while (agent.pathPending || agent.remainingDistance > hedefeYaklasmaMesafesi)
        {
            yield return null;
        }
    }
}

