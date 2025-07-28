using System.Collections.Generic;
using UnityEngine;


public class SpiderController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private LegPair[] _legPairs;
    [SerializeField] private Transform _body;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _raycastYOffset;
    [SerializeField] private float _raycastDistance;

    [Header("Step Settings")]
    [SerializeField] private float _stepSpeed;

    [Header("Body Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationSmoothness;

    private void Start()
    {
        InitializeStepPositions();
    }

    private void LateUpdate()
    {
        UpdateBodyPosition();
        UpdateStepPositions();
    }

    

    private void UpdateStepPositions()
    {
        foreach (LegPair leg in _legPairs)
        {
            Vector3 raycastStartPosition = leg.legTracker.transform.position + Vector3.up * _raycastYOffset;
            if (Physics.Raycast(raycastStartPosition, Vector3.down, out RaycastHit hit, _raycastDistance, _layerMask))
            {
                if (Vector3.Distance(leg.lastLegPosition, hit.point) >  leg.legStepLength && leg.stepProgress >= 1f)
                {
                    leg.lastLegPosition = hit.point;
                    leg.stepProgress = 0f;
                }
            }

            if (leg.stepProgress < 1f)
            {
                leg.stepProgress += Time.deltaTime * _stepSpeed;

                leg.legTarget.position = GetArcPosition(
                    leg.legTarget.position,
                    leg.lastLegPosition,
                    leg.stepProgress,
                    leg.legStepHeight
                );
            }
        }
    }

    private Vector3 GetArcPosition(Vector3 start, Vector3 end, float progress, float height)
    {
        Vector3 pos = Vector3.Lerp(start, end, progress);
        pos.y += Mathf.Sin(progress * Mathf.PI) * height;
        return pos;
    }

    private void InitializeStepPositions()
    {
        foreach (LegPair leg in _legPairs)
        {
            Vector3 raycastStartPosition = leg.legTracker.transform.position + Vector3.up * _raycastYOffset;
            if (Physics.Raycast(raycastStartPosition, Vector3.down, out RaycastHit hit, _raycastDistance, _layerMask))
            {
                leg.legTarget.position = hit.point;
                leg.lastLegPosition = hit.point;
            }
        }
    }

    private void UpdateBodyPosition()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0)
        {
            float targetRotationY = _body.rotation.eulerAngles.y + horizontalInput * _rotationSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.Euler(_body.rotation.eulerAngles.x, targetRotationY, _body.rotation.eulerAngles.z);
            _body.rotation = Quaternion.Slerp(_body.rotation, targetRotation, _rotationSmoothness * Time.deltaTime);
        }

        if (verticalInput != 0)
        {
            Vector3 moveDirection = _body.forward * verticalInput * _moveSpeed * Time.deltaTime;
            _body.position += moveDirection;
        }

        Vector3 raycastStartPosition = _body.position + Vector3.up * _raycastYOffset;
        if (Physics.Raycast(raycastStartPosition, Vector3.down, out RaycastHit hit, _raycastDistance, _layerMask))
        {
            _body.position = new Vector3(_body.position.x, hit.point.y, _body.position.z);
        }
    }

    void OnDrawGizmos()
    {
        foreach (LegPair leg in _legPairs)
        {
            // Raycasts
            Vector3 raycastStartPosition = leg.legTracker.transform.position + Vector3.up * _raycastYOffset;
            if (Physics.Raycast(raycastStartPosition, Vector3.down, out RaycastHit hit, _raycastDistance, _layerMask))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(raycastStartPosition, hit.point);
                //legTargetPosition Offset Sphere
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(hit.point, leg.legStepLength);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(raycastStartPosition, raycastStartPosition + Vector3.down * _raycastDistance);
            }

            
        }


    }

    [System.Serializable]
    public class LegPair
    {
        public Transform legTarget;
        public Transform legTracker;
        public float legStepLength;
        public float legStepHeight;
        [HideInInspector] public Vector3 lastLegPosition;
        [HideInInspector] public float stepProgress = 1f;
    }
}
