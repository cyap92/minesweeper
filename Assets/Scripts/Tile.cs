using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public bool isMine;
    public bool revealed;
    public bool flag;
    public int adjMines = -1;
    public int row;
    public int col;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] TextMesh adjMinesText;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite pressedSprite;
    [SerializeField] GameObject flagSprite;
    [SerializeField] GameObject mineSprite;

    //handle left click logic
    public void Reveal()
    {
        //if game is not over and non revealed space
        if (!revealed && GameManager.Instance.GameState != GameState.GameOver)
        {
            spriteRenderer.sprite = pressedSprite;
            revealed = true;

            //if first spot, start game logic
            if (GameManager.Instance.GameState == GameState.Ready)
            {
                GameManager.Instance.StartGame(row, col);
                adjMinesText.gameObject.SetActive(true);

                //if non boarder space, reveal adjacecnt
                if (adjMines == 0)
                {
                    GameManager.Instance.RevealAdjacentEmpty(row, col);
                }

            }
            // handle click logic while game is being played
            else if (GameManager.Instance.GameState == GameState.Playing)
            {
                //game over mine hit
                if (isMine)
                {
                    mineSprite.SetActive(true);
                    spriteRenderer.color = Color.red;
                    GameManager.Instance.GameOver();
                }
                // safe move
                else
                {
                    adjMinesText.gameObject.SetActive(true);
                    if (adjMines == 0)
                    {
                        GameManager.Instance.RevealAdjacentEmpty(row, col);
                    }
                }
                GameManager.Instance.CheckWin();
            }
  
        }
        //show mines when game over
        if (GameManager.Instance.GameState == GameState.GameOver)
        {
            if (isMine)
            {
                mineSprite.SetActive(true);
                flagSprite.SetActive(false);
            }
        }
    }

    public void SetMine(bool _isMine)
    {
        isMine = _isMine;
    }

    //right click flag logic
    public void Flag()
    {
        if (revealed) return;
        if (!flag)
        {
            flagSprite.SetActive(true);
            flag = true;
        }
        else
        {
            flagSprite.SetActive(false);
            flag = false;
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Reveal();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Flag();
        }
    }

    public void SetAdjMineNumber(int mines)
    {
        adjMines = mines;
        if (mines == 0) adjMinesText.text = "";
        else
        {
            adjMinesText.text = "" + mines;
        }
    }
}

