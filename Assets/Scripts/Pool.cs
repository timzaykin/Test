using System.Collections;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

//Кастомная реализация шаблона ObjecPool
public class Pool 
{

    private Transform parentPool;

    private Dictionary<int, Stack<GameObject>> cachedObjects = new Dictionary<int, Stack<GameObject>>();
    private Dictionary<int, int> cachedIds = new Dictionary<int, int>();

    public Pool PopulateWith(GameObject prefab, int amount, int amountPerTick, int tickSize = 1) {

        int key = prefab.GetInstanceID();
        Debug.Log(key);
        Stack<GameObject> stack = new Stack<GameObject>();

        cachedObjects.Add(key, stack);
        Observable.IntervalFrame(tickSize, FrameCountType.EndOfFrame).Where(val => amount > 0).Subscribe(_loop => {
            Observable.Range(0, amountPerTick).Where(Check => amount > 0).Subscribe(_pop =>
            {
                GameObject go = Populate(prefab, Vector3.zero, Quaternion.identity, parentPool);
                go.SetActive(false);
                cachedIds.Add(go.GetInstanceID(), key);
                cachedObjects[key].Push(go);
                amount--;
            });
        });
        return this;
    }

    public void SetParent(Transform parent) {
        this.parentPool = parent;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position = default,Quaternion rotation = default, Transform parent = null) {

        int key = prefab.GetInstanceID();

        Stack<GameObject> stack = new Stack<GameObject>();

        bool stacked = cachedObjects.TryGetValue(key, out stack);
        if (stacked && stack.Count > 0) {
            Transform transform = stack.Pop().transform;
            transform.SetParent(parent);
            transform.rotation = rotation;
            transform.gameObject.SetActive(true);
            if (parent) transform.position = position;
            else transform.localPosition = position;
            IPoolable poolable = transform.GetComponent<IPoolable>();
            if (poolable != null) poolable.OnSpawn();
            return transform.gameObject;
        }
        if (!stacked) cachedObjects.Add(key, new Stack<GameObject>());

        GameObject createdPrefab = Populate(prefab, position, rotation, parent);
        cachedIds.Add(createdPrefab.GetInstanceID(), key);

        return createdPrefab;
    }

    public void Despawn(GameObject go) {
        go.SetActive(false);
        cachedObjects[cachedIds[go.GetInstanceID()]].Push(go);
        IPoolable poolable = go.GetComponent<IPoolable>();
        if (poolable != null) poolable.OnDespawn();
        if (parentPool != null) go.transform.SetParent(parentPool);
    }

    public void Dispose() {
        parentPool = null;
        cachedObjects.Clear();
        cachedIds.Clear();
    }

    GameObject Populate(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null) {
        Transform go = Object.Instantiate(prefab, position, rotation, parent).transform;
        if (parent == null) go.position = position;
        else go.localPosition = position;
        return go.gameObject;
    }
}
