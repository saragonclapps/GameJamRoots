using System;
using UnityEngine;

public class ManagerUpdate : MonoBehaviour {
    public static ManagerUpdate instance;
    public event Action Execute = delegate { };
    public event Action ExecuteFixed = delegate { };
    public event Action ExecuteLate = delegate { };

    public bool isPause { get; set; }

    private void Awake() {
        if (instance != null) {
            Destroy(this);
            instance = this;
        }
        else {
            instance = this;
        }
    }


    #region UPDATES

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            isPause = !isPause;
        }

        if (!isPause && !Execute.Equals(null)) {
            Execute();
        }
    }

    private void LateUpdate() {
        if (!isPause && !ExecuteLate.Equals(null)) ExecuteLate();
    }

    private void FixedUpdate() {
        if (!isPause && !ExecuteFixed.Equals(null)) ExecuteFixed();
    }

    #endregion UPDATES
}