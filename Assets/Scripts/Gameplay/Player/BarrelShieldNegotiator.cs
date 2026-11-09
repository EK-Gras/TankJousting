using System.Collections;
using Gameplay;
using UnityEngine;

public class BarrelShieldNegotiator : MonoBehaviour
{
    [SerializeField] private float shieldDelay = .4f;
    public Barrel barrelToDisable;
    
    public void RunShieldProtocol()
    {
        StartCoroutine("ShieldHitDisable");
    }

    // turns off barrel when hitting shiel
    private IEnumerator ShieldHitDisable()
    {
        barrelToDisable.enabled = false;
        print("disabling barrel");
        yield return new WaitForSeconds(shieldDelay);
        print("delay over");
        barrelToDisable.enabled = true;
    }
}
