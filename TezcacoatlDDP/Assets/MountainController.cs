using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainController : MonoBehaviour {

    public static MountainController Instance;

    [SerializeField] Sprite[] faces;

    [SerializeField] float basePatternAngerDecreaseAmount;
    [SerializeField] float perfectBeatDecreaseAmount;
    [SerializeField] float angerIncreaseMultiplier;

    [SerializeField] SpriteRenderer face;

    [SerializeField] float eruptionShakeAmount;
    [SerializeField] float eruptionShakeDuration;

    [SerializeField] AudioClip eruptionSound;

    [SerializeField] GameObject debrisObject;
    [SerializeField] int debrisToSpawn;
    [SerializeField] Transform debrisOrigin;

    [SerializeField] Animator lavacolumn;

    // 0 - 1
    public float anger;

    [HideInInspector] public float targetVolume;

    AudioSource source;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() {
        Reset();
        source = GetComponent<AudioSource>();
    }

    private void Update() {
        if(Mathf.Abs(source.volume - targetVolume) > 0.01f) {
            source.volume = Mathf.Lerp(source.volume, targetVolume, Time.deltaTime * 3f);
        }
    }

    // Use this for initialization
    public void Reset () {
        face.sprite = faces[0];
        anger = 0f;
        targetVolume = anger;
    }
	
	// Update is called once per frame
	void UpdateFace () {
        int index = Mathf.FloorToInt(anger * (faces.Length - 1));
        face.sprite = faces[index];
	}

    public void ReduceAnger() {
        anger = Mathf.Clamp01(anger - basePatternAngerDecreaseAmount);
        UpdateFace();
        targetVolume = anger;
    }

    public void ReduceAngerPerfect() {
        anger = Mathf.Clamp01(anger - perfectBeatDecreaseAmount);
        UpdateFace();
        targetVolume = anger;
    }

    public void IncreaseAnger() {
        anger = Mathf.Clamp01(anger + Time.deltaTime * angerIncreaseMultiplier);
        UpdateFace();

        if(anger >= 1f) {
            Erupt();
        }

        targetVolume = anger;
    }

    public void Erupt() {
        ScreenShaker.Instance.StartCoroutine("Shake", new object[2] { eruptionShakeAmount, eruptionShakeDuration });
        Camera.main.GetComponent<AudioSource>().PlayOneShot(eruptionSound);
        StateController.Instance.SwapState(2);

        lavacolumn.SetTrigger("Fire");

        for(int i = 0; i < debrisToSpawn; i++) {
            Instantiate(debrisObject, debrisOrigin.position, Quaternion.identity);
        }
    }

    public IEnumerator Rumble() {
        yield return null;
    }
}
