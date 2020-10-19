using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public sealed class LoadHelper : MonoBehaviour
    {
        private static LoadHelper Instance;

        void Awake()
        {
            Instance = this;
        }

        public static Coroutine Start(IEnumerator func)
        {
            return Instance.StartCoroutine(func);
        }

        public static void StopAll(IEnumerator func)
        {
            Instance.StopAllCoroutines();
        }

        public static void StopOne(Coroutine task)
        {
            Instance.StopCoroutine(task);
        }
    }

}