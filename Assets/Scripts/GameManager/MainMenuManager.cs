using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void OnSinglePlayerClick()
    {
        GameModManager.SetMod(GameMod.Single);
        GameManager.Instance.SetGameState(GameState.TeamSelectMenu);
        SceneLoader.Instance.LoadScene("SingleTeamSelectScene");
    }
}
