using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticStone : MonoBehaviour
{
    // �÷��̾� ���� �� ����
    public int playerOneHaveStone = 10;
    public int playerTwoHaveStone = 10;
    private int clingStone = 0;

    // �ڼ� �Ÿ�, �Ŀ�, ���ѽð�
    [SerializeField]
    private float magnetRange = 2;
    [SerializeField]
    private int magnetMinForce = 1;
    [SerializeField]
    private int magnetMaxForce = 10;
    [SerializeField]
    private float magnetTime = 2f;

    // ���� �ð�
    private float turnTime = 10f;




    void Start()
    {
        /*for (int i = 0; i < GameManager.Instance.attackables.Count; i++)
        {
            // ĳ����
            Vector3 _attackablePos = ((MonoBehaviour)GameManager.Instance.attackables[i]).transform.position;

            RaycastHit _hit;
            Vector3 _hitDir = (_attackablePos - _explode.position).normalized;
            float _distance = Vector3.Distance(_explode.position, _attackablePos);

            if (_distance <= radius)
            {
                if (Physics.Raycast(_explode.position, _hitDir, out _hit))
                {
                    // �Ÿ��� �� �Ǻ� ( �־������� ���� �� )
                    float _damagePersent = 1 - (_distance / radius);
                    if (_damagePersent > 0)
                    {
                        Debug.Log(_hit.collider.gameObject.name);
                        //�ʹ� �ָ� ������ �ǰ� �������� ���� �߻� 1�ۼ�Ʈ�̻��� ������� �ٶ��� ����� �Լ� ȣ��
                        int _calDamage = Mathf.RoundToInt(damage * _damagePersent);
                        GameManager.Instance.attackables[i].Damaged(_calDamage, _hit.point);
                    }
                }
            }
        }*/
    }

}
