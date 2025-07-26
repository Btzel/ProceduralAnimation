using UnityEngine;

public class GeckoController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform _target;

    [Header("Head Tracking")]
    [SerializeField] Transform _headBone;
    [SerializeField] float _headTrackingSpeed;
    [SerializeField] float _headMaxTurnAngle;

    [Header("Eye Tracking")]
    [SerializeField] Transform _leftEyeBone;
    [SerializeField] Transform _rightEyeBone;
    [SerializeField] float _eyeTrackingSpeed;
    [SerializeField] float _leftEyeMaxYRotation;
    [SerializeField] float _leftEyeMinYRotation;
    [SerializeField] float _rightEyeMaxYRotation;
    [SerializeField] float _rightEyeMinYRotation;

    void LateUpdate()
    {
        HeadTrackingUpdate();
        EyeTrackingUpdate();
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
            1 - Mathf.Exp(-_headTrackingSpeed * Time.deltaTime)
        );
    }

    void EyeTrackingUpdate()
    {
        Quaternion _targetEyeRotation = Quaternion.LookRotation(
            _target.position - _headBone.position,
            transform.up
        );

        _leftEyeBone.rotation = Quaternion.Slerp(
            _leftEyeBone.rotation,
            _targetEyeRotation,
            1 - Mathf.Exp(-_eyeTrackingSpeed * Time.deltaTime)
        );
        _rightEyeBone.rotation = Quaternion.Slerp(
            _rightEyeBone.rotation,
            _targetEyeRotation,
            1 - Mathf.Exp(-_eyeTrackingSpeed * Time.deltaTime)
        );

        float leftEyeCurrentYRotation = _leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentYRotation = _rightEyeBone.localEulerAngles.y;

        if (leftEyeCurrentYRotation > 180)
        {
            leftEyeCurrentYRotation -= 360;
        }
        if (rightEyeCurrentYRotation > 180)
        {
            rightEyeCurrentYRotation -= 360;
        }
        float leftEyeClampedYRotation = Mathf.Clamp(
            leftEyeCurrentYRotation,
            _leftEyeMinYRotation,
            _leftEyeMaxYRotation
        );

        float rightEyeClampedYRotation = Mathf.Clamp(
            rightEyeCurrentYRotation,
            _rightEyeMinYRotation,
            _rightEyeMaxYRotation
        );

        _leftEyeBone.localEulerAngles = new Vector3(
            _leftEyeBone.localEulerAngles.x,
            leftEyeClampedYRotation,
            _leftEyeBone.localEulerAngles.z
        );
        _rightEyeBone.localEulerAngles = new Vector3(
            _rightEyeBone.localEulerAngles.x,
            rightEyeClampedYRotation,
            _rightEyeBone.localEulerAngles.z
        );
    }

}
