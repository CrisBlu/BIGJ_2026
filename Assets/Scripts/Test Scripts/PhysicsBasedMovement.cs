using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PhysicsBasedMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    private Vector3 _gravitationalForce;
    private Vector3 _rayDir = Vector3.down;
    private Vector2 _moveContext;
    private Vector3 _moveInput;
    private Vector3 _goalVel = Vector3.zero;
    private Quaternion _uprightTargetRot = Quaternion.identity;
    private Quaternion _lastTargetRot;

    [Header("Movement")]
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _acceleration = 200f;
    [SerializeField] private float _maxAccelForce = 150f;
    [SerializeField] private float _speedMultiplier = 1f;
    [SerializeField] private float _leanFactor = 0.25f;
    [SerializeField] private AnimationCurve _accelerationFactorFromDot;
    [SerializeField] private AnimationCurve _maxAccelForceFactorFromDot;
    [SerializeField] private Vector3 _moveForceScale = new Vector3(1f, 0f, 1f);

    [Header("Hover")]
    [SerializeField] private float _rideHeight = 1.75f;
    [SerializeField] private float _rayToGroundLength = 3f;
    [SerializeField] private float _rideSpringStrength = 50f;
    [SerializeField] private float _rideSpringDamper = 5f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Upright")]
    [SerializeField] private float _uprightSpringStrength = 40f;
    [SerializeField] private float _uprightSpringDamper = 5f;

    [Header("Camera")]
    [SerializeField] private bool _adjustInputsToCameraAngle = false;

    private void Start()
    {
        _gravitationalForce = Physics.gravity * _rb.mass;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _moveContext = new Vector2(h, v);
        Debug.Log(_moveContext);
    }

    private void FixedUpdate()
    {
        _moveInput = new Vector3(_moveContext.x, 0f, _moveContext.y);

        if (_adjustInputsToCameraAngle)
        {
            float facing = Camera.main.transform.eulerAngles.y;
            _moveInput = Quaternion.Euler(0, facing, 0) * _moveInput;
        }

        (bool rayHitGround, RaycastHit rayHit) = RaycastToGround();

        if (rayHitGround)
            MaintainHeight(rayHit);

        Vector3 velocity = _rb.linearVelocity;
        velocity.y = 0f;
        MaintainUpright(velocity);

        CharacterMove(_moveInput, rayHit);
    }

    private (bool, RaycastHit) RaycastToGround()
    {
        RaycastHit rayHit;
        bool hit = Physics.Raycast(transform.position, _rayDir, out rayHit, _rayToGroundLength, _groundLayer.value);
        return (hit, rayHit);
    }

    private void MaintainHeight(RaycastHit rayHit)
    {
        Vector3 vel = _rb.linearVelocity;
        Vector3 otherVel = Vector3.zero;

        if (rayHit.rigidbody != null)
            otherVel = rayHit.rigidbody.linearVelocity;

        float rayDirVel = Vector3.Dot(_rayDir, vel);
        float otherDirVel = Vector3.Dot(_rayDir, otherVel);
        float relVel = rayDirVel - otherDirVel;
        float currHeight = rayHit.distance - _rideHeight;
        float springForce = (currHeight * _rideSpringStrength) - (relVel * _rideSpringDamper);

        Vector3 maintainHeightForce = -_gravitationalForce + springForce * Vector3.down;
        _rb.AddForce(maintainHeightForce);

        if (rayHit.rigidbody != null)
            rayHit.rigidbody.AddForceAtPosition(-maintainHeightForce, rayHit.point);
    }

    private void MaintainUpright(Vector3 yLookAt)
    {
        if (yLookAt != Vector3.zero)
        {
            _uprightTargetRot = Quaternion.LookRotation(yLookAt, Vector3.up);
            _lastTargetRot = _uprightTargetRot;
        }

        Quaternion currentRot = transform.rotation;
        Quaternion toGoal = ShortestRotation(_uprightTargetRot, currentRot);

        toGoal.ToAngleAxis(out float rotDegrees, out Vector3 rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        _rb.AddTorque((rotAxis * (rotRadians * _uprightSpringStrength)) - (_rb.angularVelocity * _uprightSpringDamper));
    }

    private void CharacterMove(Vector3 moveInput, RaycastHit rayHit)
    {
        Vector3 unitVel = _goalVel.normalized;
        float velDot = Vector3.Dot(moveInput, unitVel);
        float accel = _acceleration * _accelerationFactorFromDot.Evaluate(velDot);

        Vector3 goalVel = moveInput * _maxSpeed * _speedMultiplier;

        Vector3 groundVel = Vector3.zero;
        if (rayHit.rigidbody != null)
            groundVel = rayHit.rigidbody.linearVelocity;

        _goalVel = Vector3.MoveTowards(_goalVel, goalVel + groundVel, accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (_goalVel - _rb.linearVelocity) / Time.fixedDeltaTime;
        float maxAccel = _maxAccelForce * _maxAccelForceFactorFromDot.Evaluate(velDot);
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        _rb.AddForceAtPosition(
            Vector3.Scale(neededAccel * _rb.mass, _moveForceScale),
            transform.position + new Vector3(0f, transform.localScale.y * _leanFactor, 0f)
        );
    }

    private Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        if (Quaternion.Dot(a, b) < 0)
            return a * Quaternion.Inverse(new Quaternion(-b.x, -b.y, -b.z, -b.w));
        else
            return a * Quaternion.Inverse(b);
    }
}
