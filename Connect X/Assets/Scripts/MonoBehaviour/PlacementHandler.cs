/* Handles the moving animation of the game piece, respective UI elements, as well as
 * -win conditions and detecting them after any move has been made.
 */

using UnityEngine;
using UnityEngine.UI;

public class PlacementHandler : MonoBehaviour
{
    private enum WinCondition { DRAW, P1WIN, P2WIN } // Possible win conditions

    [HideInInspector] public Location.Player firstPlayer; // The player that starts first

    [SerializeField] float dropTime; // The time it takes for the game piece to drop 1 vertical unit

    [SerializeField] GameHandler gameHandler; // Reference to the GameHandler object placed on an empty GameObject in the scene

    [SerializeField] GameObject information, win; // GameObjects with UI elements which display after the game has ended

    [SerializeField] InputHandler inputHandler; // Reference to the InputHandler object placed on an empty GameObject in the scene

    [SerializeField] Material player1, player1Prediction, player2, player2Prediction; // Reference to game piece materials indicating the current player

    [SerializeField] Text conditionText, turnText, winText, returnText, escapeText; // Additional UI elements that enable when the game ends

    private bool isBusy; // True when a game piece is dropping; other processes must wait until false (such as placing an additional piece)

    private float timer; // Used in conjunction with the game piece dropping; refreshes every time 1 unit is traveled

    private int currentPlays = 0, maxPlays; // Used to keep track of dropped pieces; used to check for the draw win condition

    private Location.Player player; // The current player

    private Vector3 currentPos, endPos, nextPos; // Used for dropping game pieces

    private void Start()
    {
        maxPlays = gameHandler.board.sizeX * gameHandler.board.sizeY; // Calculates the total number of plays possible

        /* If the player has not reset the game (arrived at the menu) then the default player is determined. Otherwise, find the Rememberer
         * GameObject to determine the correct player. This will alternate infinitely until the player returns to the menu.
         */
        if (FindObjectOfType<Rememberer>() == null)
        {
            player = firstPlayer = Location.Player.ONE;
        }

        else
        {
            player = firstPlayer = FindObjectOfType<Rememberer>().newPlayer;
            Destroy(FindObjectOfType<Rememberer>().gameObject);
        }

        // UI elements are updated; some are disabled
        win.SetActive(false);
        information.SetActive(false);
        conditionText.text = "Condition: <color=#00ff00>" + gameHandler.board.consecutiveNum + "</color> in a row";
        escapeText.text = "Tab: <color=#00ff00>menu</color>";
        SetColors();
        UpdateTurnText();
    }

    private void Update()
    {
        // If a valid move can be made and the game is not busy dropping a game piece, the player can press LMB to drop a game piece
        if (Input.GetMouseButtonDown(0) && gameHandler.isActive && !isBusy)
        {
            gameHandler.enabled = false; // Disables the GameHandler script to prevent any interference with the dropping process
            gameHandler.isActive = false; // Also disables isActive for reasons above
            isBusy = true;
            currentPos = gameHandler.gamePiece.transform.position;
            endPos = gameHandler.predictionGamePiece.transform.position;
            gameHandler.predictionGamePiece.SetActive(false);
            nextPos = currentPos - Vector3.up;
            gameHandler.board.grid[(int)endPos.x, (int)endPos.y].taken = true; // The coordinate where the game piece drops is now taken
            gameHandler.board.grid[(int)endPos.x, (int)endPos.y].player = player; // The coordinate where the game piece drops now has a player occupation
            currentPlays++;
            DropPiece(); // Begins the dropping process
        }
    }

    // Updates the text to reflect which player is going
    private void UpdateTurnText()
    {
        string text = "Turn: ";

        if (player == Location.Player.ONE)
        {
            text += "<color=#ffff00>player 1</color>";
        }

        else
        {
            text += "<color=#ff0000>player 2</color>";
        }

        turnText.text = text;
    }

    // Returns whether an integer location in the world is valid using the board size; used for verifying a win condition
    private bool Valid(int x, int y)
    {
        if (x >= 0 && x < gameHandler.board.sizeX && y >= 0 && y < gameHandler.board.sizeY)
        {
            return true;
        }

        return false;
    }

    /* The brains of the game; determines whether any cardinal direction (horizontal, vertical, diagonal right-up, diagonal left-up)
     * has enough pieces in a row (excluding where the game piece is placed) to obtain a victory.
     */
    private bool CountCheck()
    {
        int requiredCount = gameHandler.board.consecutiveNum - 1;

        if (RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(1, 0)) + RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(-1, 0)) >= requiredCount)
        {
            return true;
        }

        if (RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(0, 1)) + RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(0, -1)) >= requiredCount)
        {
            return true;
        }

        if (RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(1, 1)) + RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(-1, -1)) >= requiredCount)
        {
            return true;
        }

        if (RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(1, -1)) + RowCheck((int)endPos.x, (int)endPos.y, new Vector2Int(-1, 1)) >= requiredCount)
        {
            return true;
        }

        return false;
    }

    /* The method that CountCheck utilizes to check for a win condition. Given a two-dimensional direction and an initial position,
     * RowCheck utilizes recursion to find all of the pieces in a row; if the given position is not valid, not taken, or is not the same player
     * as the one whose turn it is, then RowCheck returns 0. Else, 1 + RowCheck in the same direction is continued until 0 is returned.
     */
    private int RowCheck(int x, int y, Vector2Int direction)
    {
        if (!Valid(x + direction.x, y + direction.y) || !(gameHandler.board.grid[x + direction.x, y + direction.y].taken && gameHandler.board.grid[x + direction.x, y + direction.y].player == player))
        {
            return 0;
        }

        else
        {
            return 1 + RowCheck(x + direction.x, y + direction.y, direction);
        }
    }

    /* Every time a piece is dropped, a win condition is checked for. If there are not enough pieces in a row, then the draw condition is checked.
     * The order must be this, or else a draw condition may be falsely chosen even if the last move on the board would obtain a player victory.
     */
    private bool WinConditionCheck()
    {
        if (CountCheck()) // Checks for the count win condition
        {
            if (player == Location.Player.ONE)
            {
                SetWinCondition(WinCondition.P1WIN);
            }

            else
            {
                SetWinCondition(WinCondition.P2WIN);
            }

            return true;
        }

        if (currentPlays == maxPlays) // Checks for the draw win condition
        {
            SetWinCondition(WinCondition.DRAW);
            return true;
        }

        return false;
    }

    // Enables UI elements to display type of victory given in the parameter
    private void SetWinCondition(WinCondition condition)
    {
        enabled = false;
        gameHandler.enabled = false;
        string text = "Winner: ";

        if (player == Location.Player.ONE && condition != WinCondition.DRAW)
        {
            text += "<color=#ffff00>player 1</color>";
        }

        else if (condition != WinCondition.DRAW)
        {
            text += "<color=#ff0000>player 2</color>";
        }

        else
        {
            text += "<color=#00ff00>n/a</color> (draw)";
        }

        information.SetActive(true);
        win.SetActive(true);
        winText.text = text;
        returnText.text = "Return: <color=#00ff00>restart</color>";
        turnText.text = "Turn: <color=#00ff00>n/a</color>";
        inputHandler.canReset = true;
    }

    // Drops the current game piece to the desired position over time
    private void DropPiece()
    {

        timer = Mathf.Clamp(timer - Time.deltaTime, 0, dropTime); // Decrements and clamps timer
        gameHandler.gamePiece.transform.position = Vector3.Lerp(currentPos, nextPos, 1 - timer / dropTime); // Uses linear interpolation to move the piece

        if (timer == 0)
        {
            if (nextPos == endPos) // If the game piece is done dropping
            {
                if (!WinConditionCheck()) // If no win condition is achieved, then the game will continue playing
                {
                    isBusy = false;
                    gameHandler.gamePiece.transform.SetParent(gameHandler.gamePieces.transform);
                    gameHandler.gamePiece.name = "Game Piece(" + endPos.x + ", " + endPos.y + ", 0)";
                    gameHandler.gamePiece = Instantiate(gameHandler.gamePieceRef);
                    gameHandler.gamePiece.name = "Game Piece";
                    gameHandler.gamePiece.SetActive(false);
                    SwitchPlayer();
                    SetColors();
                    UpdateTurnText();
                    Invoke("EnableGameHandler", Time.deltaTime);
                }
            }

            else // Refresh the timer, set new positions, and restart the dropping cycle
            {
                currentPos = nextPos;
                nextPos -= Vector3.up;
                timer = dropTime;
                Invoke("DropPiece", Time.deltaTime);
            }
        }

        else
        {
            Invoke("DropPiece", Time.deltaTime);
        }

    }

    private void EnableGameHandler() // Invoked method, effectively continues the game as pieces can be dropped again
    {
        gameHandler.enabled = true;
    }

    // Switches the player based on the current player
    private void SwitchPlayer()
    {
        switch (player)
        {
            case Location.Player.ONE:
                player = Location.Player.TWO;
                break;

            case Location.Player.TWO:
                player = Location.Player.ONE;
                break;
        }
    }

    // Sets the materials of the game piece and prediction game piece based on the player
    private void SetColors()
    {
        switch (player)
        {
            case Location.Player.ONE:
                gameHandler.gamePiece.GetComponent<MeshRenderer>().material = player1;
                gameHandler.predictionGamePiece.GetComponent<MeshRenderer>().material = player1Prediction;
                break;

            case Location.Player.TWO:
                gameHandler.gamePiece.GetComponent<MeshRenderer>().material = player2;
                gameHandler.predictionGamePiece.GetComponent<MeshRenderer>().material = player2Prediction;
                break;
        }
    }
}