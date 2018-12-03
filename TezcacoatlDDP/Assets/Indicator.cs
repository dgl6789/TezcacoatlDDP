using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour {

    public KeyCode[] Keys;
    public bool isLastInPattern;

    [HideInInspector] public bool destroyed = false;
    public bool EnteredRange = false;

    bool up, down, left, right;

    public IEnumerator MoveToTarget(float timeToMove) {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1) {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, IndicatorController.Instance.indicatorTargetMin.position, t);
            yield return null;
        }

        StartCoroutine("FadeOut", 0.25f);
    }

    IEnumerator FadeOut(float fadeTime) {
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        float t = 0f;

        while(t < 1) {
            t += Time.deltaTime / fadeTime;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1 - t);

            yield return null;
        }

        IndicatorController.Instance.RemoveFromActive(this);
        Destroy(gameObject);
    }

    public bool CheckCorrectInput() {
        bool success = true;

        if (Array.IndexOf(Keys, KeyCode.RightArrow) == -1) {
            if (right) success = false;
        } else {
            if (!right) success = false;
        }

        if (Array.IndexOf(Keys, KeyCode.LeftArrow) == -1) {
            if (left) success = false;
        } else {
            if (!left) success = false;
        }

        if (Array.IndexOf(Keys, KeyCode.UpArrow) == -1) {
            if (up) success = false;
        } else {
            if (!up) success = false;
        }

        if (Array.IndexOf(Keys, KeyCode.DownArrow) == -1) {
            if (down) success = false;
        } else {
            if (!down) success = false;
        }

        return success;
    }

    public void SetInputs(bool up, bool down, bool left, bool right) {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }

    public void Destroyed(bool isSuccess) {
        if (!destroyed) {
            destroyed = true;
            if (isSuccess) {
                // play success animation, queue successful drumbeat
                DrumbeatController.Instance.source.PlayOneShot(IndicatorController.Instance.vocalizations[IndicatorController.Instance.GetVocalizationIndex(Keys)]);
            }

            if (isLastInPattern) {
                if (isSuccess) {
                    // this pattern was a success, fling the current sacrifice
                    // and reduce the volcano's anger
                    IndicatorController.Instance.DoSuccessfulEndPattern();
                    MountainController.Instance.ReduceAnger();
                }

                DrumbeatController.Instance.QueuePatternAfterBaseTime();
            }
        }
    }
}
