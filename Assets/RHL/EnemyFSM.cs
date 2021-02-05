using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    NavMeshAgent agent;
    GameObject target;
    Vector3 originPos;

    [Header("Agent")]
    public float speed;
    public float stoppingDistance = 1;

    [Header("Enemy Info")]
    public int attackDamage;
    public float maxMoveDistance = 20;    // ���� ��ġ���� �ִ�� �־��� �� �ִ� �Ÿ� 
    public float sensingArea = 5;         // Ÿ��(Player,Pet) ���� ����
    public float attackArea = 3;          // ���� ���� ����
    public Animator anim;

    public enum State
    {
        Idle,       // ���
        Walk,       // ����
        Return,     // ����
        Attack      // ����
    }
    public State state;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        Initialize();
    }

    /// <summary>
    /// Enemy �ʱ�ȭ
    /// </summary>
    void Initialize()
    {
        agent.isStopped = true;
        agent.stoppingDistance = stoppingDistance;

        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
                OnIdle();
                break;
            case State.Walk:
                OnWalk();
                break;
            case State.Return:
                OnReturn();
                break;
        }
    }

    private void OnReturn()
    {
        float distance = distance = Vector3.Distance(originPos, transform.position);

        // ����, ���� ��ġ�� �����ߴٸ� ��� ���·� ��ȯ�Ѵ�.
        if (distance < 0.1f)
        {
            transform.position = originPos;
            SetState(State.Idle);
            return;
        }

        agent.destination = originPos;
    }

    public void OnAttack()
    {
        // ���ݰŸ� �ȿ� Ÿ���� ������ ��ȿŸ
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= attackArea)
        {
            target.gameObject.GetComponent<Player>().Damaged(attackDamage);
        }
    }

    public void OnAttackFInished()
    {
        // ������ ���� ������ Ÿ���� ���� ���� �ȿ� ������ Ÿ���� �����Ѵ�.
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (attackArea < distance)
        {
            SetState(State.Walk);
            return;
        }
    }

    private void OnWalk()
    {
        float distance = 0;

        // ���� ��ġ���� �󸶸�ŭ �־������� Ȯ���ϴ�.
        distance = Vector3.Distance(originPos, transform.position);
        if (maxMoveDistance <= distance)
        {
            // ����, �̵� ������ �ִ� �Ÿ���ŭ ���ٸ� ���� ��ġ�� ���ư���.
            SetState(State.Return);
            agent.destination = originPos;
            return;
        }

        // Ÿ���� ���� ���� �����ȿ� �ִ��� Ȯ���Ѵ�.
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (attackArea >= distance)
        {
            // ���� ���� ���� ���̶�� ���� ���·� ��ȯ�Ѵ�.
            SetState(State.Attack);
            return;
        }

        agent.destination = target.transform.position;
    }

    void OnIdle()
    {
        // Ÿ���� ���� �����ȿ� �ִ��� Ȯ���Ѵ�.
        Collider[] cols = Physics.OverlapSphere(transform.position, sensingArea);
        if (cols.Length > 0)
        {
            print(cols[0].gameObject.tag);
            if (cols[0].gameObject.tag == "Player")
            {
                // ������ Ÿ���� ������ 0��° Ÿ�� ������ ������ �ְ�, �ȱ� ���·� ��ȯ�Ѵ�.
                target = cols[0].gameObject;
                SetState(State.Walk);
                return;
            }
        }

    }

    void SetState(State nextState)
    {
        state = nextState;


        if (state == State.Walk || state == State.Return)
            agent.isStopped = false;
        else
            agent.isStopped = true;

        string str = (state == State.Return ? "Walk" : state.ToString());
        anim.SetTrigger(str);
    }
}
