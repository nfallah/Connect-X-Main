/* Once the game is over, the InputHandler script is enabled to give the player the options
 * -to either go back to the menu or reset the game.
 */

using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public bool canReset; // Determines whether the scene can be reset

    [SerializeField] PlacementHandler placementHandler; // Reference to the PlacementHandler object placed on an empty GameObject in the scene

    bool isEscapeSelected, isReturnSelected; // Prevents possible spamming of said keys breaking the game

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isEscapeSelected && !isReturnSelected)
        {
            isEscapeSelected = true;
            SceneManager.LoadScene(0); // Loads the menu
        }

        else if (Input.GetKeyDown(KeyCode.Return) && canReset &&!isEscapeSelected && !isReturnSelected)
        {
            isReturnSelected = true;
            GameObject rememberer = new GameObject("Rememberer", typeof(Rememberer)); // Creates a new GameObject and attaches the Rememberer script to it
            DontDestroyOnLoad(rememberer); // Ensures the GameObject wil not be destroyed after the new scene is loaded
            
            // Determines the order of play for the next reset, which the rememberer GameObject will remember after the scene is loaded
            if (placementHandler.firstPlayer == Location.Player.ONE)
            {
                rememberer.GetComponent<Rememberer>().newPlayer = Location.Player.TWO;
            }

            else
            {
                rememberer.GetComponent<Rememberer>().newPlayer = Location.Player.ONE;
            }

            SceneManager.LoadScene(1); // Loads the game
        }
    }
}