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
    public float maxMoveDistance = 20;    // 기존 위치에서 최대로 멀어질 수 있는 거리 
    public float sensingArea = 5;         // 타깃(Player,Pet) 감지 영역
    public float attackArea = 3;          // 공격 가능 영역
    public Animator anim;

    public enum State
    {
        Idle,       // 대기
        Walk,       // 추적
        Return,     // 복귀
        Attack      // 공격
    }
    public State state;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        Initialize();
    }

    /// <summary>
    /// Enemy 초기화
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

        // 만약, 기존 위치에 근접했다면 대기 상태로 전환한다.
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
        // 공격거리 안에 타겟이 있으면 유효타
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= attackArea)
        {
            target.gameObject.GetComponent<Player>().Damaged(attackDamage);
        }
    }

    public void OnAttackFInished()
    {
        // 공격이 끝난 시점에 타겟이 공격 범위 안에 없으면 타겟을 추적한다.
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

        // 기존 위치에서 얼마만큼 멀어졌는지 확인하다.
        distance = Vector3.Distance(originPos, transform.position);
        if (maxMoveDistance <= distance)
        {
            // 만약, 이동 가능한 최대 거리만큼 갔다면 기존 위치로 돌아간다.
            SetState(State.Return);
            agent.destination = originPos;
            return;
        }

        // 타깃이 공격 가능 영역안에 있는지 확인한다.
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (attackArea >= distance)
        {
            // 공격 가능 영역 안이라면 공격 상태로 전환한다.
            SetState(State.Attack);
            return;
        }

        agent.destination = target.transform.position;
    }

    void OnIdle()
    {
        // 타깃이 감지 영역안에 있는지 확인한다.
        Collider[] cols = Physics.OverlapSphere(transform.position, sensingArea);
        if (cols.Length > 0)
        {
            print(cols[0].gameObject.tag);
            if (cols[0].gameObject.tag == "Player")
            {
                // 감지된 타깃이 있으면 0번째 타깃 정보를 설정해 주고, 걷기 상태로 전환한다.
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
