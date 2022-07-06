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
		
		int winner = CheckGameEndState(boardState,selectorState);

		if (winner == -1)
		{
			_isPlayerTurn = !_isPlayerTurn;
			_buttons.SetActive(!_buttons.activeInHierarchy);
		}
		else
		{
			gameEnded = true;
			onPlayerWin.Invoke(winner);
		}
	}

	private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
	{
		Instantiate(
			targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
			_triggers[coordX, coordY].transform.position,
			Quaternion.identity
		);
	}
	
	private int CheckGameEndState(TicTacToeState[,] board, TicTacToeState currentPlayerState)
	{
		//check for winner
		if ((board[0,0] == currentPlayerState && board[0,1] == currentPlayerState && board[0,2] == currentPlayerState) ||
			(board[1,0] == currentPlayerState && board[1,1] == currentPlayerState && board[1,2] == currentPlayerState) ||
			(board[2,0] == currentPlayerState && board[2,1] == currentPlayerState && board[2,2] == currentPlayerState) ||
			(board[0,0] == currentPlayerState && board[1,0] == currentPlayerState && board[2,0] == currentPlayerState) ||
			(board[0,1] == currentPlayerState && board[1,1] == currentPlayerState && board[2,1] == currentPlayerState) ||
			(board[0,2] == currentPlayerState && board[1,2] == currentPlayerState && board[2,2] == currentPlayerState) ||
			(board[0,0] == currentPlayerState && board[1,1] == currentPlayerState && board[2,2] == currentPlayerState) ||
			(board[2,0] == currentPlayerState && board[1,1] == currentPlayerState && board[0,2] == currentPlayerState))
		{
			return currentPlayerState.GetHashCode();
		}
		//check for tie, a.k.a. nine rounds has been played with no winner
		if (roundsPlayed == 9)
		{
			return 0;
		}
		return -1;
	}

	private async void StartAITurn()
	{
		await Task.Delay(1000);
		
		if (_aiLevel == 0) AIEasy().HandleAITriggerSelection();
		else if (_aiLevel == 1) AIHard().HandleAITriggerSelection();
	}
	
	private List<ClickTrigger> FindAvailableTriggers()
	{
		List<ClickTrigger> availableTriggers = new List<ClickTrigger>();
		for (int i = 0; i < _gridSize; i++) {
			for (int j = 0; j < _gridSize; j++) {
				if (boardState[i,j] == TicTacToeState.none) {
					availableTriggers.Add(_triggers[i,j]);
				}
			}
		}
		return availableTriggers;
	}
	
	private ClickTrigger FindRandomAvailableTrigger()
	{
		var availableTriggers = FindAvailableTriggers();
		int random = Random.Range(0, availableTriggers.Count);
		return availableTriggers[random];
	}

	private ClickTrigger AIEasy()
	{
		var availableTriggers = FindAvailableTriggers();
		
		//find a move that would cause AI to win, and choose it
		foreach (var trigger in availableTriggers)
		{
			//pretend to do an AI move on a copy of the current boardState
			var boardCopy = (TicTacToeState[,])boardState.Clone();
			boardCopy[trigger._myCoordX, trigger._myCoordY] = aiState;

			//If the AI wins, choose the move
			if (CheckGameEndState(boardCopy, aiState) == 1) return trigger;
		}
		
		//find a move that would cause player to win, and choose to block it
		foreach (var trigger in availableTriggers)
		{
			//pretend to do a player move on a copy of the current boardState
			var boardCopy = (TicTacToeState[,])boardState.Clone();
			boardCopy[trigger._myCoordX, trigger._myCoordY] = playerState;
			
			//If the player wins, choose the move to block it
			if (CheckGameEndState(boardCopy, playerState) == 2) return trigger;
		}
		
		return FindRandomAvailableTrigger();
	}

	private ClickTrigger AIHard()
	{
		//implement MinMax algorithm to make this AI absolutely brutal
		
		return FindRandomAvailableTrigger();
	}

}
