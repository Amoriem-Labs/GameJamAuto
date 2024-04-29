using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public enum GameplayState { NOT_PLAYING, HERO_SETUP, BATTLING, CASTING };

    public GameplayState gameplayState = GameplayState.NOT_PLAYING;

    public float currentMana = 0;
    public float maxMana;

    public Tile hoveredTile;

    void Update()
    {
        GameObject hoveredTile = GetHoveredTile();
    }

    private GameObject GetHoveredTile()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            if (hit.collider.gameObject.CompareTag("Tile"))
            {
                return hit.collider.gameObject;
            }
        }
        return null;
    }

    public void startBattle()
    {

    }
}
