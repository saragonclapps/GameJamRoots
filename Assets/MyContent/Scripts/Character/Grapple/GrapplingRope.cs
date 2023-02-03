using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GrapplingRope : MonoBehaviour {
    [Header("General References:")] 
    public GrapplingGun grapplingGun;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")] 
    [SerializeField]
    private int percision = 40;
    [Range(0, 20)] 
    [SerializeField] 
    private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")] 
    public AnimationCurve ropeAnimationCurve;
    [FormerlySerializedAs("StartWaveSize")]
    [Range(0.01f, 4)] 
    [SerializeField] 
    private float _startWaveSize = 2;
    private float _waveSize;

    [Header("Rope Progression:")] 
    public AnimationCurve ropeProgressionCurve;
    [FormerlySerializedAs("ropeProgressionSpeed")]
    [SerializeField] 
    [Range(1, 50)] 
    private float _ropeProgressionSpeed = 1;
    private float _moveTime;

    [HideInInspector] 
    public bool isGrappling = true;
    private bool _strightLine = true;

    #region MonoBehaviour
    private void Start() {
        ManagerUpdate.instance.Execute += Execute;
    }
    
    private void OnDestroy() {
        ManagerUpdate.instance.Execute -= Execute;
    }

    private void OnEnable() {
        _moveTime = 0;
        m_lineRenderer.positionCount = percision;
        _waveSize = _startWaveSize;
        _strightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
    }

    private void OnDisable() {
        m_lineRenderer.enabled = false;
        isGrappling = false;
    }
    #endregion
    
    private void LinePointsToFirePoint() {
        for (var i = 0; i < percision; i++) {
            m_lineRenderer.SetPosition(i, grapplingGun.firePoint.position);
        }
    }

    private void Execute() {
        if (!enabled) return;
        _moveTime += Time.deltaTime;
        DrawRope();
    }

    private void DrawRope() {
        if (!_strightLine) {
            var distance = Vector2.Distance(m_lineRenderer.GetPosition(percision - 1), grapplingGun.grapplePoint);
            if (distance <= grapplingGun.hookingRadius) {
                _strightLine = true;
            }
            else {
                DrawRopeWaves();
            }
        }else {
            if (!isGrappling) {
                grapplingGun.Grapple();
                isGrappling = true;
            }

            if (_waveSize > 0) {
                _waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else {
                _waveSize = 0;

                if (m_lineRenderer.positionCount != 2) {
                    m_lineRenderer.positionCount = 2;
                }

                DrawRopeNoWaves();
            }
        }
    }

    private void DrawRopeWaves() {
        for (var i = 0; i < percision; i++) {
            var delta = (float)i / ((float)percision - 1f);
            var counterClockwise = Vector2.Perpendicular(grapplingGun.grappleDistanceVector).normalized;
            var offset = counterClockwise * (ropeAnimationCurve.Evaluate(delta) * _waveSize);
            var position = grapplingGun.firePoint.position;
            var targetPosition = Vector2.Lerp(position, grapplingGun.grapplePoint, delta) + offset;
            var currentPosition = Vector2.Lerp(position, targetPosition, ropeProgressionCurve.Evaluate(_moveTime) * _ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }
    }

    private void DrawRopeNoWaves() {
        m_lineRenderer.SetPosition(0, grapplingGun.firePoint.position);
        m_lineRenderer.SetPosition(1, grapplingGun.grapplePoint);
    }
}