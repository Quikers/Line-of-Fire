using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject GunFirePos;
    public GameObject ObjectToSpawn;
    LineRenderer LR;

    void Start()
    {
        LR = GetComponent<LineRenderer>();
    }

    void Update()
    {

        if( !Input.GetMouseButton( 0 ) )
            return;

        StartCoroutine( shoot() );

        Vector3 rayOrigin = Camera.main.ViewportToWorldPoint( new Vector3( 0.5f, 0.5f ) );

        RaycastHit hit;

        LR.SetPosition( 0, GunFirePos.transform.position );

        if( Physics.Raycast( rayOrigin, Camera.main.transform.forward, out hit, 50f ) )
        {
            LR.SetPosition( 1, hit.point );

            for (int i = 0; i < 10; i++) 
                Instantiate( ObjectToSpawn, hit.point, new Quaternion() );
        }
        else
            LR.SetPosition( 1, rayOrigin + ( Camera.main.transform.forward * 50f ) );

        
    }

    IEnumerator shoot()
    {
        LR.enabled = true;
        yield return new WaitForSeconds( 0.7f );
        LR.enabled = false;
    }
}