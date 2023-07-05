using UnityEngine;
using System;
using System.Collections.Generic;

public class BugaUIManager : MonoBehaviour
{
    public static BugaUIManager Instance { get; private set; }

    public Canvas canvas;
    public List<UIScreenSession> screenSessions;
    private int currentScreenSessionIndex = 0;


    [SerializeField]
    private List<GameObject> currentScreenSessionScreens;

    public string CurrentScreenSessionName
    {
        get { return $"{CurrentScreenSession.name}: {string.Join(", ", currentScreenSessionScreens.ConvertAll(s => s.name))}"; }
    }

    public UIScreenSession CurrentScreenSession
    {
        get { return screenSessions[currentScreenSessionIndex]; }
    }

    public event Action<string, bool> ScreenToShowChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Reposiciona as telas para o centro do canvas
        CenterScreensOnCanvas();

        // Define a primeira sessão de telas como a sessão atual
        SetCurrentScreenSession(currentScreenSessionIndex);
    }

    public void ChangeScreenSession(int sessionIndex)
    {
        if (sessionIndex >= 0 && sessionIndex < screenSessions.Count)
        {
            SetCurrentScreenSession(sessionIndex);
        }
    }

    public void ShowScreen(string screenName)
    {
        foreach (GameObject screen in currentScreenSessionScreens)
        {
            bool show = screen.name.Equals(screenName);
            screen.SendMessage("ToShow", show);
            ScreenToShowChanged?.Invoke(screen.name, show);
        }
    }

    public void ShowScreenSession(string sessionName)
    {
        foreach (UIScreenSession session in screenSessions)
        {
            if (session.name.Equals(sessionName))
            {
                SetCurrentScreenSession(screenSessions.IndexOf(session));
                return;
            }
        }
    }

    public void HideScreenSession(string sessionName)
    {
        foreach (UIScreenSession session in screenSessions)
        {
            if (session.name.Equals(sessionName))
            {
                DeactivateScreens(session.screensToShow, session.screensToHide);
                return;
            }
        }
    }

    private void ActivateScreens(List<GameObject> screensToShow, List<GameObject> screensToHide)
    {
        // Desativa todas as telas da sessão atual
        foreach (GameObject screen in currentScreenSessionScreens)
        {
            screen.SetActive(false);
        }

        // Ativa as telas a serem mostradas
        foreach (GameObject screen in screensToShow)
        {
            screen.SetActive(true);
            screen.SendMessage("ToShow", true);
            ScreenToShowChanged?.Invoke(screen.name, true);

            Debug.Log($"Screen activated: {screen.name}");
        }

        // Oculta as telas a serem ocultadas
        foreach (GameObject screen in screensToHide)
        {
            //screen.SetActive(false);
            screen.SendMessage("ToShow", false);
            ScreenToShowChanged?.Invoke(screen.name, false);

            Debug.Log($"Screen hidden: {screen.name}");
        }

        // Atualiza a lista de telas da sessão atual
        currentScreenSessionScreens = screensToShow;
    }

    private void DeactivateScreens(List<GameObject> screensToShow, List<GameObject> screensToHide)
    {
        // Oculta as telas a serem mostradas
        foreach (GameObject screen in screensToShow)
        {
            screen.SendMessage("ToShow", false);
            ScreenToShowChanged?.Invoke(screen.name, false);

            Debug.Log($"Desativando tela: {screen.name}");

        }

        // Ativa as telas a serem ocultadas
        foreach (GameObject screen in screensToHide)
        {
            screen.SendMessage("ToShow", true);
            ScreenToShowChanged?.Invoke(screen.name, true);

            Debug.Log($"Ativando tela: {screen.name}");
        }
    }

    private void CenterScreensOnCanvas()
    {
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        foreach (UIScreenSession screenSession in screenSessions)
        {
            foreach (GameObject screen in screenSession.screensToShow)
            {
                RectTransform rectTransform = screen.GetComponent<RectTransform>();

                // Reposiciona a tela para o centro do canvas
                rectTransform.SetParent(canvasRectTransform, false);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;

                // Reatribui o objeto pai original
                rectTransform.SetParent(screen.transform.parent, true);
            }
        }
    }

    private void SetCurrentScreenSession(int sessionIndex)
    {
        foreach (UIScreenSession session in screenSessions)
        {
            session.isCurrentSession = false;
        }

        UIScreenSession currentScreenSession = screenSessions[sessionIndex];
        currentScreenSession.isCurrentSession = true;
        currentScreenSessionScreens = currentScreenSession.screensToShow;
        ActivateScreens(currentScreenSession.screensToShow, currentScreenSession.screensToHide);
    }
}

[System.Serializable]
public class UIScreenSession
{
    public string name;
    public List<GameObject> screensToShow;
    public List<GameObject> screensToHide;
    public bool isCurrentSession; // Propriedade para indicar se é a sessão atual
}