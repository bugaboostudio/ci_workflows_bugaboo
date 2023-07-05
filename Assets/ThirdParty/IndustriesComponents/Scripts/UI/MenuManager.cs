using Fusion.XR.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/**
 * 
 * MenuManager provides the functions to open/close the main menu and to quit the application.
 * It sets the keyboard binding key
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject mainMenu;
        public Managers managers;

        [SerializeField] private RigSelection rigSelection;
        [SerializeField] private List<GameObject> UIItemsforDesktopRigOnly = new List<GameObject>();

        public bool IsMainMenuOpened => mainMenu.activeInHierarchy;
        public InputActionProperty menuAction;


        private void Awake()
        {
            if (managers == null) managers = Managers.FindInstance();
            if (rigSelection == null) rigSelection = FindObjectOfType<RigSelection>();

            rigSelection.OnSelectRig.AddListener(OnSelectRig);
            if (rigSelection.rigSelected) OnSelectRig();
        }

        private void Start()
        {

            mainMenu.SetActive(false);
            if (menuAction.reference == null && menuAction.action.bindings.Count == 0)
            {
                menuAction.action.AddBinding("<Keyboard>/escape");
            }
            menuAction.action.Enable();
        }

        private void Update()
        {
            if (menuAction.action.WasPressedThisFrame())
            {
                if (IsMainMenuOpened)
                    CloseMainMenu();
                else
                    OpenMainMenu();
            }
        }

        public void OpenMainMenu()
        {

            Debug.Log("Open Menu");
            mainMenu.SetActive(true);
        }

        public void CloseMainMenu()
        {

            Debug.Log("Close Main Menu");
            mainMenu.SetActive(false);
        }

        public void QuitApplication()
        {
            Debug.Log("User Exit Application with Main Menu");
            Application.Quit();
        }



        private void OnSelectRig()
        {
            if (rigSelection.vrRig.isActiveAndEnabled)
                HideUIItems(false);
            else
                HideUIItems(true);
        }

        private void HideUIItems(bool shouldBeHidden)
        {
            foreach (GameObject item in UIItemsforDesktopRigOnly)
            {
                item.SetActive(shouldBeHidden);
            }
        }
    }
}

