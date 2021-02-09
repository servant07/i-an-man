using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * ������Ʈ Ǯ Ŭ����.
 * ����
 * 1. ObjectPool ������Ʈ�� inspector â���� PoolNode �߰�.
 * 2. �߰��� prefab�� name���� ObjectType enum�� �߰�.
 * 3. �ʿ��� ������Ʈ�� GetObject �Լ��� ������Ʈ�� ��������, ����� ������ ReturnObject �Լ��� ���������ּ���.
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
            Debug.LogErrorFormat("{0} ��ũ��Ʈ�� �̱����Դϴ�. ���̻� ������ �� �����ϴ�. {1}����. {2}����", typeof(BKU_ObjectPool), ObjectPath(Instance.transform), ObjectPath(transform));
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
                Debug.LogErrorFormat("���ο� object�� ������. BKU_ObjectPool ��ũ��Ʈ {0}Ȯ�� �� ����. ", typeof(ObjectType));
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
