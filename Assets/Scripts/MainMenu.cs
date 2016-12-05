using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public int index;

    public Image FadeImg;
    public float fadeSpeed = 1.5f;

    public float minAlpha = 0.05f;
    public float maxAlpha = 0.90f;

    public float FADEIN_TIMER = -5f;
    public float START_TIMER = 15f;
    public float timer = 5.0f;
    public bool isFading = false;
    public bool stop;

    void Awake() {
        FadeImg.rectTransform.localScale = new Vector2(
            Screen.width,
            Screen.height
        );
        stop = false;
        FadeImg.color = new Color(0f, 0f, 0f, 0f);
    }

    void Start(){
    	index = SceneManager.GetActiveScene().buildIndex + 1;
    }

    void Update() {

    	if(Input.GetButtonDown("Start"))
			StartCoroutine("LoadLevel", index);

        // timer -= Time.deltaTime;
        // if (!stop) {
        //     if ((timer <= 0) && !isFading){

        //         FadeImg.gameObject.SetActive(true);
        //         isFading = true;

        //         // start fade
        //         StartCoroutine("FadeRoutine");
        //     }

        //     if ((timer <= -1.3f) && isFading){

        //         // start fade
        //         StartFadeOut();
        //     }
        // }
    }

    void FadeOut() {
        FadeImg.color = Color.Lerp(
            FadeImg.color,
            Color.clear,
            fadeSpeed*Time.deltaTime
        );
    }


    void FadeIn(){
        FadeImg.color = Color.Lerp(
            FadeImg.color,
            Color.black,
            fadeSpeed * Time.deltaTime
        );
    }

    void FadeIn(float speed){
        FadeImg.color = Color.Lerp(
            FadeImg.color,
            Color.black,
            speed * Time.deltaTime
        );
    }


    public void StartFadeOut() {
        
        // Fade the texture to clear
        FadeOut();
        
        if (FadeImg.color.a <= minAlpha) {
            FadeImg.color = Color.clear;
            FadeImg.enabled = false;

            isFading = false;
            timer = START_TIMER - Random.Range(0f, 3f);
        }
    }


    public IEnumerator FadeRoutine() {
        
        // Make sure the image is enabled
        FadeImg.enabled = true;

        do {
            // Start fading in
            FadeIn();

            // Let some alpha 
            if (FadeImg.color.a >= maxAlpha) {
                yield break;

            } else yield return null;

        } while (true);
    }

    public IEnumerator FadeRoutine(float speed){

        // Make sure the image is enabled
        FadeImg.enabled = true;

        do {
            // Start fading in
            FadeIn();

            // Let some alpha 
            if (FadeImg.color.a >= maxAlpha){
                yield break;

            } else yield return null;

        } while (true);
    }

    IEnumerator LoadLevel(int index){
		StartCoroutine("FadeRoutine");
		yield return new WaitForSeconds(0.5f);
		SceneManager.LoadScene(index);
	}
}
