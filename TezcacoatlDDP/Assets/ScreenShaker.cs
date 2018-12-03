using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShaker : MonoBehaviour {

    public static ScreenShaker Instance;

    Vector3 initPosition;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() {
        initPosition = new Vector3(0, 0, -10);
    }

    public void ShakeInSeconds() {
        StartCoroutine("Shake", new object[2] { 0.1f, 0.2f });
    }

    public IEnumerator Shake(object[] amountAndDuration) {
        if (amountAndDuration.Length != 2) yield break;

        float amount = (float)amountAndDuration[0];
        float duration = (float)amountAndDuration[1];
        float t = 0f;

        while (t < 1) {
            t += Time.deltaTime / duration;
            Vector3 newPos = (Vector3)Random.insideUnitCircle * Mathf.Lerp(amount, 0, t);
            newPos.z = initPosition.z;
            transform.position = newPos;
            yield return null;
        }

        transform.position = initPosition;
    }
}
