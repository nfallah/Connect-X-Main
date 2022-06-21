/* If the player to go first is initially red, then in the next game, yellow must go first.
 * Thus, when the player chooses to reset the game rather than return to the menu, an object containing the
 * -Rememberer script is spawned, and stores the value opposite of the new player.
 * Then, once the scene loads, this value is stored as the new instance of newPlayer, and the object is destroyed.
 */

using UnityEngine;

public class Rememberer : MonoBehaviour
{
    public Location.Player newPlayer; // The player that should go first
}