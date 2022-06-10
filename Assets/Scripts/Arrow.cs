using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Oklarýn ileri - geri hareketi
    float timeZ = 1;
    private void Update()
    {
        float randomZ = Random.Range(0.02f, -0.02f);    // Oklarýn gidip geleceði aralýk random deðer alýr
        Vector3 currentPos = transform.position;    // Oklarýn ilk pozisyonun deðerini tutar
       
        if (timeZ >= 0 && timeZ < 0.5f)
        {
            timeZ += 0.01f; // Oklar timeZ deðeri 0.05f olana kadar ilk pozisyonda kalýr
            if (timeZ == 0.5f)
                timeZ = 1;
        }
        else if (timeZ > 0.5f && timeZ <= 1f)
        {            
            currentPos.z -= randomZ;    // Random gelen deðer kadar oklar geri hareket eder
            timeZ -= 0.01f; // TimeZ deðeri her geri gidiþte süre 0.01f kýsalýr 0.5f olana kadar oklar geri hareket eder
            if (timeZ == 0.5f)
                timeZ = 0;
        }
        transform.position = currentPos;    
        // Oklarýn pozisyonu "currentPos" deðerine göre ileri - geri hareket eder
    }
}
