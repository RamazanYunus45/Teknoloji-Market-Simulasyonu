using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyValue : MonoBehaviour
{
    public int value;
    public Transform hedefNokta; // Paran�n gidece�i yer
    public GameObject paraKopyaPrefab; // Paran�n kopyas� i�in prefab

    public void ParayiAnimasyonlaG�tur()
    {
        StartCoroutine(ParayiKopyalaVeG�tur());
    }

    private IEnumerator ParayiKopyalaVeG�tur()
    {
        // Kopyas�n� olu�tur
        GameObject kopya = Instantiate(paraKopyaPrefab, transform.position, transform.rotation);

        float sure = 0.5f;
        float zaman = 0f;
        Vector3 baslangic = kopya.transform.position;
        Vector3 hedef = hedefNokta.position;

        while (zaman < sure)
        {
            kopya.transform.position = Vector3.Lerp(baslangic, hedef, zaman / sure);
            zaman += Time.deltaTime;
            yield return null;
        }

        kopya.transform.position = hedef;

        // Bir s�re bekle (�rne�in 1 saniye) sonra yok et
        yield return new WaitForSeconds(1f);
        Destroy(kopya);
    }
}
