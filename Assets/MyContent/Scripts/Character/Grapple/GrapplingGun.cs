using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GrapplingGun : MonoBehaviour {
    private enum LaunchType {
        TRANSFORM_LAUNCH,
        PHYSICS_LAUNCH
    }
    
    [Header("General Settings:")]
    [SerializeField]
    private Player _player;
    private float _rbInitialGravityScale = 1;
    
    [Header("Scripts Ref:")] 
    public GrapplingRope grappleRope;

    [Header("Layers Settings:")] 
    [SerializeField]
    private bool grappleToAll;
    [SerializeField] 
    private LayerMask m_WhatIsHooking;
    public float hookingRadius = .01f;
    
    [Header("Main Camera:")] 
    public Camera m_camera;

    [Header("Transform Ref:")] 
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")] 
    [SerializeField] 
    private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] 
    private float rotationSpeed = 4;

    [Header("Distance:")] 
    [SerializeField] 
    private bool hasMaxDistance = false;
    [FormerlySerializedAs("maxDistance")] [SerializeField] 
    private float _maxDistance = 20;

    [Header("Launching:")] [SerializeField]
    private bool launchToPoint = true;
    [SerializeField] 
    private LaunchType launchType = LaunchType.PHYSICS_LAUNCH;
    [SerializeField] 
    private float launchSpeed = 1;

    [Header("No Launch To Point")] 
    [SerializeField]
    private bool autoConfigureDistance = false;
    [SerializeField] 
    private float targetDistance = 3;
    [SerializeField] 
    private float targetFrequency = 1;
    
    [Header("Pendulum Settings:")] 
    [SerializeField]
    private bool _isPendulum = true;
    // Initial angle. Must be different from 0
    // 1 is 360
    [SerializeField]
    [Range(0.01f, 1)]
    private float _initialValueAngular = 0.5f;
    // Initial angular velocity
    [SerializeField]
    private float _initialAngularVelocity = 3;
    [SerializeField] 
    private float _speedDistanceMovePendulum = 30;
    [SerializeField]
    private float _minDistancePendulum = 3;
    [SerializeField]
    private float _maxDistancePendulum = 5;
    private float _currentDistancePedulum =  5;
    
    [SerializeField]
    private float _speedToMoveAngularVelocity = 1;
    [SerializeField]
    private Vector2 _pendulumImpulse = new Vector2(300, 50);
    
    private float _valueAngular; 
    private float _nextValueAngular;
    private float _angularVelocity; 
    private float _nextAngularVelocity;

    
    [HideInInspector] 
    public Vector2 grapplePoint;
    [HideInInspector] 
    public Vector2 grappleDistanceVector;

    #region MonoBehaviour
    
    private void Start() {
        m_springJoint2D.enabled = false;
        grappleRope.enabled = false;
        _rbInitialGravityScale = m_rigidbody.gravityScale;
        ManagerUpdate.instance.Execute += Execute;
    }

    private void OnDestroy() {
        ManagerUpdate.instance.Execute -= Execute;
    }
    
    private void OnDrawGizmosSelected() {
        if (firePoint == null || !hasMaxDistance) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(firePoint.position, _maxDistance);
        if (transform.parent == null) return;
        Gizmos.color = Color.blue;
        var rotationDistance = Vector2.Distance(transform.position, transform.parent.position);
        Gizmos.DrawWireSphere(transform.parent.position, _maxDistance + rotationDistance);
    }

    #endregion

    private void Execute() {
        if (!enabled) return;
        
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            SetGrapplePoint();
        }
        if (Input.GetKey(KeyCode.Mouse0)) {
            if (grappleRope.enabled) {
                RotateGun(grapplePoint, false);
            }
            else {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (!launchToPoint || !grappleRope.isGrappling) return;
            PendulumMovementInGrapple();
            VerticalPendulumMovement();

            if (launchType != LaunchType.TRANSFORM_LAUNCH) return;
            Vector2 firePointDistance = firePoint.position - gunHolder.localPosition;
            var targetPos = grapplePoint - firePointDistance;
            gunHolder.position = Vector2.Lerp(gunHolder.position, targetPos, Time.deltaTime * launchSpeed);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            _player.characterController2D.hasCustomGravity = true;
            m_rigidbody.gravityScale = _rbInitialGravityScale;
            
            // Pendulum add force
            var xNormalizePosition = _player.transform.position.x < grapplePoint.x ? -1 : 1;
            var angle = Math.Abs(AngleBetweenVector2(transform.position, grapplePoint)) / 180;
            var normalizePosition = angle * Mathf.PI * xNormalizePosition;
            
            m_rigidbody.AddForce(new Vector2(normalizePosition * _pendulumImpulse.x, _pendulumImpulse.y));
        }else {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }



    #region Pendulum
    private void VerticalPendulumMovement() {
        if (!_isPendulum || _player.playerController.verticalMove == 0) return;
        
        if (_player.playerController.verticalMove > 0 && _minDistancePendulum <= _currentDistancePedulum) {
#if UNITY_EDITOR
            // Logger.Debug.LogColor(_currentDistancePedulum, "red");
#endif
            _currentDistancePedulum -= _speedDistanceMovePendulum * Time.deltaTime;
        }else if (_player.playerController.verticalMove < 0 && _maxDistancePendulum >= _currentDistancePedulum) {
#if UNITY_EDITOR
            // Logger.Debug.LogColor(_currentDistancePedulum, "green");
#endif
            _currentDistancePedulum += _speedDistanceMovePendulum * Time.deltaTime;
        }
    }
    
    private void PendulumMovementInGrapple() {
        var currentDistance = Vector2.Distance(grapplePoint, transform.position);
        if (!_isPendulum || !(currentDistance <= _maxDistancePendulum)) return;
        m_springJoint2D.enabled = false;
        
        EulerCromer(_currentDistancePedulum);
        PolarToCartesian(_currentDistancePedulum);
    }
    
    private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2) {
        var diference = vec2 - vec1;
        var sign = vec2.y < vec1.y ? -1.0f : 1.0f;
        
        return Vector2.Angle(Vector2.right, diference) * sign;
    }
    
    private void EulerCromer(float distance) {
        var g = Math.Abs(Constants.GameSettings.GRAVITY);
        _angularVelocity = _nextAngularVelocity;
        _valueAngular = _nextValueAngular;
        _nextAngularVelocity = _angularVelocity - (g/distance) * _valueAngular * Time.deltaTime;
        _nextValueAngular = _valueAngular + _nextAngularVelocity * Time.deltaTime;
    }
    
    private void PolarToCartesian(float distance){
        Vector2 position =  _player.transform.position;
        position.x = grapplePoint.x + distance * Mathf.Sin(_nextValueAngular);
        position.y = grapplePoint.y + -distance * Mathf.Cos(_nextValueAngular);

        m_rigidbody.MovePosition(Vector2.Lerp(m_rigidbody.position, position, Time.deltaTime * _speedToMoveAngularVelocity));
    }
    
    #endregion
    
    private void RotateGun(Vector3 lookPoint, bool allowRotationOverTime) {
        var distanceVector = lookPoint - gunPivot.position;

        var angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime) {
            gunPivot.rotation = Quaternion.Lerp(
                gunPivot.rotation, 
                Quaternion.AngleAxis(angle, Vector3.forward),
                Time.deltaTime * rotationSpeed
            );
        }
        else {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void SetGrapplePoint() {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        if (!Physics2D.Raycast(firePoint.position, distanceVector.normalized)) return;
        var hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized);
        var mousePos = (Vector2)m_camera.ScreenToWorldPoint(Input.mousePosition);

        if (!Physics2D.OverlapCircle(mousePos, hookingRadius, m_WhatIsHooking) && !grappleToAll) return;
        if (!(Vector2.Distance(hit.point, firePoint.position) <= _maxDistance) && hasMaxDistance) return;
        grapplePoint = hit.point;
        grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
        grappleRope.enabled = true;
        
        // It's a initial position in the pendulum
        m_springJoint2D.distance = _maxDistancePendulum;
        var xNormalizePosition = _player.transform.position.x < grapplePoint.x ? 1 : -1;
        var distance = Vector2.Distance(grapplePoint, transform.position);
        _nextValueAngular = _valueAngular = _initialValueAngular * xNormalizePosition * -1;
        _nextAngularVelocity = _angularVelocity = (_initialAngularVelocity * distance / _maxDistance) * xNormalizePosition;
    }

    public void Grapple() {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance) {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequency;
        }

        if (!launchToPoint) {
            if (autoConfigureDistance) {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else {
            LaunchByType();
        }
    }

    private void LaunchByType() {
        _currentDistancePedulum = _maxDistancePendulum;
        switch (launchType) {
            case LaunchType.PHYSICS_LAUNCH:
                m_springJoint2D.connectedAnchor = grapplePoint;
                _player.characterController2D.hasCustomGravity = false;

                // Vector2 distanceVector = firePoint.position - gunHolder.position;
                // m_springJoint2D.distance = distanceVector.magnitude;
                m_springJoint2D.frequency = launchSpeed;
                m_springJoint2D.enabled = true;
                break;
            case LaunchType.TRANSFORM_LAUNCH:
                _player.characterController2D.hasCustomGravity = false;

                m_rigidbody.gravityScale = 0;
                m_rigidbody.velocity = Vector2.zero;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}