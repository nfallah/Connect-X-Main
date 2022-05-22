using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] float maxSpeed, minSpeed;

    [SerializeField] GameHandler gameHandler;

    [SerializeField] Transform playerCamera;

    private float moveSpeed, zoomSpeed;

    private int maxRight, maxUp, maxZoomOut, maxZoomIn = -8;

    private void Update()
    {
     
        float x = Input.GetAxisRaw("Horizontal") * Mathf.Clamp(moveSpeed * (maxZoomOut / playerCamera.position.z), minSpeed, maxSpeed) * Time.deltaTime;
        float y = Input.GetAxisRaw("Vertical") * Mathf.Clamp(moveSpeed * (maxZoomOut / playerCamera.position.z), minSpeed, maxSpeed) * Time.deltaTime;
        float z = Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        Vector3 pos = playerCamera.position;

        playerCamera.position = new Vector3(Mathf.Clamp(pos.x + x, 0, maxRight), Mathf.Clamp(pos.y + y, 0, maxUp), Mathf.Clamp(pos.z + z, maxZoomOut, maxZoomIn));
    }

    public void Initialize()
    {
        moveSpeed = (0.33f/0.33f) * (gameHandler.board.sizeX - 1) / 3;
        zoomSpeed = 4 * gameHandler.board.consecutiveNum;
        maxRight = gameHandler.board.sizeX - 1;
        maxUp = gameHandler.board.sizeY - 1;
        maxZoomOut = -2 * gameHandler.board.consecutiveNum;
        playerCamera.transform.position = new Vector3((gameHandler.board.sizeX - 1) * 0.5f, (gameHandler.board.sizeY - 1) * 0.5f, maxZoomOut);
    }
}