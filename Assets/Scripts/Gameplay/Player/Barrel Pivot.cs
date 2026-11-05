using System;
using System.Collections;
using UnityEngine;

public class BarrelPivot : MonoBehaviour
{
    private Quaternion targetRotation;
    
    [Header("Scalable params")]
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float lerpDuration = 1f;

    // thought i might need this but might be redundant
    public void SetRotate(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    
    // proxy function to call in player class
    public void StartRotationTowards(Quaternion rotation)
    {
        targetRotation = rotation;
        StopAllCoroutines();
        StartCoroutine("RotateTowards");
    }
    
    private IEnumerator RotateTowards()
    {
        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;
        print(targetRotation.eulerAngles.z);

        Quaternion target;
        
        if (targetRotation.z == 180 || targetRotation.z == -180)
        {
           target = transform.rotation * Quaternion.Euler(0, 0, targetRotation.z);
        }
        else 
            target = transform.rotation * Quaternion.Euler(0, 0, targetRotation.z*-1);
        
        
        
        while (timeElapsed < lerpDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, target, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        // to be exact with floating point precision errors
        transform.rotation = target;
    }
}
