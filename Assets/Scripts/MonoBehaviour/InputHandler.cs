using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [HideInInspector] public bool canReset;

    [SerializeField] PlacementHandler placementHandler;

    bool isReturnSelected; // Prevents possible spamming of return key breaking the game
    bool isEscapeSelected;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isEscapeSelected && !isReturnSelected)
        {
            isEscapeSelected = true;
            SceneManager.LoadScene(0);
        }

        else if (Input.GetKeyDown(KeyCode.Return) && canReset &&!isEscapeSelected && !isReturnSelected)
        {
            isReturnSelected = true;
            GameObject rememberer = new GameObject("Rememberer", typeof(Rememberer));
            DontDestroyOnLoad(rememberer);
            
            if (placementHandler.firstPlayer == Location.Player.ONE)
            {
                rememberer.GetComponent<Rememberer>().newPlayer = Location.Player.TWO;
            }

            else
            {
                rememberer.GetComponent<Rememberer>().newPlayer = Location.Player.ONE;
            }

            SceneManager.LoadScene(1);
        }
    }
}