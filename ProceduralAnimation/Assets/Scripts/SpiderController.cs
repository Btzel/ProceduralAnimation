using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SpiderController : MonoBehaviour
{   
    [Header("Settings")]
    [SerializeField] private float _raycastYOffset;
    [SerializeField] private float _raycastMaxDistance;
    [SerializeField] private LayerMask _raycastLayerMask;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _alignSmoothSpeed;

    private void Update()
    {
        UpdateSpiderRotation();
        UpdateSpiderPosition();
    }

    private void UpdateSpiderRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float rotationAmount = horizontalInput * _rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
        }
        Vector3 raycastStartPos = transform.position + transform.up * _raycastYOffset;
        Vector3 raycastDirection = -transform.up;
        if (Physics.Raycast(raycastStartPos, raycastDirection, out RaycastHit hit, _raycastMaxDistance, _raycastLayerMask))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _alignSmoothSpeed);
        }
        
    }

    private void UpdateSpiderPosition()
    {
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            Vector3 forwardMovement = transform.forward * verticalInput * _movementSpeed * Time.deltaTime;
            transform.position += forwardMovement;
        }
        
        Vector3 raycastStartPos = transform.position + transform.up * _raycastYOffset;
        Vector3 raycastDirection = -transform.up;

        if (Physics.Raycast(raycastStartPos, raycastDirection, out RaycastHit hit, _raycastMaxDistance, _raycastLayerMask))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);

        }
    }

    void OnDrawGizmos()
    {
        Vector3 raycastStartPos = transform.position + transform.up * _raycastYOffset;
        Vector3 raycastDirection = -transform.up;
        if (Physics.Raycast(raycastStartPos, raycastDirection, out RaycastHit hit, _raycastMaxDistance, _raycastLayerMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(raycastStartPos, hit.point);
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(raycastStartPos, raycastStartPos + Vector3.down * _raycastMaxDistance);
        }
    }
}
