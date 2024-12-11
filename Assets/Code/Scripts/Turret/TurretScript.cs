using System;
using System.Linq;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private Transform barrelObject;
    
    [SerializeField]
    private Transform barrelClamp;
    
    [SerializeField]
    private Transform cannonPivot;
    
    [SerializeField] private float maxUpRotation = -10;
    [SerializeField] private float maxDownRotation = 20;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float sphereRadius = 5;

    private RaycastHit hit;
    
    
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, sphereRadius, targetLayer);
        if (targets.Length > 0){
            
            //Find the closest target
            float smallestDistance = Vector3.Distance(transform.position, targets[0].transform.position);
            Collider closestTarget = targets[0];
            foreach (Collider target in targets)
            {
                var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToTarget < smallestDistance)
                {
                    smallestDistance = distanceToTarget;
                    closestTarget = target;
                }
            }
            
            //Rotate the cannon without moving lowering or raising the barrel
            Vector3 targetPostition = new Vector3( closestTarget.transform.position.x, 
                                            transform.position.y, 
                                            closestTarget.transform.position.z ) ;
            cannonPivot.LookAt( targetPostition ) ;
            
            //Lower or raise the hidden element that is at the same position as barrel
            barrelClamp.LookAt( closestTarget.transform.position );

            //Get the rotation from the hidden element
            Vector3 clampedRotation = barrelClamp.eulerAngles;
            
            //Clamp the x rotation of barrel
            float xRotation = clampedRotation.x;
            
            if (xRotation > 180)
            {
                xRotation -= 360;
            }
            
            clampedRotation.x = Math.Clamp( xRotation, maxUpRotation, maxDownRotation );
            
            //Apply the rotation with clamped x to the barrel
            barrelObject.eulerAngles = clampedRotation;

        }
        
    }
}
