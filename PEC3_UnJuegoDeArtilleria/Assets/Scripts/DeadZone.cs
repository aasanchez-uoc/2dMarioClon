using UnityEngine;
/// <summary>
/// Script que termina el juego cuando colisionamos con el
/// </summary>
public class DeadZone : MonoBehaviour
{
    /// <summary>
    /// Referencia al gameplayManager para comunicarle que ha terminado el juego
    /// </summary>
    public GameplayManager GameplayManager;

    /// <summary>
    /// Booleano que indica si hemos ganado o perdido
    /// </summary>
    public bool isWin = false;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.layer != 8) other.gameObject.GetComponent<CharacterController>().Lifes--;
            if(isWin || (!isWin && GameplayManager.Player.Lifes <= 0) )GameplayManager.GameOver(isWin); //si se trata de una zona de muerte y no de victoria, comprobamos también que no le queden vidas al jugador
            else //es una zona de muerte pero le quedan vidas al jugador
            {
                GameplayManager.RespawnPlayer();
            }
        }
    }

}
