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
    private GameObject currentRightHandWall = null;
    private GameObject materialObject = null;

    void Start()
    {
        teleportButtonPress.action.performed += DoRayCast;
        dropButtonPressAction.action.performed += dropItem;
        
    }

    // Update is called once per frame
    void DoRayCast(InputAction.CallbackContext __) {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, teleportMask)) {

           
            if (holdingWall){
                DestroyCurrentWall();
            }

            if (hit.collider.gameObject.name == "Wall") {

                currentRightHandWall = Instantiate(prefabWall, rightHandTransform.position, rightHandTransform.rotation, rightHandTransform);
                currentRightHandWall.transform.localRotation = Quaternion.Euler(0, 0, 0);

                // if the object has a material, change the mateiral of the currentRightHandWall to it

                /// NOT TESTED -- TEST TEST TEST TEST TEST TEST TEST TEST
                if (materialObject != null) {
                    currentRightHandWall.GetComponent<MeshRenderer>().material = materialObject.GetComponent<MeshRenderer>().material;
                }
                
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
            currentRightHandWall.transform.parent = null;
            currentRightHandWall = null;
        }

        

        holdingWall = false;
        
    }
    


    /// need to destroy the current weapon before instantiating a new one
    void DestroyCurrentWall() {

        if (currentRightHandWall != null) 
        {
            Destroy(currentRightHandWall);
            currentRightHandWall = null;
        }

    }
    
}



