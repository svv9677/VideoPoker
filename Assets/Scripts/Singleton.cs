using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T mInstance = null;

    public static T Instance
    {
        get
        {
            if (null == mInstance)
            {
                mInstance = (T)FindObjectOfType(typeof(T));

                if (null == mInstance)
                {
                    Debug.LogWarning("Singleton object not found. Please make sure there is a Game Object in the scene!");
                }
            }

            return mInstance;
        }
    }

    public static bool HasInstance
    {
        get
        {
            return mInstance != null;
        }
    }

    public void Destroy()
    {
        if (mInstance == this)
            mInstance = null;
    }

    protected virtual void Awake()
    {
        if (mInstance == null)
            mInstance = this as T;
    }

    protected virtual void OnDestroy()
    {
        Destroy();
    }
}
