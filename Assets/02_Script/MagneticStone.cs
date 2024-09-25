using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticStone : MonoBehaviour
{
    // 플레이어 최초 돌 갯수
    public int playerOneHaveStone = 10;
    public int playerTwoHaveStone = 10;
    private int clingStone = 0;

    // 자성 거리, 파워, 제한시간
    [SerializeField]
    private float magnetRange = 2;
    [SerializeField]
    private int magnetMinForce = 1;
    [SerializeField]
    private int magnetMaxForce = 10;
    [SerializeField]
    private float magnetTime = 2f;

    // 제한 시간
    private float turnTime = 10f;




    void Start()
    {
        /*for (int i = 0; i < GameManager.Instance.attackables.Count; i++)
        {
            // 캐스팅
            Vector3 _attackablePos = ((MonoBehaviour)GameManager.Instance.attackables[i]).transform.position;

            RaycastHit _hit;
            Vector3 _hitDir = (_attackablePos - _explode.position).normalized;
            float _distance = Vector3.Distance(_explode.position, _attackablePos);

            if (_distance <= radius)
            {
                if (Physics.Raycast(_explode.position, _hitDir, out _hit))
                {
                    // 거리별 값 판별 ( 멀어질수록 작은 값 )
                    float _damagePersent = 1 - (_distance / radius);
                    if (_damagePersent > 0)
                    {
                        Debug.Log(_hit.collider.gameObject.name);
                        //너무 멀면 오히려 피가 차오르는 현상 발생 1퍼센트이상의 대미지를 줄때만 대미지 함수 호출
                        int _calDamage = Mathf.RoundToInt(damage * _damagePersent);
                        GameManager.Instance.attackables[i].Damaged(_calDamage, _hit.point);
                    }
                }
            }
        }*/
    }

}
