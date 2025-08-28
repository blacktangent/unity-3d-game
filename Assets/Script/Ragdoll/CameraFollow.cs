using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // 対象の3Dオブジェクト

    void Start()
    {
        if (target != null)
        {
            transform.LookAt(target);  // 対象を向く
        }
    }
}
