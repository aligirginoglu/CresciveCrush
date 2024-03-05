using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public List<Ball> AllBall = new List<Ball>();
    public Ball Candy;

    [Header("Balls")]
    public GameObject[] startPosArray;
    public PositionData[] positionsArray = new PositionData[9];
    public BallPrefab[] scriptBallPrefabs;

    [System.Serializable]
    public class PositionData
    {
        public GameObject[] positions;
    }

    public static Action OnGameStart;
    public static Action OnChooseBall;

    private void OnDisable()
    {
        OnGameStart -= SpawnBall;
        OnChooseBall -= LayerOnChooseBall;
    }

    private void Start()
    {
        Physics.gravity = new Vector3(0F, -100F, 0F);
        scriptBallPrefabs = FindObjectsOfType<BallPrefab>();

        for (int i = 0; i < startPosArray.Length; i++)
        {
            for (int j = 0; j < positionsArray[i].positions.Length; j++)
            {
                positionsArray[i].positions[j] = startPosArray[i].transform.GetChild(j).gameObject;
            }
        }

        OnGameStart += SpawnBall;
        OnChooseBall += LayerOnChooseBall;
    }

    private async void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.onClickableCandy)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.GetComponent<BallPrefab>() != null)
                {
                    if (hit.transform.gameObject.GetComponent<BallPrefab>().type == BallType.Candy)
                    {
                        GameManager.Instance.directionalLight.cullingMask = ~(7);
                        hit.transform.gameObject.transform.GetChild(0).gameObject.transform
                            .DOScale(hit.transform.gameObject.transform.GetChild(0).gameObject.transform.localScale * 3f, .5f).SetEase(Ease.InElastic);
                        hit.transform.gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = true;
                        TutorialManager.Instance.NextTutorialStep();
                        GameManager.Instance.onClickableCandy = false;
                        await Task.Delay(250);
                        GameManager.Instance.choosePanel.SetActive(true);
                        StartCoroutine(TutorialManager.Instance.ChooseYourColorBallAnimation());
                    }
                }
            }
        }
    }

    private async void LayerOnChooseBall()
    {
        GameObject candyGO = null;
        foreach (var candy in scriptBallPrefabs)
        {
            if (candy.gameObject.GetComponent<BallPrefab>().type == BallType.Candy)
            {
                candyGO = candy.gameObject;
            }
        }

        foreach (var item in scriptBallPrefabs)
        {
            await Task.Delay(10);
            if (item.gameObject.GetComponent<BallPrefab>().type == GameManager.Instance.selectBallType)
            {
                OnCompleteDoTweenAnimation();

                foreach (var ball in AllBall)
                {
                    if (ball.Type == item.type)
                    {
                        GameObject particleEffect = Instantiate(ball.ExplotionsEffect, item.transform);
                        particleEffect.GetComponent<ParticleSystem>().startColor = ball.ExplotionColor;
                    }
                }
                item.gameObject.transform.GetChild(0).gameObject.layer = 6;

                item.transform.GetChild(0).gameObject.transform
                    .DOScale(item.transform.GetChild(0).gameObject.transform.localScale * 1.2f, .5f).SetEase(Ease.InElastic)
                    .OnComplete(() => item.transform.GetChild(0).gameObject.transform
                    .DOScale(item.transform.GetChild(0).gameObject.transform.localScale / 1.2f, .3f).SetEase(Ease.InOutCirc))
                    .OnComplete(() => BallMoveToCandy(item, candyGO));
            }

            item.GetComponent<Rigidbody>().constraints =
                RigidbodyConstraints.FreezePositionX |
                RigidbodyConstraints.FreezePositionZ |
                RigidbodyConstraints.FreezeRotation;
        }
    }

    private void BallMoveToCandy(BallPrefab item, GameObject candyGO)
    {
        item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

        item.gameObject.transform.GetChild(0).transform
            .DOMove(candyGO.transform.position, .5f).SetEase(Ease.InFlash)
            .OnComplete(() => item.transform.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false)
            .OnComplete(() => candyGO.transform.GetChild(0).transform
            .DOScale(candyGO.transform.GetChild(0).transform.localScale * 1.1f, .1f)
            .OnComplete(() => candyGO.transform.GetChild(0).transform
            .DOScale(candyGO.transform.GetChild(0).transform.localScale / 1.1f, .1f)
            ));
    }

    private async void SpawnBall()
    {
        await DestroyBallControl();
    }

    private async Task DestroyBallControl()
    {
        await DestroyBalls();
        await SpawnBallAgainControl();
    }

    private void OnCompleteDoTweenAnimation()
    {
        AudioManager.Instance.CollectStartSound().Play();
    }

    private async Task DestroyBalls()
    {
        await Task.Delay(200);

        foreach (var scriptBallPrefab in scriptBallPrefabs)
        {
            if (scriptBallPrefab.GetComponent<BallPrefab>().type == GameManager.Instance.selectBallType)
            {
                Destroy(scriptBallPrefab.gameObject, 0.4f);
            }
            if (scriptBallPrefab.GetComponent<BallPrefab>().type == BallType.Candy)
            {
                Instantiate(Candy.ExplotionsEffect, scriptBallPrefab.transform);
                scriptBallPrefab.transform.GetChild(0).gameObject.SetActive(false);
                Destroy(scriptBallPrefab.gameObject, 0.8f);
            }
        }

        if (!AudioManager.Instance.CollectEndSound().isPlaying)
        {
            AudioManager.Instance.CollectEndSound().Play();
        }

        await Task.Delay(150);
        scriptBallPrefabs = FindObjectsOfType<BallPrefab>();
    }

    private async Task SpawnBallAgainControl()
    {
        await Task.Delay(500);

        for (int i = 0; i < 9; i++)
        {
            SpawnBallAgain(positionsArray[i], startPosArray[i]);
        }
        GameManager.Instance.directionalLight.cullingMask = 1;
    }

    private async void SpawnBallAgain(PositionData obj, GameObject pos)
    {
        await Task.Delay(150);
        int total = 0;
        for (int i = 0; i < obj.positions.Length; i++)
        {
            if (obj.positions[i])
            {
                foreach (var item in obj.positions)
                {
                    total += 1;
                }
            }
            else
            {
                int rnd = UnityEngine.Random.RandomRange(0, 6);
                GameObject ball = AllBall[rnd].Prefab.gameObject;
                ball.GetComponent<BallPrefab>().type = AllBall[rnd].Type;
                GameObject spawnBall = Instantiate(ball, pos.transform);
                Instantiate(AllBall[rnd].Model, spawnBall.transform);
                obj.positions[i] = spawnBall;
            }
        }
    }
}