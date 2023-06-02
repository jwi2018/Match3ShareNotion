using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectPool : Singleton<DynamicObjectPool>
{
    [SerializeField] private GameObject[] _objectPrefabs;
    [SerializeField] private Transform[] _objectParents;

    [SerializeField] public List<GameObject>[] _pooledObjects;

    [SerializeField] private int[] _amountToBuffer;

    [SerializeField] private int _defaultBufferAmount = 3;


    public void Init()
    {
        _pooledObjects = new List<GameObject>[_objectPrefabs.Length];

        var i = 0;
        foreach (var objectPrefab in _objectPrefabs)
        {
            _pooledObjects[i] = new List<GameObject>();

            int bufferAmount;

            if (i < _amountToBuffer.Length)
                bufferAmount = _amountToBuffer[i];
            else
                bufferAmount = _defaultBufferAmount;

            for (var n = 0; n < bufferAmount; n++)
            {
                var newObj = Instantiate(objectPrefab);
                newObj.name = objectPrefab.name;
                newObj.transform.SetParent(_objectParents[i]);
                newObj.transform.localScale = new Vector3(1, 1, 1);
                PoolObject(newObj, true);
            }

            i++;
        }
    }

    public GameObject GetObjectForType(string objectType, bool onlyPooled)
    {
        //Debug.Log($"GetObjectForType {objectType}");
        for (var i = 0; i < _objectPrefabs.Length; i++)
        {
            var prefab = _objectPrefabs[i];
            if (prefab.name == objectType)
            {
                if (!onlyPooled)
                {
                    var newObj = Instantiate(_objectPrefabs[i]);
                    newObj.name = _objectPrefabs[i].name;
                    newObj.transform.SetParent(_objectParents[i]);
                    newObj.transform.localScale = new Vector3(1, 1, 1);
                    newObj.SetActive(true);
                    return newObj;
                }

                if (_pooledObjects[i].Count > 0)
                {
                    var pooledObject = _pooledObjects[i][0];
                    _pooledObjects[i].RemoveAt(0);
                    //pooledObject.transform.parent = null;
                    pooledObject.SetActive(true);

                    return pooledObject;
                }

                {
                    var newObj = Instantiate(_objectPrefabs[i]);
                    newObj.name = _objectPrefabs[i].name;
                    newObj.transform.SetParent(_objectParents[i]);
                    newObj.transform.localScale = new Vector3(1, 1, 1);
                    newObj.SetActive(true);
                    return newObj;
                }
            }
        }

        Debug.LogError($"Not Set {objectType}");

        return null;
    }

    public bool PoolObject(GameObject obj, bool onlyPooled)
    {
        for (var i = 0; i < _objectPrefabs.Length; i++)
            if (_objectPrefabs[i].name == obj.name)
            {
                if (!onlyPooled)
                {
                    Destroy(obj);
                }
                else
                {
                    obj.SetActive(false);
                    obj.transform.localPosition = Vector3.zero;
                    _pooledObjects[i].Add(obj);
                }

                return true;
            }

        return false;
    }
}