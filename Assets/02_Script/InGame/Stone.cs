using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private List<FixedJoint> joints = new List<FixedJoint>();
    private Magnet magnet;
    
    private void Awake()
    {
        magnet = GetComponent<Magnet>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Stone otherAttacher = collision.gameObject.GetComponent<Stone>();
        if (otherAttacher != null && !IsConnectedTo(otherAttacher.gameObject))
        {
            AttachObject(otherAttacher.gameObject);
        }
    }

    private void AttachObject(GameObject obj)
    {
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        if (objRb == null)
        {
            objRb = obj.AddComponent<Rigidbody>();
        }

        FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = objRb;
        joints.Add(newJoint);

        // ���� ������Ʈ�� ������ ����
        objRb.mass = 0f;
        objRb.drag = 0f;
        objRb.angularDrag = 0.5f;

        // �ð� �ʱ�ȭ �� �Ŀ�, ���� ����
        magnet.magnetTime = 3f;
        magnet.magnetRange += 1;
        magnet.magnetMaxForce += 1;
    }

    private bool IsConnectedTo(GameObject obj)
    {
        return joints.Exists(j => j.connectedBody.gameObject == obj);
    }

}
