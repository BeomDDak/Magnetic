using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private Rigidbody rb;
    private Magnet magnet;
    private List <FixedJoint> joints = new List<FixedJoint>();
    private bool canAttach = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        magnet = GetComponent<Magnet>();
    }

    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canAttach && collision.gameObject.CompareTag("Stone"))
        {
            Attach(collision.gameObject);
        }
    }

    private void Attach(GameObject target)
    {
        FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
        joints.Add(newJoint);

        // 시간 초기화 및 파워, 범위 증가
        magnet.magnetTime = 3f;
        magnet.magnetRange += 1;
        magnet.magnetMaxForce += 1;
    }

}
