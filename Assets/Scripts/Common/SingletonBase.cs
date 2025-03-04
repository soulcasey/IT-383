using UnityEngine;

public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
			{
				instance = (T) FindFirstObjectByType(typeof(T));
			}
			
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
		{
			instance = this as T;
		}
        else
		{
			Destroy(gameObject);
		}
    }
}