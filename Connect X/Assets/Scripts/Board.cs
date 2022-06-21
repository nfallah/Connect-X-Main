/* Declares the two-dimensional size of an imaginary board
 * -using the "numbers in a row" given through a constructor
 * The imaginary board is then later accessed throughout the game
 */ 

public class Board
{
    public int consecutiveNum, sizeX, sizeY;

    public Location[,] grid; // The two-dimensional grid referenced throughout the game

    public Board(int _consecutiveNum)
    {
        consecutiveNum = _consecutiveNum;
        sizeX = 2 * consecutiveNum - 1;
        sizeY = 2 * consecutiveNum - 2;
        grid = new Location[sizeX, sizeY]; // Sets the size of the board grid using above formulas

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                grid[x, y] = new Location(); // Initializes each Location object in the new grid
            }
        }
    }
}