using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour {

    public static IndicatorController Instance;

    [SerializeField] Color endBeatColor;
    [SerializeField] Color failedPatternColor;

    [SerializeField] GameObject[] indicators;
    List<Indicator> activeIndicators;

    [SerializeField] Transform indicatorSpawnOrigin;
    public Transform indicatorTargetMin;
    [SerializeField] Transform indicatorTargetMax;
    Vector2 indicatorTargetXRange;
    float bubbleXCenter;
    [SerializeField] float perfectRange; 

    public Animator dancer;
    public AudioClip[] vocalizations;
    [SerializeField] GameObject[] flyingTexts;

    // Use this for initialization
    void Awake () {
        if (Instance == null) Instance = this;
        else Destroy(this);
	}

    private void Start() {
        activeIndicators = new List<Indicator>();
        bubbleXCenter = (indicatorTargetMin.position.x + indicatorTargetMax.position.x) / 2;

        indicatorTargetXRange = new Vector2(
            indicatorTargetMin.position.x,
            indicatorTargetMax.position.x
        );
    }

    // Update is called once per frame
    void Update() {
        bool indicatorsInRange = false;

        int h = 0;
        int v = 0;

        if (Input.GetKey(KeyCode.RightArrow)) h++;
        if (Input.GetKey(KeyCode.LeftArrow)) h--;
        if (Input.GetKey(KeyCode.UpArrow)) v++;
        if (Input.GetKey(KeyCode.DownArrow)) v--;

        // Update the success values of each active indicator
        foreach (Indicator i in activeIndicators) {
            bool success = true;

            // if it's in the range...
            if (i.transform.position.x > indicatorTargetXRange.x && i.transform.position.x < indicatorTargetXRange.y) {
                indicatorsInRange = true;

                i.SetInputs(Input.GetKey(KeyCode.UpArrow), Input.GetKey(KeyCode.DownArrow), Input.GetKey(KeyCode.LeftArrow), Input.GetKey(KeyCode.RightArrow));
                success = i.CheckCorrectInput();

            } else success = false;

            if ((h != 0 || v != 0) && !indicatorsInRange) {
                DrumbeatController.Instance.ClearCurrentBeat();
                break;
            }

            // if it's passed the range, mark it failed
            if (i.transform.position.x <= indicatorTargetXRange.x) {
                DrumbeatController.Instance.ClearCurrentBeat();
                SpawnFlyingText(i.transform.position, 2);
                break;
            }

            if (success) {
                if (!i.destroyed) {
                    if (Mathf.Abs(bubbleXCenter - i.transform.position.x) < perfectRange) {
                        SpawnFlyingText(i.transform.position, 0);
                        MountainController.Instance.ReduceAngerPerfect();
                    } else {
                        SpawnFlyingText(i.transform.position, 1);
                    }
                }

                i.Destroyed(true);
                i.StartCoroutine("FadeOut", 0.15f);
                i.StopCoroutine("MoveToTarget");
            }

            if (indicatorsInRange && (h != 0 || v != 0)) {
                dancer.SetTrigger("Dance");
                dancer.SetFloat("Horizontal", h);
                dancer.SetFloat("Vertical", v);
            }
        }
	}

    public void SpawnFlyingText(Vector3 position, int type) {
        Instantiate(flyingTexts[type], new Vector3(position.x, position.y + 0.1f, 0), Quaternion.identity);
    }

    public void SpawnIndicator(GameObject toInstantiate, float time, bool isLast) {
        GameObject toSpawn = Instantiate(toInstantiate);
        toSpawn.transform.position = indicatorSpawnOrigin.transform.position;
        activeIndicators.Add(toSpawn.GetComponent<Indicator>());

        toSpawn.GetComponent<Indicator>().isLastInPattern = isLast;
        if (isLast) toSpawn.GetComponentInChildren<SpriteRenderer>().color = endBeatColor;
        toSpawn.GetComponent<Indicator>().StartCoroutine("MoveToTarget", time);
    }

    public GameObject RandomIndicator(bool spawnHardIndicators = false) {
        return indicators[UnityEngine.Random.Range(0, spawnHardIndicators ? indicators.Length : 4)];
    }

    public void RemoveFromActive(Indicator i) {
        if(activeIndicators.Contains(i)) activeIndicators.Remove(i);
    }

    public void ClearAllIndicators() {
        foreach(Indicator i in activeIndicators) {
            i.GetComponentInChildren<SpriteRenderer>().color = failedPatternColor;
            i.StartCoroutine("FadeOut", 0.25f);
            i.StopCoroutine("MoveToTarget");
        }

        activeIndicators.Clear();
    }

    public void DoSuccessfulEndPattern() {
        dancer.SetTrigger("Flip");
        ScreenShaker.Instance.StartCoroutine("Shake", new object[2] { DrumbeatController.Instance.successShakeAmount, DrumbeatController.Instance.successShakeDuration });

        SacrificeController.Instance.DoSacrifice();
    }

    public int GetVocalizationIndex(KeyCode[] keys) {
        bool up = Array.IndexOf(keys, KeyCode.UpArrow) != -1;
        bool down = Array.IndexOf(keys, KeyCode.DownArrow) != -1;
        bool left = Array.IndexOf(keys, KeyCode.LeftArrow) != -1;
        bool right = Array.IndexOf(keys, KeyCode.RightArrow) != -1;

        if (up && !down && !left && !right) return 0;
        if (!up && down && !left && !right) return 1;
        if (!up && !down && left && !right) return 2;
        if (!up && !down && !left && right) return 3;
        if (up && left && !down && !right) return 4;
        if (up && !left && right && !down) return 5;
        if (!up && down && right && !left) return 6;
        if (!up && down && !right && left) return 7;

        return -1;
    }
}
