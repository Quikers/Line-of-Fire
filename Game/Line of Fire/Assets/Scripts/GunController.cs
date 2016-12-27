using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public GameObject GunFirePos;
    public GameObject ObjectToSpawn;
    public float FireRate = .066f;
    public float FiringRange = 100f;
    public bool BulletTracing;

    private float _nextFire;
    private Vector3 _rayOrigin;

    void Update()
    {

        if( !Input.GetButton( "Fire1" ) || !( Time.time > _nextFire ) )
            return;

        _nextFire = Time.time + FireRate;


        _rayOrigin = Camera.main.ViewportToWorldPoint( new Vector3( 0.5f, 0.5f ) );

        RaycastHit hit;

        if( Physics.Raycast( _rayOrigin, Camera.main.transform.forward, out hit, FiringRange ) )
        {
            for( int i = 0; i < 10; i++ )
                Instantiate( ObjectToSpawn, hit.point, new Quaternion() );

            if( BulletTracing )
                BulletTrace( hit.point );
        }
        else
        {
            if( BulletTracing )
                BulletTrace( Vector3.down );

        }
    }

    void BulletTrace(Vector3 point)
    {
        LineRenderer LR = new GameObject(name: "Debug Line Renderer").AddComponent<LineRenderer>();

        LR.startWidth = 0.03f;
        LR.endWidth = 0.03f;

        LR.SetPosition( 0, GunFirePos.transform.position );

        LR.SetPosition( 1, point != Vector3.down ? point : _rayOrigin + ( Camera.main.transform.forward * FiringRange ) );
    }
}