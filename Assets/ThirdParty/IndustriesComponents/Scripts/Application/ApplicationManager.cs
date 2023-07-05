using Fusion;
using Fusion.XR;
using Photon.Voice.Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
* 
* The ApplicationManager is in charge to display error messages when an error occurs.
* So, the application manager listens the SessionEventsManager to get informed when a network error occurs.
* 
**/

namespace Fusion.Samples.IndustriesComponents
{
    public class ApplicationManager : MonoBehaviour
    {
        public Managers managers;
        public SessionEventsManager sessionEventsManager;
        public GameObject staticLevel;
        public GameObject interactableObjects;
        const string shutdownErrorMessage = "Look like we have a connection issue.";
        const string disconnectedFromServerErrorMessage = "Sorry, you have been disconnected ! \n\n Please restart the application.";
        [Header("Desktop Rig settings")]
        public GameObject desktopErrorMessageGO;
        public Fader desktopCameraFader;
        public TextMeshPro desktopErrorMessageTMP;

        [Header("XR Rig settings")]
        public GameObject hardwareRigErrorMessageGO;
        public Fader hardwareRigFader;
        public TextMeshPro hardwareRigErrorMessageTMP;

        private void Start()
        {
            if (!sessionEventsManager)
                sessionEventsManager = GetComponent<SessionEventsManager>();
            if (!sessionEventsManager)
                Debug.LogError("Can not get SessionEventsManager");

            sessionEventsManager.onDisconnectedFromServer.AddListener(DisconnectedFromServer);
            sessionEventsManager.onShutdown.AddListener(Shutdown);

            if (managers == null) managers = Managers.FindInstance();
        }

        // ShutdownWithError is called when the application is launched without an active network connection (network interface disabled or no link for example) or if an network interface failure occurs at run
        private void Shutdown(ShutdownReason shutdownReason)
        {
            Debug.LogError($" ApplicationManager Shutdown : { shutdownReason} ");
            string details = shutdownReason.ToString();
            if (details == "Ok") details = "Connection lost";
            // The runner will be destroyed, as we launch a coroutine, we want to survive :)
            transform.parent = null;
            UpdateErrorMessage(shutdownErrorMessage + $"\n\nCause: {details}");
            StartCoroutine(CleanUpScene());

        }

        // DisconnectedFromServer is called when the internet connection is lost.
        private void DisconnectedFromServer()
        {
            UpdateErrorMessage(disconnectedFromServerErrorMessage);
            StartCoroutine(CleanUpScene());
        }

        // DestroyNetworkedObjects is called when the connection erros occurs in order to delete spawned objects
        private void DestroyNetworkedObjects()
        {
            // Destroy the runner to delete Network objects (bots)
            if (managers.runner)
            {
                // restore the Unity Physics engine if the runner must be destroyed because it can not handle the physic anymore
                if (managers.runner.Config.PhysicsEngine != NetworkProjectConfig.PhysicsEngines.None)
                {
                    Physics.autoSimulation = true;
                }
                var voiceBridge = managers.runner.GetComponent<FusionVoiceBridge>();
                if (voiceBridge != null)
                    Destroy(voiceBridge);
                GameObject.Destroy(managers.runner);
            }
        }

        // UpdateErrorMessage update the error message on the UI
        private void UpdateErrorMessage(string shutdownErrorMessage)
        {
            Debug.LogError($"UpdateErrorMessage : { shutdownErrorMessage} ");
            if (desktopErrorMessageTMP) desktopErrorMessageTMP.text = shutdownErrorMessage;
            if (hardwareRigErrorMessageTMP) hardwareRigErrorMessageTMP.text = shutdownErrorMessage;
        }

        // DisplayErrorMessage is in charge to hide all scene objects and display the error message
        private IEnumerator CleanUpScene()
        {
            GameObject errorGO = null;
            Fader fader = null;


            if (desktopCameraFader && desktopCameraFader.gameObject.activeInHierarchy)
            {
                errorGO = desktopErrorMessageGO;
                fader = desktopCameraFader;
            }
            if (hardwareRigFader && hardwareRigFader.gameObject.activeInHierarchy)
            {
                errorGO = hardwareRigErrorMessageGO;
                fader = hardwareRigFader;
            }

            yield return Fadeout(fader);
            ConfigureSceneForOfflineMode();
            yield return DisplayErrorMessage(fader, errorGO);

        }

        private IEnumerator Fadeout(Fader fader)
        {
            // display black screen
            yield return fader.FadeIn();
            yield return new WaitForSeconds(1);
        }

        void ConfigureSceneForOfflineMode()
        {
            // Hide all scene
            HideScene();
            // Destroy spawned objects
            DestroyNetworkedObjects();
        }


        private IEnumerator DisplayErrorMessage(Fader fader, GameObject errorMessage)
        {
            // Display error message UI
            if (errorMessage) errorMessage.SetActive(true);
            // remove black screen
            yield return fader.FadeOut();
        }

        // HideScene is in charge to hide the scene 
        private void HideScene()
        {
            if (staticLevel) staticLevel.SetActive(false);
            if (interactableObjects) interactableObjects.SetActive(false);
            RenderSettings.skybox = null;
        }


        // QuitApplication is called when the user push the UI Exit button 
        public void QuitApplication()
        {
            Debug.LogError("User exit the application");
            Application.Quit();
        }
    }
}