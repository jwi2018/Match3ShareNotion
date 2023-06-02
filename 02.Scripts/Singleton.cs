using UnityEngine;

namespace Logic
{
    public class Singleton<T> where T : class, new()
    {
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }

                return instance;
            }
        }

        public virtual void Init()
        {
        }

        public virtual void Destroy()
        {
            instance = null;
        }
    }
}
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    public static T GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;

                if (instance == null) return null;
            }

            return instance;
        }
    }
}