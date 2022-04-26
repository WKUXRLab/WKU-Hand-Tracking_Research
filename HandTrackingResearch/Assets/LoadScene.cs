using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1")) {
            SceneManager.LoadScene("FirstDemo", LoadSceneMode.Single);
        }
        else if (Input.GetKey("2"))
        {
            SceneManager.LoadScene("SecondDemo", LoadSceneMode.Single);
        }
        else if (Input.GetKey("3"))
        {
            SceneManager.LoadScene("ThirdDemo", LoadSceneMode.Single);
        }
        else if (Input.GetKey("4"))
        {
            SceneManager.LoadScene("FourthDemo", LoadSceneMode.Single);
        }
        else if (Input.GetKey("5"))
        {
            SceneManager.LoadScene("FifhrDemo", LoadSceneMode.Single);
        }
    }
}
