using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class castle : MonoBehaviour
{
    // Start is called before the first frame update
     void OnTriggerEnter2D(Collider2D other)
    {
     if(other.tag=="Player")
        {
            SceneManager.LoadScene("level2");
        }
    }
}
