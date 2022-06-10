using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;    // Takip edilecek nesne
    [SerializeField] Vector3 offset;  // Ne kadar uzakl�ktan  takip edecek 

    [SerializeField] float lerpValue;   
    void LateUpdate()
    {     
        if (ArrowController.instance.isFinish)
        {   
            // E�er PlayerArrow Finish alan�na gelirse kamera a��s� ve uzakl��� de��ir  -- Kamera finish takip ayar�
            Vector3 desPos = target.position + new Vector3(0,8,-14);
            transform.position= Vector3.Lerp(transform.position, desPos, lerpValue*0.5f*Time.deltaTime);
        }
        else
        {
            // Kamera normal takip ayar�
            Vector3 desPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desPos, lerpValue);
        }
    }
}
