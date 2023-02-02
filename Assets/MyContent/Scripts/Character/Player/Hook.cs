using UnityEngine;
using Utils;

public class Hook : MonoBehaviour {
    [SerializeField] private Camera _camera;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private DistanceJoint2D _distanceJoint;
    [SerializeField] private LayerMask m_WhatIsHooking;
    [SerializeField] private float k_HookingRadius = .2f;

    // Start is called before the first frame update
    void Start() {
        _distanceJoint.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            var mousePos = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
            // var distance = Vector2.Distance(mousePos, transform.position);

            if (Physics2D.OverlapCircle(mousePos, k_HookingRadius, m_WhatIsHooking)) {
                _lineRenderer.SetPosition(0, mousePos);
                _lineRenderer.SetPosition(1, transform.position);
                _distanceJoint.connectedAnchor = mousePos;
                _distanceJoint.enabled = true;
                _lineRenderer.enabled = true;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0)) {
            _distanceJoint.enabled = false;
            _lineRenderer.enabled = false;
        }

        if (_distanceJoint.enabled) {
            _lineRenderer.SetPosition(1, transform.position);
        }
    }
}