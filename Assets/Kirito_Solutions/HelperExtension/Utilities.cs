using System;
using Kirito_Solutions.AdvanceSingleTon;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Kirito_Solutions.HelperExtension
{
    public class Utilities : SingleTon<Utilities> { }

    public class Helper : MonoBehaviour
    {
        public static List<GameObject> CreatePool(GameObject _prefab, int _count, bool _isActive = false, Transform _parent = null, List<GameObject> _poolList = null)
        {
            List<GameObject> poolList;
            if (_poolList != null)
            {
                poolList = _poolList;
            }
            else
            {
                poolList = new List<GameObject>();
            }

            for (int i = 0; i < _count; i++)
            {
                GameObject newObject;
                if (_parent)
                {
                    newObject = Instantiate(_prefab, _parent);
                }
                else
                {
                    newObject = Instantiate(_prefab);
                }
                newObject.SetActive(_isActive);
                poolList.Add(newObject);
            }
            return poolList;
        }

        public static GameObject GetObjectFromPool<T>(List<GameObject> _poolList, bool _inactive = true, Predicate<GameObject> _match = null)
        {
            GameObject objectFromPool;
            if (_match != null)
            {
                objectFromPool = _poolList.Find(_match);
            }
            else
            {
                objectFromPool = _poolList.Find(_x => _x.gameObject.activeInHierarchy == _inactive);
            }
            if (objectFromPool == null)
            {
                CreatePool(_poolList[0], 2, _poolList: _poolList);
            }
            return objectFromPool;
        }

    }
}
