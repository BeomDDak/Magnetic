using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    /*private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Board"))
        {
            GameManager.Instance.StoneManager.TriggerMagneticEffect(transform.position);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.StoneManager.RemoveStone(this);
    }*/
}
