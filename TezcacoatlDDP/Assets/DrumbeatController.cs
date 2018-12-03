using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumbeatController : MonoBehaviour {

    public static DrumbeatController Instance;

    [SerializeField] float difficulty;
    [SerializeField] float difficultyScale;

    [SerializeField] float patternLengthScale;
    [SerializeField] float patternDifficultyThreshold;

    List<GameObject> currentPattern;

    [SerializeField] float[] possibleBeatTimes;

    // Time it takes for an indicator to travel down the line
    [SerializeField] float IndicatorBaseSpeed;

    [SerializeField] float BeatTimer;
    float initBeatTimer;
    [SerializeField] float SpeedScale;
    float t;

    [SerializeField] float baseTimeBetweenPatterns;

    public Animator drummer;
    [SerializeField] AudioClip beat;
    public AudioSource source;

    [SerializeField] float failShakeAmount;
    [SerializeField] float failShakeDuration;

    public float successShakeAmount;
    public float successShakeDuration;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
        initBeatTimer = baseTimeBetweenPatterns * 2;
    }

    public void StartGame() {
        if (StateController.Instance.State == 1) {
            MountainController.Instance.Reset();

            currentPattern = new List<GameObject>();
            drummer.SetFloat("SpeedMultiplier", 1 / BeatTimer);
            t = BeatTimer;

            difficulty = 0;
            Invoke("QueueNewPattern", initBeatTimer);
        }
    }

    // Update is called once per frame
    void Update () {
        if (StateController.Instance.State == 1) {
            difficulty += Time.deltaTime * difficultyScale;
            MountainController.Instance.IncreaseAnger();

            if (currentPattern.Count > 0) {
                if (t > 0) {
                    t -= Time.deltaTime;
                    if (t <= 0) {
                        SpawnNextIndicator();
                        source.PlayOneShot(beat);

                        if (currentPattern.Count == 0) {
                            drummer.SetFloat("SpeedMultiplier", 0);
                        } else {
                            SetBeat();
                        }
                    }
                }
            }
        }
	}

    public void ClearCurrentBeat() {
        ScreenShaker.Instance.StartCoroutine("Shake", new object[2] { failShakeAmount, failShakeDuration});
        SacrificeController.Instance.GoFree();

        IndicatorController.Instance.ClearAllIndicators();

        currentPattern.Clear();
        QueuePatternAfterBaseTime();
    }

    public void QueueNewPattern() {
        currentPattern = new List<GameObject>();

        int l = 3 + (Mathf.FloorToInt(difficulty / patternLengthScale));

        for (int i = 0; i < l; i++) {
            currentPattern.Add(IndicatorController.Instance.RandomIndicator(difficulty > patternDifficultyThreshold));
        }

        SetBeat();
    }

    void SetBeat() {
        BeatTimer = possibleBeatTimes[Random.Range(0, possibleBeatTimes.Length)];
        drummer.SetFloat("SpeedMultiplier", 1 / BeatTimer);
        t = BeatTimer;
    }

    public void QueuePatternAfterBaseTime() {
        Invoke("QueueNewPattern", baseTimeBetweenPatterns);
    }

    public void SpawnNextIndicator() {
        IndicatorController.Instance.SpawnIndicator(currentPattern[0], IndicatorBaseSpeed - Mathf.Clamp((SpeedScale * difficulty), 0, IndicatorBaseSpeed + SpeedScale), currentPattern.Count == 1);
        currentPattern.RemoveAt(0);
    }
}
