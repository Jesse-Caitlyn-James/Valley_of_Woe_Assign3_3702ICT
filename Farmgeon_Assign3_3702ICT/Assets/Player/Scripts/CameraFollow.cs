using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

// Separating camera rotation from the character ensures smooth movement by avoiding conflicts 
// between their motions. LateUpdate is used for the camera to follow the character smoothly, updating 
// after all movements to prevent stuttering, especially when using a CharacterController
public class CameraFollow : MonoBehaviour
{
    public string _target = "CameraTarget";

    // Treat this target as camera when working on your crouching mechanic, don't edit this script.
    private Transform cameraTarget;

    private void Start()
    {
        try
        {
            cameraTarget = GameObject.FindGameObjectWithTag(_target).transform;
        }
        catch
        {
            Debug.Log("Create an empty GameObject and give it the tag " + _target + " to enable the camera rotation script");
            GetComponent<CameraFollow>().enabled = false;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = cameraTarget.rotation;
        transform.position = cameraTarget.position;
    }
}
