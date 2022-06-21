/* General script which holds the main Board object, also controlling
 * -the movement of the prediction piece, which shows where a piece would drop.
 */

using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Board board; // The board object used throughout the game

    public bool isActive = true; // Used to control whether the prediction piece GameObject should be active or not

    public GameObject gamePieceRef; // Used as a reference GameObject to instantiate new game pieces

    [HideInInspector] public GameObject gamePiece, gamePieces, predictionGamePiece; // Reference to the current game piece, a parent which holds all game pieces, and the prediction game piece respectively

    [SerializeField] Camera playerCamera;

    [SerializeField] CameraHandler cameraHandler; // Reference to the CameraHandler object placed on an empty GameObject in the scene

    [SerializeField] GameObject boardPieceRef; // Used as a reference GameObject to instantiate the board at playtime

    private Plane gamePlane; // The imaginary plane through which mouse to world position raycasts are made

    private Vector3 predictionPosition; // The position of the prediction game piece

    private void Awake()
    {
        GameObject boardPieces = new GameObject("Board Pieces"); // Spawns an empty GameObject to which all board pieces are attached to

        gamePieces = new GameObject("Game Pieces"); // Spawns an empty GameObject to which all game pieces are attached to
        board = new Board(Random.Range(4, 21)); // Defines the board with a random integer from 4 to 20 (the game pieces in a row required to win)
        gamePlane = new Plane(Vector3.back, 0); // Defines the imaginary plane
        predictionGamePiece = Instantiate(gamePieceRef);
        predictionGamePiece.name = "Prediction Game Piece";
        gamePiece = Instantiate(gamePieceRef);
        gamePiece.name = "Game Piece";
        cameraHandler.Initialize(); // References the CameraHandler object to set movement-related values based on the board size

        // Creates the Connect X board one-by-one using a for loop as iteration
        for (int y = 0; y < board.sizeY; y++)
        {
            for (int x = 0; x < board.sizeX; x++)
            {
                GameObject boardPiece = Instantiate(boardPieceRef, boardPieces.transform); // Spawns a board piece and sets its parent
                boardPiece.name = "Board Piece(" + x + ", " + y + ", -0.1)"; // Sets the name of the board piece based on coordinates
                boardPiece.transform.position = new Vector3(x, y, -0.1f); // Sets the position of the board piece based on coordinates
            }
        }
    }

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Creates a ray from the player camera to the player mouse

        if (gamePlane.Raycast(ray, out float enter)) // Checks if the newly-created ray collides with the imaginary plane
        {
            int xSign, ySign;
            Vector3 hitPoint = ray.GetPoint(enter); // The position of the collision

            /* Due to the nature of the code and position of the game pieces, an "offset" is needed depending on whether the hit position is positive
             * -or negative. Thus, the following if statements calculate the sign for which the offset (0.5f) is multiplied by.
             */
            if (hitPoint.x < 0)
            {
                xSign = -1;
            }

            else
            {
                xSign = 1;
            }

            if (hitPoint.y < 0)
            {
                ySign = -1;
            }

            else
            {
                ySign = 1;
            }

            hitPoint = new Vector3((int)(hitPoint.x + xSign * 0.5f), (int)(hitPoint.y + ySign * 0.5f), 0); // Locks the hit position to a grid using the offset

            // Checks to see if the hit point is valid (it is horizontally and vertically touching the imaginary board)
            if (hitPoint.x >= 0 && hitPoint.x < board.sizeX && hitPoint.y >= 0 && hitPoint.y < board.sizeY && canPlace((int)hitPoint.x))
            {
                if (!isActive) // If the current state is NOT active, then take steps to ensure that it is
                {
                    isActive = true;
                    gamePiece.SetActive(true); // Enables the game piece GameObject
                    predictionGamePiece.SetActive(true); // Enables the prediction game piece GameObject
                }

                gamePiece.transform.position = new Vector3(hitPoint.x, board.sizeY, 0); // Sets the position of the game piece
                predictionGamePiece.transform.position = predictionPosition; // Sets the position of the prediction game piece
            }

            else if (isActive) // If the current state IS active, then take steps to ensure that it is not
            {
                isActive = false;
                gamePiece.SetActive(false);
                predictionGamePiece.SetActive(false);
            }
        }

        else if (isActive)
        {
            isActive = false;
            gamePiece.SetActive(false);
            predictionGamePiece.SetActive(false);
        }
    }

    // In short, if a certain column is filled, then no game piece can be dropped and the position is "invalid"
    private bool canPlace(int xPos)
    {
        bool canPlace = false;

        predictionPosition = -Vector3.one; // Set to a position that is never obtainable (used for debugging)

        // From the bottom to the top of the column, it is checked whether said position is taken. If not, then there exists at least one empty coordinate. 
        for (int y = 0; y < board.sizeY; y++)
        {
            if (!board.grid[xPos, y].taken)
            {
                canPlace = true;
                predictionPosition = new Vector3(xPos, y, 0);
                break;
            }
        }

        return canPlace;
    }
}