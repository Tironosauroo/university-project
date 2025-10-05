using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    [SerializeField]
    private GameObject MainPanel;
    [SerializeField]
    private GameObject CreditsPanel;
    [SerializeField]
    private GameObject SettingsPanel;
    public void ExitButton()
    {
        Application.Quit();
    }

    public void ShowCredits()
    {
        MainPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        CreditsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void HideSettings()
    {
        SettingsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void PlayGame()
    {
        LevelManager.LoadFirstScene();
    }
}
