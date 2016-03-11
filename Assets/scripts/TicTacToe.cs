using UnityEngine;
using System.Collections;

public class TicTacToe  {
//http://www.cppforschool.com/project/tic-tac-toe-project.html

static System.Random r = new System.Random();

char[] square = new char[10] {' ','1','2','3','4','5','6','7','8','9'};
// Mike: make first position of array indicate who's move it is from this position

/********************************************
	FUNCTION TO RETURN GAME STATUS
	1 FOR GAME IS OVER WITH RESULT
	-1 FOR GAME IS IN PROGRESS
	O GAME IS OVER AND NO RESULT
**********************************************/

	public TicTacToe() {
		if(r.Next(2) == 1) {  // randomly choose who's first
			square[0] = 'X';
		}
		else {
			square[0] = 'O';
		}
	}

	public TicTacToe(TicTacToe ttt) {
		for(int i = 0; i < 10; i++) {
			square[i] = ttt.square[i];
		}
	}
	
	public char whoseMove() {
			return square[0];
	}

	public void makeMove(int move) {
		if(square[move] == 'X' || square[move] == 'O') {
			// illegal move
			Debug.LogError("attempted move to sqaure already taken");
		}
		square[move] = square[0];
		if (square[0] == 'X') {
			square[0] = 'O';
		}
		else {
			square[0] = 'X';
		}
	}

	public string ToString() {
		return new string(square);
	}

	public ArrayList availableMoves() {
		ArrayList am = new ArrayList(9);
		//int j = 0;
		for(int i=1; i<10; i++) {
			if(square[i] != 'X' && square[i] != 'O') {
				//Debug.Log ("available: "+i.ToString());
				am.Add (i);
				//j++;
			}
		}
		return am;
	}

	public int checkwin()
		// return 1 if anyone won, 0 for draw and -1 if no winner yet
	{
		if (square[1] == square[2] && square[2] == square[3])
			return 1;
		else if (square[4] == square[5] && square[5] == square[6])
			return 1;
		else if (square[7] == square[8] && square[8] == square[9])
			return 1;
		else if (square[1] == square[4] && square[4] == square[7])
			return 1;
		else if (square[2] == square[5] && square[5] == square[8])
			return 1;
		else if (square[3] == square[6] && square[6] == square[9])
			return 1;
		else if (square[1] == square[5] && square[5] == square[9])
			return 1;
		else if (square[3] == square[5] && square[5] == square[7])
			return 1;
		else if (square[1] != '1' && square[2] != '2' && square[3] != '3' 
		         && square[4] != '4' && square[5] != '5' && square[6] != '6' 
		         && square[7] != '7' && square[8] != '8' && square[9] != '9')
			return 0;
		else
			return -1;
	}

	public int drawBoard(int x, int y, string title, bool acceptInput)
	{
		int[] xOffset = new int[10] {0,20,40,60,20,40,60,20,40,60};
		int[] yOffset = new int[10] {0,20,20,20,40,40,40,60,60,60};
		int lOffset = 5;

		int rc = 0;
		//Debug.Log("drawBoard called");
		GUI.Box(new Rect(x,y,100,100), title);

		for(int i=1; i<10; i++) {
			if(square[i] == 'X') {
				GUI.Label(new Rect(x+xOffset[i]+lOffset,y+yOffset[i],20,20), "X");
			}
			else if(square[i] == 'O') {
				GUI.Label(new Rect(x+xOffset[i]+lOffset,y+yOffset[i],20,20), "O");
			}
			else {
				if(acceptInput) {
					if(GUI.Button(new Rect(x+xOffset[i],y+yOffset[i],20,20), i.ToString())) {
						rc = i;
					}
				}
				else {
					GUI.Button(new Rect(x+xOffset[i],y+yOffset[i],20,20), " ");
				}
			}
		}
		return rc;
	}

}
