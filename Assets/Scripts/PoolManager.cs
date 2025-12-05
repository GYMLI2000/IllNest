using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class PoolItem
    {
        public string key;
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 50;
        public Transform poolParent;
    }

    public List<PoolItem> pools;

    private Dictionary<string, IObjectPool<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        poolDictionary = new Dictionary<string, IObjectPool<GameObject>>();

        foreach (var p in pools)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(p.prefab,p.poolParent),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: p.defaultCapacity,
                maxSize: p.maxSize
            );

            poolDictionary.Add(p.key, pool);
        }
    }

    public GameObject Get(string key)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($"Pool '{key}' neexistuje!");
            return null;
        }

        return poolDictionary[key].Get();
    }

    public void Get(string key, float delay, System.Action<GameObject> callback)
    {
        StartCoroutine(GetAfterDelay(key, delay, callback));
    }

    private IEnumerator GetAfterDelay(string key, float delay, System.Action<GameObject> callback)
    {
        yield return new WaitForSeconds(delay);

        GameObject result = Get(key); 
        callback?.Invoke(result);
    }

    public void Release(string key, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($"Pool '{key}' neexistuje!");
            Destroy(obj);
            return;
        }

        poolDictionary[key].Release(obj);
    }

    public void Release(string key, GameObject obj, float delay)
    {
        StartCoroutine(ReleaseAfterDelay(key, obj, delay));
    }

    private IEnumerator ReleaseAfterDelay(string key, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null && obj.activeInHierarchy)
        {
            Release(key, obj);
        }
    }


}
