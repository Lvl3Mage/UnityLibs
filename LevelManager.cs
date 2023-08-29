using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    static LevelManager instance;

    void Awake(){
        if(instance != null){
            Debug.LogError("An instance of LevelManager already exists!");
            return;
        }
        instance = this;
    }
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = 1;
    [SerializeField] bool clearSlowmotion = true;
    
    void Start(){
        StartCoroutine(Fade(false));
    }
    public static void LoadScene(int sceneIndex){
        StartCoroutine(FadeToScene(sceneIndex));
    }
    public static void ReloadScene(){
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public static void LoadNextScene(){
        LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    IEnumerator FadeToScene(int levelindex){
        //wait until all fading is complete
        while(fading){
            yield return null;
        }

        //fade in
        yield return Fade(true);

        //change scene
        SceneManager.LoadScene(levelindex);
        if(clearSlowmotion){
            SlowMotion.SpeedUp();
        }
    }
    bool fading = false;
    IEnumerator Fade(bool toggle){
        fading = true;
        canvasGroup.blocksRaycasts = toggle;
        canvasGroup.interactable = toggle;

        float targetAlpha = toggle ? 1f : 0f;
        float startingAlpha = 1 - targetAlpha;
        canvasGroup.alpha = startingAlpha;


        float elapsed = 0;
        while(canvasGroup.alpha != targetAlpha){
            yield return null;
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, elapsed/fadeTime);
        }
        fading = false;
    }
}
