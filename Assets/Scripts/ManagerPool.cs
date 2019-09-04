﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Кастомная реализация шаблона ObjecPool
public class ManagerPool : Singleton<ManagerPool>
{
    public Dictionary<int, Pool> pools = new Dictionary<int, Pool>();

    public Pool AddPool(PoolType id, bool reparent = true)
    {
        Pool pool;
        if(pools.TryGetValue((int)id, out pool) == false){
            pool = new Pool();
            pools.Add((int)id, pool);
            if (reparent) {
                GameObject poolsGO = GameObject.Find("[POOLS]") ?? new GameObject("[POOLS]");
                GameObject poolGO = new GameObject("Pool:" + id);
                poolsGO.transform.SetParent(transform);
                poolGO.transform.SetParent(poolsGO.transform);
                pool.SetParent(poolGO.transform);
            }
        }
        return pool;
    }

    public GameObject Spawn(PoolType id, GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        return pools[(int)id].Spawn(prefab, position, rotation, parent);
    }

    public T Spawn<T>(PoolType id, GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        var value = pools[(int)id].Spawn(prefab, position, rotation, parent);
        return value.GetComponent<T>();
    }

    public void Despawn(PoolType id, GameObject go)
    {
        pools[(int)id].Despawn(go);
    }

    public void Dispose()
    {
        foreach (var poolsValue in pools.Values)
        {
            poolsValue.Dispose();
        }
        pools.Clear();
    }
}
