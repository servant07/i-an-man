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
    public float speedRotate;       // ȸ�� �ӵ�
    public int maxHP = 100;
    int curHP;
    float curTime;

    GameObject target;
    public float sensingArea;       // Ž�� ����
    public float sensingTime;       // Ž�� ���� �ð�
    public float attackDelayTime;   // ���� ��� �ð�
    //public GameObject gunPos;       // �ѱ� ��ġ(Gun_joint 1)

    public GameObject itemLvl;      // ������ ��ȭ ���

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
        // HP�� 0�̸� �������� ������
        GameObject item = Instantiate(itemLvl);
        item.transform.position = transform.position;

        // �ڱ��ڽ��� �ı��Ѵ�.
        Destroy(gameObject);
    }

    private void OnIdle()
    {
        // ���� �ð��� ������ �ڱ� �ֺ��� Ž���Ѵ�.
        curTime += Time.deltaTime;
        if (curTime >= sensingTime)
        {
            // User�� �����Ǹ� ���� ���·� ��ȯ�Ѵ�.
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
        // Ÿ���� ���� ������ ������� Ȯ���Ѵ�.
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (sensingArea <= distance)
        {
            state = STATE.Idle;
            curTime = 0;
            return;
        }

        transform.LookAt(target.transform);

        // ���� ��� �ð��� ������ ������ �Ѵ�.
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
