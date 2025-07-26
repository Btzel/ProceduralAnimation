using UnityEngine;

public class GeckoController : MonoBehaviour
{
    [SerializeField] Transform _target;

    [SerializeField] Transform _headBone;
    [SerializeField] float _headBoneRotationSpeed;
    [SerializeField] float _headMaxTurnAngle;

    void LateUpdate()
    {
        HeadTrackingUpdate();        
    }

    void HeadTrackingUpdate()
    {
        Quaternion currentLocalRotation = _headBone.localRotation;
        _headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = _target.position - _headBone.position;
        Vector3 targetLocalLookDir = _headBone.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(
            Vector3.forward,
            targetLocalLookDir,
            Mathf.Deg2Rad * _headMaxTurnAngle,
            0f
        );

        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        _headBone.localRotation = Quaternion.Slerp(
            currentLocalRotation,
            targetLocalRotation,
            1 - Mathf.Exp(-_headBoneRotationSpeed * Time.deltaTime)
        );
    }
}
