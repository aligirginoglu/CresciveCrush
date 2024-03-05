using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Transforms")]
    [SerializeField] private Transform gameIn;

    [Header("Prefabs")]
    [SerializeField] private GameObject ballManagerPrefab;

    [Header("Lights")]
    [SerializeField] public Light directionalLight;

    [Header("Other")]
    public BallType selectBallType;
    public GameObject choosePanel;
    public bool onClickableCandy = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Instantiate(ballManagerPrefab, gameIn);
    }

    public async void OnSelectDestroyBallType(int typeIndex)
    {
        choosePanel.SetActive(false);

        selectBallType = (BallType)Enum.GetValues(typeof(BallType)).GetValue(typeIndex);

        BallManager.OnChooseBall();

        await Task.Delay(3000);
        BallManager.OnGameStart();
    }
}