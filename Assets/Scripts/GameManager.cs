using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool isMobile;
    public bool IsMobile => isMobile;
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                return null;

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!isMobile)
        {
            FindObjectOfType<FixedJoystick>().gameObject.SetActive(false);
        }
    }
}
