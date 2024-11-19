using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastGrabber : MonoBehaviour
{
    [SerializeField]
    private GameObject playerOrigin;
    [SerializeField]
    private LayerMask grabableMask;
    [SerializeField]
    private LayerMask stylesMask;
    [SerializeField]
    private LayerMask toppingsMask;
    [SerializeField]
    private InputActionReference teleportButtonPress;
    [SerializeField]
    private InputActionReference dropButtonPressAction;
    [SerializeField]
    private InputActionReference highlightButtonPressAction;
    //////////////////////////////////////////////////
    [SerializeField]
    private InputActionReference scaleUpFromTop;
    [SerializeField]
    private InputActionReference scaleDownFromTop;
    [SerializeField]
    private InputActionReference scaleUpFromSides;
    [SerializeField]
    private InputActionReference scaleDownFromSides;

    private bool isScalingUpFromTop = false;
    private bool isScalingDownFromTop = false;
    private bool isScalingUpFromSides = false;
    private bool isScalingDownFromSides = false;

    //////////////////////////////////////////////////
    [SerializeField]   
    private Transform rightHandTransform;
    [SerializeField]   
    private Transform leftHandTransform;

    //////////////////////////////////////////////////
    // prefabs

    [SerializeField]
    private GameObject prefabWall;

    [SerializeField]
    private GameObject prefabCandycane;
    //////////////////////////////////////////////////

    private bool holdingWall = false;
    private bool holdingTopping = false;
    private GameObject currentRightHandWall = null;
    private Material storedMaterial = null;

    private GameObject currentlyHighlightedObject = null;
    private Material originalMaterial = null;
    [SerializeField]
    private Material highlightMaterial;

    private Vector2 joystickInput;
    
    void Start()
    {
        teleportButtonPress.action.performed += DoRayCast;
        dropButtonPressAction.action.performed += dropItem;
        highlightButtonPressAction.action.performed += HighlightObject;

        // for scaling
        scaleUpFromTop.action.performed += (_) => isScalingUpFromTop = true;
        scaleUpFromTop.action.canceled += (_) => isScalingUpFromTop = false;

        scaleDownFromTop.action.performed += (_) => isScalingDownFromTop = true;
        scaleDownFromTop.action.canceled += (_) => isScalingDownFromTop = false;

        scaleUpFromSides.action.performed += (_) => isScalingUpFromSides = true;
        scaleUpFromSides.action.canceled += (_) => isScalingUpFromSides = false;

        scaleDownFromSides.action.performed += (_) => isScalingDownFromSides = true;
        scaleDownFromSides.action.canceled += (_) => isScalingDownFromSides = false;
    }

    void DoRayCast(InputAction.CallbackContext __){
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, grabableMask | stylesMask | toppingsMask)){
            
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Styles")){
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null){
                    storedMaterial = renderer.material;
                }
            } else if (hit.collider.gameObject.name == "Wall") {
                Debug.Log(hit.collider.gameObject.name);
                if (holdingWall){
                    DestroyCurrentWall();
                }

                Vector3 spawnPosition = rightHandTransform.position + rightHandTransform.forward * 0.75f;
                currentRightHandWall = Instantiate(prefabWall, spawnPosition, rightHandTransform.rotation, rightHandTransform);
                currentRightHandWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                currentRightHandWall.transform.localScale = currentRightHandWall.transform.localScale * 0.50f;

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
                currentRightHandWall = hit.collider.gameObject;
                currentRightHandWall.transform.SetParent(rightHandTransform);
                currentRightHandWall.transform.localPosition = new Vector3(0, 0, 0.75f);
                currentRightHandWall.transform.localRotation = Quaternion.identity;
                holdingWall = true;
            } else if ((hit.collider.gameObject.name == "candycane") && (hit.collider.gameObject.tag == "Original")) {
                if (holdingTopping){
                    DestroyCurrentWall();
                }
                Debug.Log(hit.collider.gameObject.name);
                Debug.Log(hit.collider.gameObject.tag);
                Vector3 spawnPosition = rightHandTransform.position + rightHandTransform.forward * 0.75f;
                currentRightHandWall = Instantiate(prefabCandycane, spawnPosition, rightHandTransform.rotation, rightHandTransform);
                currentRightHandWall.transform.localRotation = Quaternion.Euler(0, 0, 0);
                currentRightHandWall.transform.localScale = currentRightHandWall.transform.localScale * 0.50f;

                holdingTopping = true;

            } else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Toppings")){
                Debug.Log(hit.collider.gameObject.name);
                Debug.Log(hit.collider.gameObject.tag);

                if (holdingTopping){
                    DestroyCurrentWall();
                }
                currentRightHandWall = hit.collider.gameObject;
                currentRightHandWall.transform.SetParent(rightHandTransform);
                currentRightHandWall.transform.localPosition = new Vector3(0, 0, 0.75f);
                currentRightHandWall.transform.localRotation = Quaternion.identity;

                holdingTopping = true;
            }
        }
    }

    void HighlightObject(InputAction.CallbackContext __) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, stylesMask | grabableMask | toppingsMask)) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grabable")|| hit.collider.gameObject.layer == LayerMask.NameToLayer("Toppings")) {
                GameObject hitObject = hit.collider.gameObject;

                // reset previous highlight
                if (currentlyHighlightedObject != null && currentlyHighlightedObject != hitObject) {
                    ResetHighlight();
                }

                // highlight the new object
                Renderer renderer = hitObject.GetComponent<Renderer>();
                if (renderer != null) {
                    if (currentlyHighlightedObject != hitObject) {
                        originalMaterial = renderer.material;
                        renderer.material = highlightMaterial;
                        currentlyHighlightedObject = hitObject;
                    }
                }
            } else {
                ResetHighlight();
            }
        } else {
            ResetHighlight();
        }
    }
      
    void ResetHighlight() {
        Debug.Log("Resetting highlight");
        if (currentlyHighlightedObject != null) {
            Renderer renderer = currentlyHighlightedObject.GetComponent<Renderer>();
            if (renderer != null) {
                renderer.material = originalMaterial;
            }
            currentlyHighlightedObject = null;
            originalMaterial = null;
        }
    }

    void Update()
    {
        if (isScalingUpFromTop)
        {
            ScaleObject(Vector3.up * 0.2f * Time.deltaTime);
        }
        if (isScalingDownFromTop)
        {
            ScaleObject(Vector3.down * 0.2f * Time.deltaTime);
        }
        if (isScalingUpFromSides)
        {
            ScaleObject(Vector3.right * 0.2f * Time.deltaTime);
        }
        if (isScalingDownFromSides)
        {
            ScaleObject(Vector3.left * 0.2f * Time.deltaTime);
        }
    }


    void ScaleObject(Vector3 scaleChange)
    {
        if (currentlyHighlightedObject == null) return;

        Vector3 currentScale = currentlyHighlightedObject.transform.localScale;
        Vector3 newScale = currentScale + scaleChange;

        currentlyHighlightedObject.transform.localScale = newScale;
    }


    void dropItem(InputAction.CallbackContext __) {
        if (holdingWall){
            currentRightHandWall.transform.parent = null;
            currentRightHandWall = null;
            holdingWall = false;
        }

        if (holdingTopping) {
            // drop the topping
            currentRightHandWall.transform.parent = null;
            currentRightHandWall = null;
            holdingTopping = false;
        }
    }

    void DestroyCurrentWall() {
        if (currentRightHandWall != null) 
        {
            Destroy(currentRightHandWall);
            currentRightHandWall = null;
        }
    }
}
