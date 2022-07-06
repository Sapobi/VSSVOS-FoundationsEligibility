using System;
using System.Collections.Generic;
using UnityEngine;

public class ClickTrigger : MonoBehaviour
{
	TicTacToeAI _ai;

	[SerializeField] public int _myCoordX;
	[SerializeField] public int _myCoordY;

	[SerializeField]
	private bool canClick;

	private void Awake()
	{
		_ai = FindObjectOfType<TicTacToeAI>();
	}

	private void Start(){

		_ai.onGameStarted.AddListener(AddReference);
		_ai.onGameStarted.AddListener(() => SetInputEndabled(true));
		_ai.onPlayerWin.AddListener((win) => SetInputEndabled(false));
	}

	private void SetInputEndabled(bool val){
		canClick = val;
	}

	private void AddReference()
	{
		_ai.RegisterTransform(_myCoordX, _myCoordY, this);
	}

	private void OnMouseDown()
	{
		if(canClick)
		{
			_ai.PlayerSelects(_myCoordX, _myCoordY);
			canClick = false;
		}
	}

	public void HandleAITriggerSelection()
	{
		_ai.AiSelects(_myCoordX, _myCoordY);
		canClick = false;
	}
}
