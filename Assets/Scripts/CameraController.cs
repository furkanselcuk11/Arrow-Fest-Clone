using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;    // Takip edilecek nesne
    [SerializeField] Vector3 offset;  // Ne kadar uzaklýktan  takip edecek 

    [SerializeField] float lerpValue;   
    void LateUpdate()
    {     
        if (ArrowController.instance.isFinish)
        {   
            // Eðer PlayerArrow Finish alanýna gelirse kamera açýsý ve uzaklýðý deðþir  -- Kamera finish takip ayarý
            Vector3 desPos = target.position + new Vector3(0,8,-14);
            transform.position= Vector3.Lerp(transform.position, desPos, lerpValue*0.5f*Time.deltaTime);
        }
        else
        {
            // Kamera normal takip ayarý
            Vector3 desPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desPos, lerpValue);
        }
    }
}
