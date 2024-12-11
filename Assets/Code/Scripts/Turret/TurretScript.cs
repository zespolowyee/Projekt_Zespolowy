using System;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private Transform barrelObject;
    
    [SerializeField] private float maxUpRotation = -10;
    [SerializeField] private float maxDownRotation = 20;

    private RaycastHit hit;
    
    
    void Start()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var result = Physics.SphereCast(transform.position, 5f, Vector3.forward, out hit, 10f, 7);
            Debug.Log(result);
            Debug.DrawRay(transform.position, Vector3.forward * 5f, Color.red, 1f);
        }
        /*var rotationVector = new Vector3();
        if (Input.GetKeyDown(KeyCode.R))
        {
            rotationVector.x = -1;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            rotationVector.x = 1;
        }

        var currentRotation = barrelObject.eulerAngles.x;

        if (currentRotation > 180)
        {
            currentRotation -= 360;
        }

        var newRotation = Math.Round(currentRotation) + rotationVector.x;

        if (newRotation>=maxUpRotation && newRotation<=maxDownRotation)
            barrelObject.Rotate(rotationVector); */
    }
}
