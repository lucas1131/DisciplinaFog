using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseAnimator : MonoBehaviour {

    public bool PlayAnimation;

	// Use this for initialization
	void Start () {
        OnEnable();
	}

    void OnEnable()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(880, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayAnimation)
        {
            play();
        }
	}

    void play()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(880, 0);

        StartCoroutine(movement(new Vector2(0, 0)));
        PlayAnimation = false;
    }

    IEnumerator movement(Vector2 target)
    {
        while(Vector2.Distance(this.GetComponent<RectTransform>().anchoredPosition,target) > 0.05f)
        {
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(this.GetComponent<RectTransform>().anchoredPosition, target, 20f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 target2 = new Vector2(-880, 0);

        while (Vector2.Distance(this.GetComponent<RectTransform>().anchoredPosition, target2) > 0.05f)
        {
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(this.GetComponent<RectTransform>().anchoredPosition, target2, 20f);
            yield return null;
        }

    }
}
