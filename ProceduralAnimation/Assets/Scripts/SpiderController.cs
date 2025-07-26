using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<BonePair> _boneTargetEndPair;


    [Header("Settings")]
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _boneEndRaycastDistance;
    [SerializeField] private float _groundDistanceOffset;
    [SerializeField] private float _stretchOffset;
    [SerializeField] private float _shrinkOffset;

    private void LateUpdate()
    {
        CalculateBoneTargetPosition();
    }

    private void CalculateBoneTargetPosition()
    {
        foreach (var bone in _boneTargetEndPair)
        {
            if (Physics.Raycast(bone.boneEnd.position, Vector3.down, out RaycastHit hit, _boneEndRaycastDistance, _layerMask))
            {
                float boneEndDistanceToGround = hit.point.y - bone.boneEnd.position.y;

                float calculatedBoneTargetYPosition = bone.boneTarget.position.y + boneEndDistanceToGround + _groundDistanceOffset;
                float minBoneTargetYPosition = bone.boneEnd.position.y + _stretchOffset;
                float maxBoneTargetYPosition = bone.boneEnd.position.y + _shrinkOffset;

                float boneTargetYPosition = Mathf.Clamp(calculatedBoneTargetYPosition, minBoneTargetYPosition, maxBoneTargetYPosition);

                bone.boneTarget.position = new Vector3(
                    hit.point.x,
                    boneTargetYPosition,
                    hit.point.z
                );

                bone.lastCalculatedBoneTargetPosition = bone.boneTarget.position;
            }
        }
    }
    void OnDrawGizmos()
    {
        foreach (var bone in _boneTargetEndPair)
        {
            if (Physics.Raycast(bone.boneEnd.position, Vector3.down, out RaycastHit hit, _boneEndRaycastDistance, _layerMask))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 0.3f);

            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(bone.boneEnd.position, bone.boneEnd.position + Vector3.down * _boneEndRaycastDistance);

            }
        }
    }
}


[System.Serializable]
public class BonePair
{
    public Transform boneEnd;
    public Transform boneTarget;
    public Vector3 lastCalculatedBoneTargetPosition;
}
