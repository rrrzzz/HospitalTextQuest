using UnityEngine;

namespace _3D_scene
{
    public class MainGameStarter : MonoBehaviour
    {
        public GameObject FpsController;
        public GameObject MainSceneLogic;
        public GameObject ViewFader;


        //Event calls this method
        private void ChangeCurrentCam()
        {
            ViewFader.GetComponent<BlackFader>().SetAlphaOne();
            FpsController.SetActive(true);
            MainSceneLogic.SetActive(true);
            gameObject.SetActive(false);
            
        }
    }
}