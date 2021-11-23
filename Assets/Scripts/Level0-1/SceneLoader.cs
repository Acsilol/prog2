using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public bool isLevel1Complete = false;
    public Animator animator;
    public Text text;


    public void LoadScene(string sceneName) 
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    private void FixedUpdate()
    {
        if (isLevel1Complete)
        {
            StartCoroutine(LoadLevel2());
            StartCoroutine(ChangeColor());
        }        
    }

    IEnumerator LoadLevel2()
    {
        animator.SetBool("isOpen", true);
        yield return new WaitForSeconds(10f);
        animator.SetBool("isOpen", false);
        yield return new WaitForSeconds(4f);
        LoadScene("Level2");
    }

    IEnumerator ChangeColor()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            text.color = Color.red;            
            yield return new WaitForSeconds(2f);
            text.color = Color.green;
            yield return new WaitForSeconds(2f);
            text.color = Color.blue;
        }
    }
}
