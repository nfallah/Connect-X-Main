using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Board board;

    public bool isActive = true;

    public GameObject gamePieceRef;

    [HideInInspector] public GameObject gamePiece, gamePieces, predictionGamePiece;

    [SerializeField] Camera playerCamera;

    [SerializeField] CameraHandler cameraHandler;

    [SerializeField] GameObject boardPieceRef;

    private Plane gamePlane;

    private Vector3 predictionPosition;

    private void Awake()
    {
        GameObject boardPieces = new GameObject("Board Pieces");

        gamePieces = new GameObject("Game Pieces");
        board = new Board(Random.Range(4, 21));
        gamePlane = new Plane(Vector3.back, 0);
        predictionGamePiece = Instantiate(gamePieceRef);
        predictionGamePiece.name = "Prediction Game Piece";
        gamePiece = Instantiate(gamePieceRef);
        gamePiece.name = "Game Piece";
        cameraHandler.Initialize();

        for (int y = 0; y < board.sizeY; y++)
        {
            for (int x = 0; x < board.sizeX; x++)
            {
                GameObject boardPiece = Instantiate(boardPieceRef, boardPieces.transform);
                boardPiece.name = "Board Piece(" + x + ", " + y + ", -0.1)";
                boardPiece.transform.position = new Vector3(x, y, -0.1f);
            }
        }
    }

    private void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        if (gamePlane.Raycast(ray, out float enter))
        {
            int xSign, ySign;
            Vector3 hitPoint = ray.GetPoint(enter);

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

            hitPoint = new Vector3((int)(hitPoint.x + xSign * 0.5f), (int)(hitPoint.y + ySign * 0.5f), 0);

            if (hitPoint.x >= 0 && hitPoint.x < board.sizeX && hitPoint.y >= 0 && hitPoint.y < board.sizeY && canPlace((int)hitPoint.x))
            {
                if (!isActive)
                {
                    isActive = true;
                    gamePiece.SetActive(true);
                    predictionGamePiece.SetActive(true);
                }

                gamePiece.transform.position = new Vector3(hitPoint.x, board.sizeY, 0);
                predictionGamePiece.transform.position = predictionPosition;
            }

            else if (isActive)
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

    private bool canPlace(int xPos)
    {
        bool canPlace = false;

        predictionPosition = -Vector3.one;

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