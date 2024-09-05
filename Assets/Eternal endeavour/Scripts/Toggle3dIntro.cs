using System;
using _3D_scene;
using UnityEngine;

[ExecuteInEditMode]
public class Toggle3dIntro : MonoBehaviour
{
    public bool isIntroActive = true;
    public bool isLightsAndDarknessActive = true;

    public GameObject door;

    public GameObject devouringDarknessObject;
    public GameObject mainSceneLogic;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnValidate()
    {
        door.SetActive(isIntroActive);
        mainSceneLogic.SetActive(!isIntroActive);
        devouringDarknessObject.SetActive(!isIntroActive);

        devouringDarknessObject.GetComponent<DevouringDarknessController>().enabled = isLightsAndDarknessActive;
        mainSceneLogic.transform.GetChild(0).GetComponent<LightsController>().enabled = isLightsAndDarknessActive;
    }
}