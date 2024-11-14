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
    private LayerMask stylesMask;
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
    private Material storedMaterial = null;

    void Start()
    {
        teleportButtonPress.action.performed += DoRayCast;
        dropButtonPressAction.action.performed += dropItem;
        
    }

    // Update is called once per frame
    void DoRayCast(InputAction.CallbackContext __){
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, teleportMask | stylesMask)){
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Styles")){
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                    //Debug.Log("renderer: " + renderer);
                if (renderer != null){
                    storedMaterial = renderer.material;
                        //Debug.Log("storedMaterial: " + storedMaterial);
                }
            } else if (hit.collider.gameObject.name == "Wall") {
                if (holdingWall){
                    DestroyCurrentWall();
                }

                currentRightHandWall = Instantiate(prefabWall, rightHandTransform.position, rightHandTransform.rotation, rightHandTransform);
                currentRightHandWall.transform.localRotation = Quaternion.Euler(0, 0, 0);

                if (storedMaterial != null){
                    Renderer wallRenderer = currentRightHandWall.GetComponent<Renderer>();
                    if (wallRenderer != null){
                        wallRenderer.material = storedMaterial;
                    }
                }

                holdingWall = true;
            } else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grabable")) {
                if (holdingWall){
                    DestroyCurrentWall();
                }

                //currentRightHandWall = hit.collider.gameObject;
            }

        }
        

    }
    

    void dropItem(InputAction.CallbackContext __) {
            //Debug.Log("dropping");

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



