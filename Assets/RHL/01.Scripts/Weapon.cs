using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum TYPE
    {
        Gauntlet,
        Sword
    }
    public TYPE type;
    public int value;
    public int damage;
    int level;
    public float attackDistance;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize()
    }

    void Initialize()
    {
        level = 1;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
