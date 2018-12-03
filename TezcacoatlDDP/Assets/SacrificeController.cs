using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeController : MonoBehaviour {

    public static SacrificeController Instance;

    [SerializeField] Transform sacrificeParent;
    public Transform newSacrificeOrigin;
    [SerializeField] Transform sacrificeTop;

    [SerializeField] GameObject sacrificeObject;
    [SerializeField] GameObject flyingSacrificeObject;

    List<GameObject> sacrificeLine;

    [SerializeField] Vector3 stepUpDelta;

    [SerializeField] Sprite[] flyingSacrificeSprites;
    [SerializeField] float waitBetweenSteps;

    [SerializeField] int initSacrificeCount;

    [HideInInspector] public bool locked;

    [SerializeField] AudioClip flingSound;
    [SerializeField] AudioClip reliefSound;
    [SerializeField] AudioClip[] screamSounds;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void SetupSacrifices() {
        sacrificeLine = new List<GameObject>();

        for(int i = 0; i < initSacrificeCount; i++) {
            Vector3 thisSacrificePosition = newSacrificeOrigin.position + (stepUpDelta * i);
            sacrificeLine.Add(Instantiate(sacrificeObject, thisSacrificePosition, Quaternion.identity, sacrificeParent));
        }

        foreach(GameObject g in sacrificeLine) {
            g.GetComponent<Sacrifice>().desiredPos = g.transform.position;
        }
    }

    public void SpawnNewSacrifice() {
        GameObject g = Instantiate(sacrificeObject, newSacrificeOrigin.transform.position, Quaternion.identity, sacrificeParent);
        g.GetComponent<Sacrifice>().desiredPos = newSacrificeOrigin.transform.position;

        sacrificeLine.Insert(0, g);
    }

    public IEnumerator DoStepUp() {
        SpawnNewSacrifice();

        for(int i = sacrificeLine.Count - 1; i > 0; i--) {
            sacrificeLine[i].GetComponent<Animator>().SetTrigger("StepUp");
            sacrificeLine[i].GetComponent<Sacrifice>().desiredPos += stepUpDelta;
            sacrificeLine[i].GetComponent<Sacrifice>().DoStepUp();

            yield return new WaitForSeconds(waitBetweenSteps);
        }

        locked = false;
    }

    public void DoSacrifice() {
        locked = true;

        ScoreController.Instance.AddScore();
        ScreenShaker.Instance.Invoke("ShakeInSeconds", 1.75f);

        Camera.main.GetComponent<AudioSource>().PlayOneShot(flingSound);
        Camera.main.GetComponent<AudioSource>().PlayOneShot(screamSounds[Random.Range(0, screamSounds.Length)], 0.25f);

        GameObject sac = sacrificeLine[sacrificeLine.Count - 1];

        sacrificeLine.RemoveAt(sacrificeLine.Count - 1);

        Sacrifice s = Instantiate(flyingSacrificeObject, sac.transform.position, Quaternion.identity, sacrificeParent).GetComponent<Sacrifice>();
        s.GetComponentInChildren<SpriteRenderer>().sprite = flyingSacrificeSprites[Random.Range(0, flyingSacrificeSprites.Length)];
        s.DoFly();

        Destroy(sac);

        StartCoroutine("DoStepUp");
    }

    public void GoFree() {
        locked = true;

        Camera.main.GetComponent<AudioSource>().PlayOneShot(reliefSound);

        GameObject sac = sacrificeLine[sacrificeLine.Count - 1];

        sacrificeLine.RemoveAt(sacrificeLine.Count - 1);

        sac.GetComponent<Animator>().SetTrigger("GoFree");

        Destroy(sac, 1.5f);

        StartCoroutine("DoStepUp");
    }

    public void AllDoJump() {
        foreach(GameObject g in sacrificeLine) {
            g.GetComponent<Animator>().SetTrigger("Jump");
        }
    }
}
