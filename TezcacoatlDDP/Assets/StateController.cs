using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    public static StateController Instance;

    Animator anim;

    public int State = 1;

    Vector3 desiredStatePos;
    [SerializeField] float smoothing;

    [SerializeField] AudioClip rumble;
    [SerializeField] AudioClip titleTrack;

	// Use this for initialization
	void Awake () {
        if (Instance == null) Instance = this;
        else Destroy(this);
	}
	
	// Update is called once per frame
	void Start () {
        anim = GetComponent<Animator>();

        SwapState(0);
	}

    private void Update() {
        if(Vector3.Distance(transform.position, desiredStatePos) > 0.1f) {
            transform.position = Vector3.Lerp(transform.position, desiredStatePos, Time.deltaTime * smoothing);
        }
    }

    public void SwapState(int state) {
        State = state;

        switch (state) {
            case 0:
                desiredStatePos = new Vector3(0, 30, -10);
                MountainController.Instance.targetVolume = 1f;
                break;
            case 1:
                desiredStatePos = new Vector3(0, 0, -10);
                CrossfadeStart();
                Invoke("GoToGame", 1.5f);

                SacrificeController.Instance.SetupSacrifices();

                // reset the game state
                DrumbeatController.Instance.StartGame();
                break;
            case 2:
                Invoke("CrossfadeStart", 3.5f);
                Invoke("GoToTitle", 5);
                break;
        }
    }

    void GoToTitle() {
        SwapState(0);
        MountainController.Instance.GetComponent<AudioSource>().clip = titleTrack;
        MountainController.Instance.GetComponent<AudioSource>().Play();
        MountainController.Instance.targetVolume = 1f;
    }

    void GoToGame() {
        MountainController.Instance.GetComponent<AudioSource>().clip = rumble;
        MountainController.Instance.GetComponent<AudioSource>().Play();
    }

    void CrossfadeStart() {
        MountainController.Instance.targetVolume = 0;
    }
}
