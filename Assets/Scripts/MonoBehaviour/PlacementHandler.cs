using UnityEngine;
using UnityEngine.UI;

public class PlacementHandler : MonoBehaviour
{
    private enum WinCondition { DRAW, P1WIN, P2WIN }

    [HideInInspector] public Location.Player firstPlayer;

    [SerializeField] float dropTime;

    [SerializeField] GameHandler gameHandler;

    [SerializeField] GameObject information, win;

    [SerializeField] InputHandler inputHandler;

    [SerializeField] Material player1, player1Prediction, player2, player2Prediction;

    [SerializeField] Text conditionText, turnText, winText, returnText, escapeText;

    private bool isBusy;

    private float timer;

    private int currentPlays = 0, maxPlays;

    private Location.Player player;

    private Vector3 currentPos, endPos, nextPos;

    private void Start()
    {
        maxPlays = gameHandler.board.sizeX * gameHandler.board.sizeY;

        if (FindObjectOfType<Rememberer>() == null)
        {
            player = firstPlayer = Location.Player.ONE;
        }

        else
        {
            player = firstPlayer = FindObjectOfType<Rememberer>().newPlayer;
            Destroy(FindObjectOfType<Rememberer>().gameObject);
        }

        win.SetActive(false);
        information.SetActive(false);
        conditionText.text = "Condition: <color=#00ff00>" + gameHandler.board.consecutiveNum + "</color> in a row";
        escapeText.text = "Tab: <color=#00ff00>menu</color>";
        SetColors();
        UpdateTurnText();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && gameHandler.isActive && !isBusy)
        {
            gameHandler.enabled = false;
            gameHandler.isActive = false;
            isBusy = true;
            currentPos = gameHandler.gamePiece.transform.position;
            endPos = gameHandler.predictionGamePiece.transform.position;
            gameHandler.predictionGamePiece.SetActive(false);
            nextPos = currentPos - Vector3.up;
            gameHandler.board.grid[(int)endPos.x, (int)endPos.y].taken = true;
            gameHandler.board.grid[(int)endPos.x, (int)endPos.y].player = player;
            currentPlays++;
            DropPiece();
        }
    }

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

    private bool Valid(int x, int y)
    {
        if (x >= 0 && x < gameHandler.board.sizeX && y >= 0 && y < gameHandler.board.sizeY)
        {
            return true;
        }

        return false;
    }

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

    private bool WinConditionCheck()
    {
        if (CountCheck())
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

        if (currentPlays == maxPlays)
        {
            SetWinCondition(WinCondition.DRAW);
            return true;
        }

        return false;
    }

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

    private void DropPiece()
    {

        timer = Mathf.Clamp(timer - Time.deltaTime, 0, dropTime);
        gameHandler.gamePiece.transform.position = Vector3.Lerp(currentPos, nextPos, 1 - timer / dropTime);

        if (timer == 0)
        {
            if (nextPos == endPos)
            {
                if (!WinConditionCheck())
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

            else
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

    private void EnableGameHandler()
    {
        gameHandler.enabled = true;
    }

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