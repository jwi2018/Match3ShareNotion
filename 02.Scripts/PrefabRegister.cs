using System.Collections.Generic;
using UnityEngine;

public class PrefabRegister : Singleton<PrefabRegister>
{
    [SerializeField] private GameObject[] _objectPrefabs;

    [SerializeField] public List<GameObject>[] _pooledObjects;
    Dictionary<string, GameObject> _dicResist = new Dictionary<string, GameObject>();

    public void Awake()
    {
        Init();
    }

    public void Init()
    {
        foreach (var objectPrefab in _objectPrefabs)
        {
            if(false == _dicResist.ContainsKey(objectPrefab.name))
            {
                _dicResist.Add(objectPrefab.name, objectPrefab);
            }
        }
    }

    public GameObject GetPrefab(string prefabName, Transform trParent = null)
    {
        GameObject r_prefab = null;
        if (true == _dicResist.ContainsKey(prefabName))
        {
            r_prefab = Instantiate(_dicResist[prefabName]);
            if(trParent != null)
            {
                r_prefab.transform.SetParent(trParent);
                r_prefab.transform.localPosition = Vector3.zero;
                r_prefab.transform.localScale = Vector3.one;
            }
        }

        return r_prefab;
    }
}