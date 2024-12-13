using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    //C# event
    public delegate void LoadingScreenDelegate(float progress);
    public static LoadingScreenDelegate Loadingbar;

    [Header("LoadingScreen")]
    [SerializeField] private GameObject loadingScreen;
    private GameObject instantiatedGameobject;

    public void SceneSwitch(string LevelToLoad)
    {
        //assign and spawn in the canvas
        instantiatedGameobject = Instantiate(loadingScreen);
        //start coroutine and give a level to load from this function
        StartCoroutine(LoadLevelASync(LevelToLoad));
    }
    IEnumerator LoadLevelASync(string LevelToLoad)
    {
        //assign the loading scene and load it while current scene is still running
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(LevelToLoad);
        //while the loading screen is still loading
        while (!loadOperation.isDone)
        {
            //limit the progress value between 0 and 0.9
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            //if loadingbar event has subscribers give them the value of progress(of the loading screen/operation)
            if (Loadingbar != null)
            {
                Loadingbar(progress);
            }
            yield return null;
        }
        //when we are done with the loading screen we want it destroyed
        Destroy(instantiatedGameobject);
    }
}
//dit script zit op de subscriber(Het UI object)
public class LoadingBarUI : MonoBehaviour
{
    private Image loadingBar;

    private void OnEnable()
    {
        //subscribe to the loadingscreen and get this objects Image
        LoadingScreen.Loadingbar += UpdateUI;
        loadingBar = GetComponent<Image>();
    }
    private void OnDisable()
    {
        //unsubscribe
        LoadingScreen.Loadingbar -= UpdateUI;
    }
    private void UpdateUI(float progress)
    {
        //fill the loadingbar with the progress of the loading scene
        loadingBar.fillAmount = progress;
    }
}