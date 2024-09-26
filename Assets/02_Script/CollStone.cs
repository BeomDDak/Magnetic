using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollStone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("LandingStone"))
        {
            GameManager.Instance.magnetTime = 3;

        }
    }

}
