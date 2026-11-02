using System;
using System.Collections;
using UnityEngine;

public class BarrelPivot : MonoBehaviour
{
    private Transform anchor;
    private Quaternion targetRotation;
    
    [Header("Scalable params")]
    [SerializeField] float turnSpeed = 10f;
    
    
    private void Awake()
    {
        anchor = transform;
    }

    public void SetRotate(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }

    public void StartRotationTowards(Quaternion rotation)
    {
        targetRotation = rotation;
        StartCoroutine("RotateTowards");
    }

    private IEnumerator RotateTowards()
    {
        // constant turn rate
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        
        yield return new WaitForEndOfFrame();
        
    }
}
