using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script encargado de mostrar el panel de game over/victoria y de reiniciar el juego
/// </summary>
public class GameplayManager : MonoBehaviour
{
    /// <summary>
    /// El GameObject que contendrá la pantalla que mostraremos cuando el jugador muera
    /// </summary>
    public GameObject GameOverScreen;

    /// <summary>
    /// El texto de game over/victoria
    /// </summary>
    public Text GameOverText;

    public Text PointsText;

    public Text CoinsText;

    public Text TimeText;

    public Text LifesText;

    public GameObject[] CheckPointsList { get; private set; }

    private float time;

    public CharacterController Player;
    void Start()
    {
        //esocndemos el panel de game over
        GameOverScreen.SetActive(false);

        //ponemos el contador de tiempo a cero
        time = 0;

        //buscamos los CheckPoints
        CheckPointsList = GameObject.FindGameObjectsWithTag("CheckPoint");
    }

    void Update()
    {
        //Actualizamos los contadores
        time += Time.deltaTime;
        TimeText.text =   ((int)time).ToString();
        PointsText.text = Player.Puntuacion.ToString();
        CoinsText.text = Player.Monedas.ToString();
        LifesText.text = Player.Lifes.ToString();
    }

    /// <summary>
    /// Función encargada de reiniciar el nivel
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void RespawnPlayer()
    {

        foreach( GameObject cp in CheckPointsList)
        {
            if(cp.GetComponent<CheckPoint>().Activated == true)
            {
                Player.gameObject.transform.position = cp.transform.position;
                Player.ResetPlayer();
                break;
            }
        }
    }

    public void GameOver(bool win = false)
    {
        GameOverText.text = (win) ? "YOU WIN" : "GAME OVER!";
        GameOverScreen.SetActive(true);
    }

    /// <summary>
    /// Función encargada de salir del juego
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }

}
