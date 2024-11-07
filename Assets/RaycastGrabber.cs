using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastGrabber : MonoBehaviour
{
    [SerializeField]
    private GameObject playerOrigin;
    [SerializeField]
    private LayerMask teleportMask;
    [SerializeField]
    private InputActionReference teleportButtonPress;
    [SerializeField]
    private InputActionReference dropButtonPressAction;
    [SerializeField]   
    private Transform rightHandTransform;
    [SerializeField]   
    private Transform leftHandTransform;

    //////////////////////////////////////////////////
    // prefabs

    [SerializeField]
    private GameObject prefabWall;

    //////////////////////////////////////////////////

    private bool holdingWall = false;
    private GameObject currentRightHandWeapon = null;

    void Start()
    {
        teleportButtonPress.action.performed += DoRayCast;
        dropButtonPressAction.action.performed += dropItem;
        
    }

    // Update is called once per frame
    void DoRayCast(InputAction.CallbackContext __) {
        //Debug.Log("processing");

        // The Unity raycast hit object, which will store the output of our raycast
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        // Parameters: position to start the ray, direction to project the ray, output for raycast, limit of ray length, and layer mask
        //Debug.Log(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, teleportMask));
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, teleportMask)) {

            // The object we hit will be in the collider property of our hit object.
            // We can get the name of that object by accessing it's Game Object then the name property
            
            // Don't forget to attach the player origin in the editor!
            //playerOrigin.transform.position = hit.collider.gameObject.transform.position;

            ////Debug.Log("bool check: " + holdingWeapon);
            // if currently holding weapon, destroy the weapon being held, and replace it with the new weapon
            if (holdingWall){
                DestroyCurrentWall();
                //currentRightHandWeapon.transform.parent = null;
            }

            ///if its a throwing knife , then instantiate it in both hands
            if (hit.collider.gameObject.name == "Wall") {

                ///instantiate the weapon in both hands and set the rotation
                currentRightHandWeapon = Instantiate(prefabWall, rightHandTransform.position, rightHandTransform.rotation, rightHandTransform);

                currentRightHandWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                
            }
            

            holdingWall = true;
        } 
    }

    void dropItem(InputAction.CallbackContext __) {
        //Debug.Log("processing");

        ////Debug.Log("bool check: " + holdingWeapon);
        // if currently holding weapon, destroy the weapon being held, and replace it with the new weapon
        if (holdingWall){
            //DestroyCurrentWall();
            currentRightHandWeapon.transform.parent = null;
            currentRightHandWeapon = null;
        }

        

        holdingWall = false;
        
    }
    


    /// need to destroy the current weapon before instantiating a new one
    void DestroyCurrentWall() {

        if (currentRightHandWeapon != null) 
        {
            Destroy(currentRightHandWeapon);
            currentRightHandWeapon = null;
        }

    }
    
}



