using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Oklar�n ileri - geri hareketi
    float timeZ = 1;
    private void Update()
    {
        float randomZ = Random.Range(0.02f, -0.02f);    // Oklar�n gidip gelece�i aral�k random de�er al�r
        Vector3 currentPos = transform.position;    // Oklar�n ilk pozisyonun de�erini tutar
       
        if (timeZ >= 0 && timeZ < 0.5f)
        {
            timeZ += 0.01f; // Oklar timeZ de�eri 0.05f olana kadar ilk pozisyonda kal�r
            if (timeZ == 0.5f)
                timeZ = 1;
        }
        else if (timeZ > 0.5f && timeZ <= 1f)
        {            
            currentPos.z -= randomZ;    // Random gelen de�er kadar oklar geri hareket eder
            timeZ -= 0.01f; // TimeZ de�eri her geri gidi�te s�re 0.01f k�sal�r 0.5f olana kadar oklar geri hareket eder
            if (timeZ == 0.5f)
                timeZ = 0;
        }
        transform.position = currentPos;    
        // Oklar�n pozisyonu "currentPos" de�erine g�re ileri - geri hareket eder
    }
}
