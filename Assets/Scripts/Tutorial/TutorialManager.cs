using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Scriptable Objects")]
    [SerializeField] private List<TutorialSteps> tutorialStep = new List<TutorialSteps>();

    [Header("Tutorial UI")]
    [Header("Tutorial Panels")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject tutorialRaycastBlack;
    [SerializeField] private GameObject speachPanel;
    [SerializeField] private GameObject handPanel;

    [Header("Tutorial Texts")]
    [SerializeField] private TMP_Text tutorialSpeachText;
    [SerializeField] private TMP_Text tutorialInfoText;

    [Header("Tutorial Buttons")]
    [SerializeField] private Button screenButton;

    [Header("Tutorial Animations")]
    [SerializeField] private Animator speachAnim;
    [SerializeField] private Animator[] chooseYourColorBalls;

    private int tutorialCount = 0;

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
        NextTutorialStep();
    }

    public void NextTutorialStep()
    {
        if (tutorialCount > tutorialStep.Capacity - 1)
        {
            tutorialPanel.SetActive(false);
        }
        else
        {
            screenButton.enabled = tutorialStep[tutorialCount].SpeachActive;
            speachPanel.SetActive(tutorialStep[tutorialCount].SpeachActive);
            handPanel.SetActive(!tutorialStep[tutorialCount].SpeachActive);

            tutorialSpeachText.text = tutorialStep[tutorialCount].SpeachString;
            tutorialInfoText.text = tutorialStep[tutorialCount].InfoString;
            speachAnim.Play(0);
            if (!tutorialStep[tutorialCount].SpeachActive)
            {
                GameManager.Instance.directionalLight.cullingMask = ~(7);
                GameManager.Instance.onClickableCandy = true;
                tutorialRaycastBlack.SetActive(false);
            }
        }
        tutorialCount++;
    }

    public IEnumerator ChooseYourColorBallAnimation()
    {
        int rnd = Random.RandomRange(0, chooseYourColorBalls.Length);
        chooseYourColorBalls[rnd].Play(0);
        yield return new WaitForSeconds(1f);
        StartCoroutine(ChooseYourColorBallAnimation());
    }
}