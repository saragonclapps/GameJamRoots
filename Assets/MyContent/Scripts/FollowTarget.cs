using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    private void Start() {
        ManagerUpdate.instance.Execute += Execute;
    }
    
    private void OnDestroy() {
        ManagerUpdate.instance.Execute -= Execute;
    }
    
    private void Execute() {
        if (target == null) return;
        transform.position = target.position;
    }
}
