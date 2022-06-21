/* Provides the necessary information for any "coordinate" in the game,
 * -such as which entity owns said location and whether a player
 * -has taken the spot at all
 */

public class Location
{
    public enum Player { NONE, ONE, TWO }; // The default Player state is NONE, implying no side has placed in that coordinate yet

    public bool taken;

    public Player player;
}