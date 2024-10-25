using System.Collections.Generic;
using UnityEngine;
using static Define;
using Protocol;
using System.Linq;

public class Stone : MonoBehaviour
{
    public List<FixedJoint> joints = new List<FixedJoint>();
    private Magnet magnet;
    public Player m_CurrentPlayer;

    public GameObject startStone;
    public string stoneId;  // ��Ʈ��ũ ����ȭ�� ���� ID �߰�
    private Rigidbody rb;

    private void Awake()
    {
        magnet = GetComponent<Magnet>();
        rb = GetComponent<Rigidbody>();
        m_CurrentPlayer = Player.None;
        stoneId = System.Guid.NewGuid().ToString();

    }

    private void OnCollisionEnter(Collision collision)
    {
        Stone otherAttacher = collision.gameObject.GetComponent<Stone>();
        if (otherAttacher != null && !IsConnectedTo(otherAttacher.gameObject))
        {
            startStone = this.gameObject;
            AttachObject(otherAttacher.gameObject);
            SendStoneSyncMessge();
        }
    }

    public void AttachObject(GameObject obj)
    {
        Rigidbody objRb = obj.GetComponent<Rigidbody>();
        FixedJoint newJoint = gameObject.AddComponent<FixedJoint>();
        newJoint.connectedBody = objRb;
        joints.Add(newJoint);

        // ���� ������Ʈ�� ������ ����
        objRb.mass = 1f;
        objRb.drag = 0f;
        objRb.angularDrag = 0.5f;

        // �ð� �ʱ�ȭ �� �Ŀ�, ���� ����
        magnet.magnetTime = 3f;
        magnet.magnetRange += 0.2f;
        magnet.magnetMaxForce += 0.1f;

        // �پ��ٸ� ������Ʈ Ȯ��
        magnet.cling = true;
    }

    public void SendStoneSyncMessge()
    {
        // ���� ����ȭ �޽��� ����
        List<string> attachedIds = new List<string>();
        foreach (var connectobj in CountConnectedObjects())
        {
            Stone attachedStone = connectobj.GetComponent<Stone>();
            if (attachedStone != null)
            {
                attachedIds.Add(attachedStone.stoneId);
            }
        }

        StoneSyncMessage msg = new StoneSyncMessage(
            stoneId,
            transform.position,
            transform.rotation,
            rb.velocity,
            rb.angularVelocity,
            attachedIds
        );
        BackendMatchManager.Instance.SendDataToInGame(msg);
    }

    public bool IsConnectedTo(GameObject obj)
    {
        return joints.Exists(j => j.connectedBody.gameObject == obj);
    }

    public List<GameObject> CountConnectedObjects()
    {
        HashSet<GameObject> visitedObjects = new HashSet<GameObject>();
        Queue<GameObject> objectsToCheck = new Queue<GameObject>();

        objectsToCheck.Enqueue(startStone);
        visitedObjects.Add(startStone);

        while (objectsToCheck.Count > 0)
        {
            GameObject currentObject = objectsToCheck.Dequeue();
            FixedJoint[] joints = currentObject.GetComponents<FixedJoint>();

            foreach (FixedJoint joint in joints)
            {
                if (joint.connectedBody != null)
                {
                    GameObject connectedObject = joint.connectedBody.gameObject;
                    if (!visitedObjects.Contains(connectedObject))
                    {
                        objectsToCheck.Enqueue(connectedObject);
                        visitedObjects.Add(connectedObject);
                    }
                }
            }
        }
        return new List<GameObject>(visitedObjects);
    }
}
