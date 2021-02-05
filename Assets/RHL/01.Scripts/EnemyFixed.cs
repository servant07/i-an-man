using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFixed : MonoBehaviour
{
    public enum STATE
    {
        Idle,
        Attack
    }

    public STATE state;
    public float speedRotate;       // 회전 속도
    public int maxHP = 100;
    int curHP;
    float curTime;

    GameObject target;
    public float sensingArea;       // 탐지 범위
    public float sensingTime;       // 탐지 진행 시간
    public float attackDelayTime;   // 공격 대기 시간
    //public GameObject gunPos;       // 총구 위치(Gun_joint 1)

    public GameObject itemLvl;      // 아이템 강화 재료

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        curTime = 0;
        state = STATE.Idle;
        HP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.Idle:
                OnIdle();
                break;
            case STATE.Attack:
                OnAttack();
                break;
        }
    }


    void OnDie()
    {
        // HP가 0이면 아이템을 떨구고
        GameObject item = Instantiate(itemLvl);
        item.transform.position = transform.position;

        // 자기자신은 파괴한다.
        Destroy(gameObject);
    }

    private void OnIdle()
    {
        // 일정 시간이 지나면 자기 주변을 탐색한다.
        curTime += Time.deltaTime;
        if (curTime >= sensingTime)
        {
            // User가 감지되면 공격 상태로 전환한다.
            int layer = 1 << LayerMask.NameToLayer("Player");
            Collider[] cols = Physics.OverlapSphere(transform.position, sensingArea, layer);
            if (cols.Length > 0)
            {
                print(cols[0].gameObject.name);
                GameObject tr = cols[0].gameObject;
                target = tr;
                state = STATE.Attack;
            }
            curTime = 0;
            return;
        }

        transform.Rotate(Vector3.up * speedRotate * Time.deltaTime);
    }

    private void OnAttack()
    {
        // 타깃이 감지 범위를 벗어났는지 확인한다.
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (sensingArea <= distance)
        {
            state = STATE.Idle;
            curTime = 0;
            return;
        }

        transform.LookAt(target.transform);

        // 공격 대기 시간이 지나면 공격을 한다.
        curTime += Time.deltaTime;
        if (curTime >= attackDelayTime)
        {
            curTime = 0;
        }
    }


    public int HP
    {
        get { return curHP; }
        set
        {
            curHP = value;
            //txtHP.text = "HP : " + curHP;
            if (curHP <= 0)
            {
                OnDie();
            }
        }
    }
    public void Damaged(int damage)
    {
        HP -= damage;
        print("EnemyFixed HP: " + HP);
    }
}
