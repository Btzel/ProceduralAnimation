using UnityEngine;

public class SpiderGait : MonoBehaviour
{
    [Header("Leg Gait Settings")]
    [SerializeField] private float _stepDistance;
    [SerializeField] private float _randomAngleRange;
    [SerializeField] private float _stepHeight;
    [SerializeField] private float _stepSpeed;

    [Header("Raycast Settings")]
    [SerializeField] private float _raycastYOffset;
    [SerializeField] private float _raycastMaxDistance;
    [SerializeField] private LayerMask _raycastLayerMask;
    [Header("References")]
    [SerializeField] public SpiderLeg[] _spiderLeg;


    private void Update()
    {
        CalculateSpiderLegTargetPositions();
        UpdateSpiderLegAnimations();
    }

    private void CalculateSpiderLegTargetPositions()
    {
        foreach (SpiderLeg spiderLeg in _spiderLeg)
        {
            if (spiderLeg.isMoving) continue;
            float distanceToTarget = Vector3.Distance(spiderLeg.spiderLegEnd.position, spiderLeg.spiderLegTarget.position);
            if (distanceToTarget > _stepDistance)
            {
                Vector3 targetToEnd = (spiderLeg.spiderLegEnd.position - spiderLeg.spiderLegTarget.position).normalized;
 
                
                Vector3 newTargetPos = spiderLeg.spiderLegEnd.position + targetToEnd * (_stepDistance/2);


                Vector3 raycastStartPos = spiderLeg.spiderLegEnd.position + transform.up * _raycastYOffset;
                Vector3 raycastDirection = -transform.up;
                if (Physics.Raycast(raycastStartPos, raycastDirection, out RaycastHit hit, _raycastMaxDistance, _raycastLayerMask))
                {
                    spiderLeg.startPosition = spiderLeg.spiderLegTarget.position;
                    spiderLeg.targetPosition = hit.point;
                    spiderLeg.isMoving = true;
                    spiderLeg.moveStartTime = Time.time;
                }
            }
            
        }
    }

    private void UpdateSpiderLegAnimations()
    {
        foreach (SpiderLeg spiderLeg in _spiderLeg)
        {
            if (spiderLeg.isMoving)
            {
                float moveProgress = (Time.time - spiderLeg.moveStartTime) * _stepSpeed;
                if (moveProgress >= 1f)
                {
                    spiderLeg.spiderLegTarget.position = spiderLeg.targetPosition;
                    spiderLeg.isMoving = false;
                }
                else
                {
                    Vector3 currentPos = Vector3.Lerp(
                        spiderLeg.startPosition,
                        spiderLeg.targetPosition,
                        moveProgress
                    );

                    float heightOffset = 4 * _stepHeight * moveProgress * (1 - moveProgress);
                    currentPos.y += heightOffset;

                    spiderLeg.spiderLegTarget.position = currentPos;
                }
            }
        }
    }


    private void OnDrawGizmos()
    {
        foreach (SpiderLeg spiderLeg in _spiderLeg)
        {
            Vector3 raycastStartPos = spiderLeg.spiderLegEnd.position + transform.up * _raycastYOffset;
            Vector3 raycastDirection = -transform.up;
            if (Physics.Raycast(raycastStartPos, raycastDirection, out RaycastHit hit, _raycastMaxDistance, _raycastLayerMask))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(raycastStartPos, hit.point);
                Gizmos.DrawSphere(hit.point, _stepDistance);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(raycastStartPos, raycastStartPos + raycastDirection * _raycastMaxDistance);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(spiderLeg.spiderLegEnd.position, spiderLeg.spiderLegTarget.position);

        }
    }

    [System.Serializable]
    public class SpiderLeg
    {
        public Transform spiderLegTarget;
        public Transform spiderLegEnd;

        public bool isMoving;
        public Vector3 startPosition;
        public Vector3 targetPosition;
        public float moveStartTime;
    }
}
