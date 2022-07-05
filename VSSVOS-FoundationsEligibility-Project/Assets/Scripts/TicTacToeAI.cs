using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
	public bool _isPlayerTurn;

	[SerializeField]
	private int _gridSize = 3;
	
	private TicTacToeState playerState = TicTacToeState.circle;
	private TicTacToeState aiState = TicTacToeState.cross;

	[SerializeField]
	private GameObject _xPrefab;

	[SerializeField]
	private GameObject _oPrefab;

	public UnityEvent onGameStarted;

	//Call This event with the player number to denote the winner
	public WinnerEvent onPlayerWin;

	private ClickTrigger[,] _triggers;
	private int roundsPlayed;
	
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

	public void PlayerSelects(int coordX, int coordY){

		HandleSelection(coordX,coordY,playerState);
		_isPlayerTurn = false;
	}

	public void AiSelects(int coordX, int coordY){

		HandleSelection(coordX,coordY,aiState);
		_isPlayerTurn = true;
	}

	private void HandleSelection(int coordX, int coordY, TicTacToeState selectorState)
	{
		roundsPlayed++;
		SetVisual(coordX, coordY, selectorState);
		boardState[coordX, coordY] = selectorState;
		
		CheckGameState(selectorState);
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
			onPlayerWin.Invoke(_isPlayerTurn ? 0 : 1);
		}
		else //check for tie, a.k.a. nine rounds has been played with no winner
		{
			if(roundsPlayed == 9) onPlayerWin.Invoke(-1);	
		}
	}
}
