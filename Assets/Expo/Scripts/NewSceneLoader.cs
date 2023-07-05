using Fusion;
using Fusion.Samples.Expo;
using Fusion.Samples.IndustriesComponents;
using Fusion.XR;
using Fusion.XR.Shared.Locomotion;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


/**
 * 
 * NewSceneLoader is in charge to load a new scene when the player collides with the box collider
 * 
 **/
public class NewSceneLoader : MonoBehaviour
{

    public string newSceneName;
    public ExpoManagers managers;

    [Header("Automatically set")]
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private ApplicationManager applicationManager;
    [SerializeField] private Fader desktopCameraFader;
    [SerializeField] private Fader hardwareRigFader;
    [SerializeField] private RigLocomotion localRigLocomotion;
    [SerializeField] private RigInfo rigInfo;

    private bool newSceneIsLoading = false;
    bool waitForFadeIn = false;
    private void Start()
    {
        if (managers == null) managers = (ExpoManagers)ExpoManagers.FindInstance();

        if (runner == null)
            runner = managers.runner;
        if (runner == null)
            Debug.LogError("Runner not found !");
        else
        {
            rigInfo = RigInfo.FindRigInfo(runner);
            if (rigInfo != null)
            {
                localRigLocomotion = rigInfo.localHardwareRig.GetComponent<RigLocomotion>();
            }
        }

        if (applicationManager == null)
            applicationManager = managers.applicationManager;
        if (applicationManager == null)
            Debug.LogError("Application Manager not found !");
        else
        {
            desktopCameraFader = applicationManager.desktopCameraFader;
            hardwareRigFader = applicationManager.hardwareRigFader;
        }
    }

    [ContextMenu("SwitchScene")]
    private async void SwitchScene()
    {

        // Application manager must be destroyed first to avoid it displaying message and fader issue when the runner is shutdown
        Destroy(applicationManager.gameObject);
        // RigLocomotion must also be destroyed to avoid Fadeout
        Destroy(localRigLocomotion);

        waitForFadeIn = true;
        StartCoroutine(DisplayFaderScreen());
        while(waitForFadeIn) await Task.Delay(10);

        Debug.Log("Stopping current Session");
        await runner.Shutdown(true);
        Debug.Log("Stopped Session");
        Debug.Log($"Runner connected? {runner.IsConnectedToServer}");
        Debug.Log("Loading new scene " + newSceneName);
        SceneManager.LoadScene(newSceneName, LoadSceneMode.Single);

    }


    private IEnumerator DisplayFaderScreen()
    {
        waitForFadeIn = true;
        if (desktopCameraFader && desktopCameraFader.gameObject.activeInHierarchy)
        {
            yield return Fading(desktopCameraFader);
        }
        if (hardwareRigFader && hardwareRigFader.gameObject.activeInHierarchy)
        {
            yield return Fading(hardwareRigFader);
        }
        waitForFadeIn = false;
    }

    private IEnumerator Fading(Fader fader)
    {
        yield return fader.FadeIn(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!newSceneIsLoading && other.GetComponentInParent<HardwareHand>())
        {
            newSceneIsLoading = true;
            Debug.Log($"Switching to scene {newSceneName}");
            SwitchScene();
        }
    }
}
