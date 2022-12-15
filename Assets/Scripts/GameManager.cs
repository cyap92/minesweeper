using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject boardContainer;
    [SerializeField] private MessagePopUp messagePopUp;

    private int Rows = 5;
    private int Cols = 5;
    private int Mines = 5;

    public GameState GameState = GameState.Ready;

    //grid of game tiles
    private Tile[,] Tiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //generate new board with parameters set by ui
    public void GenerateBoard()
    {
        if (Tiles != null)
        {
            foreach (Tile t in Tiles)
            {
                Destroy(t.gameObject);
            }
        }
        Tiles = null;
        GameState = GameState.Ready;
        InitBoard(Rows, Cols);
    }

    //switch board size via UI dropdown
    public void ChangeBoardSize(int option)
    {
        switch (option)
        {
            case 0:
                Rows = 5;
                Cols = 5;
                break;
            case 1:
                Rows = 8;
                Cols = 8;
                break;
            case 2:
                Rows = 12;
                Cols = 12;
                break;
            default:
                break;
        }
    }

    //set mine cound via input field
    public void SetMineCount(string value)
    {
        int number;

        bool success = int.TryParse(value, out number);
        if (success)
        {
            Mines = number;
        }
        else {    
            Message("Could not set numer of mines to " + value + " please check that it is a valid number!");
        }
    }

    //initialize blank board before game is played
    private void InitBoard(int rows, int cols)
    {
        if (Mines > rows * cols || Mines <= 0)
        {
            Message("Invalid number of mines for board size!");
            return;
        }
        Tiles = new Tile[rows, cols];
        Camera.main.transform.position = new Vector3(rows / 2 * boardContainer.transform.localScale.x, cols / 2 * boardContainer.transform.localScale.y, -10);
        for(int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject currTile = Instantiate(tilePrefab);
                Tiles[row, col] = currTile.GetComponent<Tile>();
                Tiles[row, col].row = row;
                Tiles[row, col].col = col;
                currTile.transform.parent = boardContainer.transform;
                currTile.transform.localPosition = new Vector3(row, col, 0);
            }
        }
    }

    //start the game on first click and place mines, first click can't be a mine
    public void StartGame(int startingClickRow, int startingClickCol)
    {
        GameState = GameState.Playing;
        
        int placedMines = 0;
        while (placedMines < Mines)
        {
            int row = Random.Range(0, Tiles.GetLength(0));
            int col = Random.Range(0, Tiles.GetLength(1));

            //make sure not double placing a mine and adding a mine to the starting position
            if (!Tiles[row, col].isMine && (row != startingClickRow && col != startingClickCol))
            {
                placedMines++;
                Tiles[row, col].SetMine(true);
            }
        }
        PopulateAdjacentMineNumbers();
    }

    //populate adjacent mine numbers after mines are placed
    //itterate through neighbors for each position in the grid
    private void PopulateAdjacentMineNumbers()
    {
        for (int row = 0; row< Rows; row++)
        {
            for (int col = 0; col <Cols; col++)
            {
                int adjMines = 0;
                for (int i = -1; i <2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (row + i < 0 || row + i >= Rows || col + j < 0 || col + j >= Cols) continue;
                        if (Tiles[row+i, col+j].isMine) adjMines++;
                    }
                }
                Tiles[row, col].SetAdjMineNumber(adjMines);
            }
        }
    }

    //recursively reveal empty spaces
    public void RevealAdjacentEmpty(int row, int col)
    {
        Tiles[row, col].Reveal();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (row + i < 0 ||
                    row + i >= Rows ||
                    col + j < 0 ||
                    col + j >= Cols ||
                    Tiles[row + i, col + j].isMine ||
                    Tiles[row + i, col + j].revealed)
                {
                    continue;
                }
                else if (Tiles[row + i, col + j].adjMines != 0)
                {
                    Tiles[row+i, col+j].Reveal();
                }
                else
                {
                    RevealAdjacentEmpty(row + i, col + j);
                }
            }
        }          
    }

    // end game and reveal mines
    public void GameOver()
    {
        GameState = GameState.GameOver;
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (Tiles[row,col].isMine)
                {
                    Tiles[row, col].Reveal();
                }
            }
        }
    }

    public void CheckWin()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                if (!Tiles[row,col].isMine && !Tiles[row, col].revealed)
                {
                    return;
                }
            }
        }
        GameState = GameState.GameOver;
        Message("You Win!");
    }

    public void Message(string message)
    {
        messagePopUp.Pop(message);
    }

}

public enum GameState
{
    Ready,
    Playing,
    GameOver
}