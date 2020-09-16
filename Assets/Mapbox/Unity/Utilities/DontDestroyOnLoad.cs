namespace Mapbox.Unity.Utilities
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class DontDestroyOnLoad : MonoBehaviour
    {
        static DontDestroyOnLoad _instance;

        [SerializeField]
        bool _useSingleInstance;

        protected virtual void Awake()
        {
            if (_instance != null && _useSingleInstance)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "Story Game" || SceneManager.GetActiveScene().name == "Snake"
                    || SceneManager.GetActiveScene().name == "SpaceInvader")
            {
                Vector3 newPos = new Vector3(transform.position.x, -100, transform.position.z);
                this.transform.position = newPos;
            /*    if (GameObject.Find("Player") != null && GameObject.Find("Player").gameObject.activeSelf == true)
                {
                    GameObject.Find("Player").gameObject.SetActive(false);
                }   */
            }

            if (SceneManager.GetActiveScene().name == "DefaultScreen")
            {
                Vector3 newPos = new Vector3(0, 0, 0);
                this.transform.position = newPos;
                //GameObject.Find("Player").gameObject.SetActive(true);
            }
        }
    }

}