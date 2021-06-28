using UnityEngine;

/// <summary>
/// Clase que representa los checkpoints del juego
/// </summary>
public class CheckPoint : MonoBehaviour
{
    #region Propiedades
    /// <summary>
    /// Propiedad de solo lectura que indica si el cheackpoint está activado
    /// </summary>
    public bool Activated { get; private set; }

    /// <summary>
    /// Propiedad de solo lectura que indica si el checkpoint ya ha sido activado
    /// </summary>
    public bool Used { get; private set; }
    #endregion

    /// <summary>
    /// Referencia al GamplayManager
    /// </summary>
    private GameplayManager manager;

    void Start()
    {
        Activated = false;
        Used = false;
        manager = GameObject.FindObjectOfType(typeof(GameplayManager)) as GameplayManager;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Comprobamos que la colision es con el jugador y que este checkpoint no haya sido activado ya
        if (other.tag == "Player" && !Used)
        {
            ActivateCheckPoint();
        }
    }

    /// <summary>
    /// Método encargado de activar nuestro checkpoint y desactivar el resto
    /// </summary>
    private void ActivateCheckPoint()
    {
        // Desactivamos todos los checkpoints de la escena
        foreach (GameObject cp in manager.CheckPointsList)
        {
            cp.GetComponent<CheckPoint>().Activated = false;
        }

        // Activamos este checkPoint
        Activated = true;
        Used = true;
    }
}
