using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] AudioClip clickSound;
    public void SwitchToGame()
    {
        AudioManager.Instance.PlayAudioClip(clickSound, transform, .4f);
        StartCoroutine(DelayedSceneSwitch());
        SceneManager.LoadScene("Game");
    }

    public void SwitchToMainMenu()
    {
        AudioManager.Instance.PlayAudioClip(clickSound, transform, .4f);
        StartCoroutine(DelayedSceneSwitch());
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        AudioManager.Instance.PlayAudioClip(clickSound, transform, .4f);
        StartCoroutine(DelayedSceneSwitch());
        Application.Quit();
    }

    private IEnumerator DelayedSceneSwitch()
    {
        yield return new WaitForSeconds(1.5f);
    }
}
