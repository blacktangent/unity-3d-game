using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // �Ώۂ�3D�I�u�W�F�N�g

    void Start()
    {
        if (target != null)
        {
            transform.LookAt(target);  // �Ώۂ�����
        }
    }
}
