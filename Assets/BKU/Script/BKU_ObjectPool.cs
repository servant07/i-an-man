using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * 오브젝트 풀 클래스.
 * 사용법
 * 1. ObjectPool 오브젝트의 inspector 창에서 PoolNode 추가.
 * 2. 추가한 prefab의 name으로 ObjectType enum에 추가.
 * 3. 필요한 오브젝트를 GetObject 함수로 오브젝트를 가져오고, 사용이 끝나면 ReturnObject 함수로 돌려보내주세요.
 */
public enum ObjectType
{
    Cubeeess,
}
public class BKU_ObjectPool : MonoBehaviour
{
    public static BKU_ObjectPool Instance { get; private set; }


    [System.Serializable]
    struct PoolNode
    {
        [SerializeField]
        public GameObject prefab;
        [SerializeField]
        public int count;
    }
    Transform transform;
    [SerializeField]
    PoolNode[] poolNode;
    Dictionary<ObjectType, Queue<GameObject>> objectPool;
    Dictionary<string, GameObject> objectPosition;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        if (Instance != null)
        {
            Debug.LogErrorFormat("{0} 스크립트는 싱글톤입니다. 더이상 존재할 수 없습니다. {1}보존. {2}삭제", typeof(BKU_ObjectPool), ObjectPath(Instance.transform), ObjectPath(transform));
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        transform = GetComponent<Transform>();
        objectPool = new Dictionary<ObjectType, Queue<GameObject>>();
        objectPosition = new Dictionary<string, GameObject>();
        for (int nodeCount = 0; nodeCount < poolNode.Length; nodeCount++)
        {
            ObjectType poolList;
            int count = poolNode[nodeCount].count;
            GameObject prefab = poolNode[nodeCount].prefab;
            string objectName = prefab.name;

            objectPosition.Add(objectName, new GameObject());
            objectPosition[objectName].name = objectName;

            if (Enum.TryParse(objectName, out poolList))
            {
                Queue<GameObject> objectQueue = new Queue<GameObject>();
                for (int objectCount = 0; objectCount < count; objectCount++)
                {
                    objectQueue.Enqueue(poolInstantiate(prefab));
                }
                objectPool.Add(poolList, objectQueue);

            }
            else
            {
                Debug.LogErrorFormat("새로운 object가 감지됨. BKU_ObjectPool 스크립트 {0}확인 후 수정. ", typeof(ObjectType));
            }
        }
    }
    string ObjectPath(Transform obj, string result = "")
    {
        if (obj == null)
        {
            return result;
        }
        else
        {
            return ObjectPath(obj.parent, obj.name + "/" + result);
        }
    }

    GameObject poolInstantiate(GameObject prefab)
    {
        GameObject inst = Instantiate(prefab);
        string objectName = prefab.name;
        inst.name = objectName;
        inst.transform.parent = objectPosition[objectName].transform;
        inst.SetActive(false);
        return inst;
    }
    GameObject poolInstantiate(ObjectType type)
    {
        for (int i = 0; i < poolNode.Length; i++)
        {
            if (poolNode[i].prefab.name == type.ToString())
            {
                return poolInstantiate(poolNode[i].prefab);
            }
        }
        return null;
    }
    public GameObject GetObject(ObjectType type)
    {
        if (objectPool[type].Count > 0)
            return objectPool[type].Dequeue();
        else
            return poolInstantiate(type);
    }
    public void ReturnObject(GameObject obj)
    {
        ObjectType type;
        if (Enum.TryParse(obj.name, out type))
        {
            objectPool[type].Enqueue(obj);

            obj.transform.parent = objectPosition[obj.name].transform;
        }

    }

}
