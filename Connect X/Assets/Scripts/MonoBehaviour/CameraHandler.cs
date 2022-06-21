/* Handles the camera basics, including the movement/zoom speed and their acceptable range of values,
 * -as well as the position of the camera.
 */

using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] float maxSpeed, minSpeed; // The established range of values of the camera, independent of the grid size

    [SerializeField] GameHandler gameHandler; // Reference to the GameHandler object placed on an empty GameObject in the scene

    [SerializeField] Transform playerCamera; // Reference to the Camera object placed in the scene

    private float moveSpeed, zoomSpeed;

    // The movement constraints (right of board, above board, out of board, and into the board respectively)
    private int maxRight, maxUp, maxZoomOut, maxZoomIn = -8;

    /* Every frame, the x, y, and z values for movement are obtained, and the x and y movement specifically speeds up to a limit as
     * -the player zooms further in, and likewise slows down when zooming out. The movement of the player is also checked at the very end
     * -to ensure they cannot go out of bounds.
     */
    private void Update()
    {
     
        float x = Input.GetAxisRaw("Horizontal") * Mathf.Clamp(moveSpeed * (maxZoomOut / playerCamera.position.z), minSpeed, maxSpeed) * Time.deltaTime;
        float y = Input.GetAxisRaw("Vertical") * Mathf.Clamp(moveSpeed * (maxZoomOut / playerCamera.position.z), minSpeed, maxSpeed) * Time.deltaTime;
        float z = Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
        Vector3 pos = playerCamera.position;

        playerCamera.position = new Vector3(Mathf.Clamp(pos.x + x, 0, maxRight), Mathf.Clamp(pos.y + y, 0, maxUp), Mathf.Clamp(pos.z + z, maxZoomOut, maxZoomIn));
    }

    // Initializes the basic camera values as the game begins; these values scale up to a grid of any size
    public void Initialize()
    {
        // Note: the expression '0.33f/0.33f' was used as a way to transform the value into a float as casting did not work for some reason
        moveSpeed = (0.33f/0.33f) * (gameHandler.board.sizeX - 1) / 3;
        zoomSpeed = 4 * gameHandler.board.consecutiveNum;
        maxRight = gameHandler.board.sizeX - 1;
        maxUp = gameHandler.board.sizeY - 1;
        maxZoomOut = -2 * gameHandler.board.consecutiveNum;
        playerCamera.transform.position = new Vector3((gameHandler.board.sizeX - 1) * 0.5f, (gameHandler.board.sizeY - 1) * 0.5f, maxZoomOut);
    }
}