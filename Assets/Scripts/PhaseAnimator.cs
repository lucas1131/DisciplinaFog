using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhaseAnimator : MonoBehaviour {

    public Sprite playerPhase;
    public Sprite enemyPhase;
    public Sprite allyPhase;

    public static bool PlayAnimation = false;
    public static bool animationIsPlaying = false;

	// Use this for initialization
	void Start () {
        OnEnable();
	}

    void OnEnable(){
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(1280, 0);
    }
	
	// Update is called once per frame
	void Update (){
        if(PlayAnimation){
            // this.GetComponent<Image>().enabled = true;
            if(BoardManager.turn == BoardManager.Turn.Player)
                this.GetComponent<Image>().sprite = playerPhase;
            else if(BoardManager.turn == BoardManager.Turn.Enemy)
                this.GetComponent<Image>().sprite = enemyPhase;
            else if(BoardManager.turn == BoardManager.Turn.Ally)
                this.GetComponent<Image>().sprite = allyPhase;

            play();
		}// else this.GetComponent<Image>().enabled = false;
	}

    public void play(){
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(1280, 0);

        StartCoroutine(movement(new Vector2(-5f, -5f)));
        PlayAnimation = false;
    }

    IEnumerator movement(Vector2 target){

        animationIsPlaying = true;

        while(Vector2.Distance(this.GetComponent<RectTransform>().anchoredPosition,target) > 0.07f){
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(this.GetComponent<RectTransform>().anchoredPosition, target, 20f);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 target2 = new Vector2(-1280, 0);

        while (Vector2.Distance(this.GetComponent<RectTransform>().anchoredPosition, target2) > 0.07f){
            this.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(this.GetComponent<RectTransform>().anchoredPosition, target2, 20f);
            yield return null;
        }
     
        animationIsPlaying = false;
    }
}
