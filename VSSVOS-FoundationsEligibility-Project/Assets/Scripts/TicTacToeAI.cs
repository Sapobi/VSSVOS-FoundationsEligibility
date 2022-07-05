using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum TicTacToeState{none, cross, circle}

[System.Serializable]
public class WinnerEvent : UnityEvent<int>
{
}

public class TicTacToeAI : MonoBehaviour
{

	int _aiLevel;

	TicTacToeState[,] boardState;

	[SerializeField]
	private bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	private TicTacToeState playerState = TicTacToeState.circle;
	private TicTacToeState aiState = TicTacToeState.cross;

	[SerializeField]
	private GameObject _xPrefab;
	[SerializeField]
	private GameObject _oPrefab;
	[SerializeField]
	private GameObject _buttons;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	private ClickTrigger[,] _triggers;
	private int roundsPlayed;
	private bool gameEnded;
	
	private void Awake()
	{
		if(onPlayerWin == null){
			onPlayerWin = new WinnerEvent();
		}
	}

	public void StartAI(int AILevel){
		_aiLevel = AILevel;
		StartGame();
	}

	public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
	{
		_triggers[myCoordX, myCoordY] = clickTrigger;
	}

	private void StartGame()
	{
		_triggers = new ClickTrigger[3,3];
		boardState = new TicTacToeState[3,3];
		onGameStarted.Invoke();
	}

	public void PlayerSelects(int coordX, int coordY)
	{
		HandleSelection(coordX,coordY,playerState);
		
		if(!gameEnded) StartAITurn();
	}

	public void AiSelects(int coordX, int coordY)
	{
		HandleSelection(coordX,coordY,aiState);
	}

	private void HandleSelection(int coordX, int coordY, TicTacToeState selectorState)
	{
		roundsPlayed++;
		SetVisual(coordX, coordY, selectorState);
		boardState[coordX, coordY] = selectorState;
		
		CheckGameState(selectorState);

		_isPlayerTurn = !_isPlayerTurn;
		_buttons.SetActive(!_buttons.activeInHierarchy);
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}
	
	private void CheckGameState(TicTacToeState currentPlayerState)
	{
		//check for winner
		if ((boardState[0,0] == currentPlayerState && boardState[0,1] == currentPlayerState && boardState[0,2] == currentPlayerState) ||
			(boardState[1,0] == currentPlayerState && boardState[1,1] == currentPlayerState && boardState[1,2] == currentPlayerState) ||
			(boardState[2,0] == currentPlayerState && boardState[2,1] == currentPlayerState && boardState[2,2] == currentPlayerState) ||
			(boardState[0,0] == currentPlayerState && boardState[1,0] == currentPlayerState && boardState[2,0] == currentPlayerState) ||
			(boardState[0,1] == currentPlayerState && boardState[1,1] == currentPlayerState && boardState[2,1] == currentPlayerState) ||
			(boardState[0,2] == currentPlayerState && boardState[1,2] == currentPlayerState && boardState[2,2] == currentPlayerState) ||
			(boardState[0,0] == currentPlayerState && boardState[1,1] == currentPlayerState && boardState[2,2] == currentPlayerState) ||
			(boardState[2,0] == currentPlayerState && boardState[1,1] == currentPlayerState && boardState[0,2] == currentPlayerState))
		{
			gameEnded = true;
			onPlayerWin.Invoke(_isPlayerTurn ? 0 : 1);
		}
		else //check for tie, a.k.a. nine rounds has been played with no winner
		{
			if (roundsPlayed == 9)
			{
				gameEnded = true;
				onPlayerWin.Invoke(-1);
			}	
		}
	}

	private async void StartAITurn()
	{
		await Task.Delay(1000);
		
		if (_aiLevel == 0) AIEasy();
		else if (_aiLevel == 1) AIHard();
	}
	
	private ClickTrigger FindRandomAvailableTrigger()
	{
		List<ClickTrigger> availableTriggers = new List<ClickTrigger>();
		for (int i = 0; i < _gridSize; i++) {
			for (int j = 0; j < _gridSize; j++) {
				if (boardState[i,j] == TicTacToeState.none) {
					availableTriggers.Add(_triggers[i,j]);
				}
			}
		}
		int random = Random.Range(0, availableTriggers.Count);
		return availableTriggers[random];
	}

	private void AIEasy()
	{
		var selectedTrigger = FindRandomAvailableTrigger();
		
		//if can make 3 in a row:  do it
		//if can block player 3 in a row:  do it
		
		selectedTrigger.HandleAITriggerSelection();
	}

	private void AIHard()
	{
		var selectedTrigger = FindRandomAvailableTrigger();
		
		//implement MinMax algorithm to make this AI absolutely brutal
		
		selectedTrigger.HandleAITriggerSelection();
	}

}
