using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyValue : MonoBehaviour
{
    public int value;
    public Transform hedefNokta; // Paranýn gideceði yer
    public GameObject paraKopyaPrefab; // Paranýn kopyasý için prefab

    public void ParayiAnimasyonlaGötur()
    {
        StartCoroutine(ParayiKopyalaVeGötur());
    }

    private IEnumerator ParayiKopyalaVeGötur()
    {
        // Kopyasýný oluþtur
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

        // Bir süre bekle (örneðin 1 saniye) sonra yok et
        yield return new WaitForSeconds(1f);
        Destroy(kopya);
    }
}
