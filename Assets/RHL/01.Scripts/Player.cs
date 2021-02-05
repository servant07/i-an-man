using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    CharacterController cc;

    float gravity = -9.81f;
    float yVelocity = 0;

    int curHP;
    public int maxHP = 100;
    public float speed = 1f;
    public float rotspeed = 200f;
    public Transform armForward;

    int iValue = 0;
    int damage;
    public float rayDistance = 2;
    float attackDistance;
    public GameObject[] weapons;
    public Text txtHP;
    public Text txtDamage;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        HP = maxHP;
        txtDamage.text = "Damage: " + damage;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();

        if(Input.GetMouseButtonDown(0))
        {
            print("Ray");

            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, rayDistance))
            {
                print(hitInfo.transform.gameObject.name);

                if (hitInfo.transform.tag == "Weapon")
                {
                    weapons[iValue].SetActive(false);

                    Weapon weapon = hitInfo.transform.gameObject.GetComponent<Weapon>();
                    iValue = weapon.value;
                    damage = weapon.damage;
                    rayDistance = weapon.attackDistance;
                    weapons[iValue].SetActive(true);
                    txtDamage.text = "Damage: " + damage;

                }
                else if (hitInfo.transform.tag == "Enemy")
                {
                    EnemyFixed enemy = hitInfo.transform.gameObject.GetComponent<EnemyFixed>();
                    enemy.Damaged(damage);
                }
                else if (hitInfo.transform.tag == "ItemEtc")
                {
                    print(hitInfo.transform.tag);
                }
                else if (hitInfo.transform.tag == "ItemLvl")
                {
                    OnWeaponLevelUp();
                    Destroy(hitInfo.transform.gameObject);
                }
            }
        }
    }

    private void OnWeaponLevelUp()
    {
        damage *= 2;
        txtDamage.text = "Damage: " + damage;
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir.Normalize();
        dir = Camera.main.transform.TransformDirection(dir);

        yVelocity += gravity * Time.deltaTime;
        dir.y = yVelocity;

        cc.Move(dir * speed * Time.deltaTime);
    }

    float ry;
    void Rotate()
    {
        float mx = Input.GetAxis("Mouse X");

        ry += mx * rotspeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, ry, 0);
    }

    public int HP
    {
        get { return curHP; }
        set
        {
            curHP = value;
            txtHP.text = "HP : " + curHP;
            if (curHP <= 0)
            {
                // 게임 종료
            }
        }
    }
    public void Damaged(int damage)
    {
        HP -= damage;
    }
}
