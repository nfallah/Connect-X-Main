public class Board
{
    public int consecutiveNum, sizeX, sizeY;

    public Location[,] grid;

    public Board(int _consecutiveNum)
    {
        consecutiveNum = _consecutiveNum;
        sizeX = 2 * consecutiveNum - 1;
        sizeY = 2 * consecutiveNum - 2;
        grid = new Location[sizeX, sizeY];

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                grid[x, y] = new Location();
            }
        }
    }
}