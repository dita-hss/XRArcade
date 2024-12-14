using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhippedCreamPlacer : MonoBehaviour
{
    [SerializeField]
    private LayerMask placementMask;
    [SerializeField]
    private InputActionReference whippedCreamButtonPress;
    [SerializeField]
    private GameObject prefabWhippedCream;

    private Coroutine placingCoroutine;

    void Start()
    {
        whippedCreamButtonPress.action.performed += (_) => StartPlacingWhippedCream();
        whippedCreamButtonPress.action.canceled += (_) => StopPlacingWhippedCream();
    }

    void StartPlacingWhippedCream()
    {
        if (placingCoroutine == null)
        {
            placingCoroutine = StartCoroutine(PlaceWhippedCreamContinuously());
        }
    }

    void StopPlacingWhippedCream()
    {
        if (placingCoroutine != null)
        {
            StopCoroutine(placingCoroutine);
            placingCoroutine = null;
        }
    }

    IEnumerator PlaceWhippedCreamContinuously()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, placementMask))
            {
                Vector3 spawnPosition = hit.point;
                GameObject whippedCream = Instantiate(prefabWhippedCream, spawnPosition, Quaternion.identity);
                whippedCream.transform.localScale = whippedCream.transform.localScale * 0.5f;
            }

            yield return new WaitForSeconds(0.1f); // Delay between placing whipped cream instances
        }
    }
}


