using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LogicScript : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource fightMusic;
    public GameObject swordObject;
    public GameObject gameOverScreen;
    public GameObject swordText;
    public GameObject bossText;
    public GameObject winScreen;
    public AudioSource youWin;

    void Start()
    {
        backgroundMusic.Play();
    }

    void Update()
    {

    }

    public void restartGame()
    {
        ghostScript.IncreaseGlobalSpeed(0);
        babyGodzillaScript.IncreaseGlobalSpeed(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true);
    }

    public void gameWin()
    {
        fightMusic.Stop();
        youWin.Play();
        winScreen.SetActive(true);
    }

    public void fightAudio()
    {
        StartCoroutine(bossTextCoroutine());
        backgroundMusic.Stop();
        fightMusic.Play();
    }

    private IEnumerator bossTextCoroutine()
    {
        yield return new WaitForSeconds(2F);
        bossText.SetActive(true);
        yield return new WaitForSeconds(2F);
        bossText.SetActive(false);
    }

    public void swordVisible()
    {
        StartCoroutine(swordTextCoroutine());
        Renderer swordRenderer = swordObject.GetComponent<Renderer>();
        PolygonCollider2D swordCollider = swordObject.GetComponent<PolygonCollider2D>();

        swordRenderer.enabled = true;
        swordCollider.enabled = true;
    }

    private IEnumerator swordTextCoroutine()
    {
        swordText.SetActive(true);
        yield return new WaitForSeconds(2F);
        swordText.SetActive(false);
    }


}
