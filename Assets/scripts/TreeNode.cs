using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TreeNode {
	static System.Random r = new System.Random();
	//static int nActions = 5;
	static double epsilon = 1e-6;
	static double Cp = 2*(1/Math.Sqrt(2));

	public TreeNode[] children;
	public double nVisits, totValue;
	public TicTacToe gameState;
	public int lastMove = 0;
	public int x = 0;
	public int y = 0;

	public TreeNode(TicTacToe ttt) {
		gameState = new TicTacToe(ttt);

	}

	public void selectAction() {
		LinkedList<TreeNode> visited = new LinkedList<TreeNode>();
		TreeNode cur = this;
		visited.AddLast(this);   //visited.add(this);
		while (!cur.isLeaf()) {
			cur = cur.select();
			// System.out.println("Adding: " + cur);
			visited.AddLast(cur); //visited.add(cur);
		}
		if (cur.gameState.checkwin() == -1)  // non-terminal state
		{
			cur.expand();
			TreeNode newNode = cur.select();
			if(newNode != null) {
				visited.AddLast(newNode); //visited.add(newNode);
				double value = rollOut(newNode);
				foreach (TreeNode node in visited) {
					node.updateStats(value);
				}
			}
			else Debug.LogError ("select action selected a null node");
		}
		else {
			double value = 0;
			switch(cur.gameState.checkwin()){ 
			case 1 : // we have a winner
				if(cur.gameState.whoseMove() == 'O') { // computer won
					value = 1.0;  
				}
				else {
					value = 0.0;   //try to make it avoid losing with -1, didn't  seem to help
				}
				break;
			case 0 : // draw
				value = 0.0; 
				break;  
			default : /* keep playing */ 
				//Debug.Log ("keep playing");

				break; 
			}
			foreach (TreeNode node in visited) {
				node.updateStats(value);
				//value = value * -1;
			}
		}
	}

	public void expand() {
		//int i = 0;
		//Debug.Log("expand()");
		//children = new TreeNode[nActions];
		ArrayList am;
		am = gameState.availableMoves();
		if (am.Count > 0) {
			children = new TreeNode[am.Count];
			for(int i=0; i < am.Count; i++) {
			//foreach (int move in am) {
				children[i] = new TreeNode(gameState);
				children[i].gameState.makeMove((int) am[i]);
				children[i].lastMove = (int) am[i];
				//i++;
			}
		}
	}
	
	private TreeNode select() {
		//Debug.Log("select()");
		TreeNode selected = null;
		double bestValue = Double.MinValue;  
		foreach (TreeNode c in children) {
			double uctValue =
				c.totValue / (c.nVisits + epsilon ) +
					Cp*Math.Sqrt(2*Math.Log(nVisits+1) / (c.nVisits + epsilon) ) +
					r.NextDouble() * epsilon;


//			c.totValue / (c.nVisits + epsilon) +
//				Math.Sqrt(Math.Log(nVisits+1) / (c.nVisits + epsilon)) +
//					r.NextDouble() * epsilon;

			// small random number to break ties randomly in unexpanded nodes
//			Debug.Log("UCT value = " + uctValue);
			if (uctValue > bestValue) {
				selected = c;
				bestValue = uctValue;
			}
		}
		// System.out.println("Returning: " + selected);
		return selected;
	}

	public bool isLeaf() {
		//Debug.Log("isLeaf()");

		return children == null;

	}
	
	public double rollOut(TreeNode tn) {
		//Debug.Log("rollOut()");
		TicTacToe rollGS = new TicTacToe(tn.gameState);
		bool stillPlaying = true;
		double rc = 0;
		int move;

		//Debug.Log ("Start rollout .............................");

		ArrayList am = rollGS.availableMoves();
		while ((am.Count > 0) && stillPlaying) {
			//Debug.Log ("game = " + rollGS.ToString());
			//Debug.Log("am.count = " + am.Count.ToString());
			//for(int i = 0; i< am.Count; i++) {
			//	Debug.Log("am["+i.ToString()+"]="+am[i].ToString());
			//}
			move = r.Next(am.Count);
			//Debug.Log ("move = "+move.ToString());
			//Debug.Log ("moveChosen = "+am[move].ToString());

			rollGS.makeMove((int)am[move]);  // make a random move
			//Debug.Log ("game = " + rollGS.ToString());

			switch(rollGS.checkwin()){ 
			case 1 : // we have a winner
				//Debug.Log("we have a winner");
				stillPlaying = false; 
				if(rollGS.whoseMove() == 'O') { // computer won
					rc = 1.0;  
				}
				break;
			case 0 : // draw
				//Debug.Log ("draw");
				stillPlaying = false;
				rc = 0.0; // better than a loss
				break;  
			default : /* keep playing */ 
				//Debug.Log ("keep playing");
				//rc = -1; // try to make it avoid losing
				am = rollGS.availableMoves();
				break; 
			}
			//Debug.Log ("after switch, time to test while");
		}
		//Debug.Log ("end of rollOut");
		return rc; //return r.nextInt(2);
	}
	
	public void updateStats(double value) {
		//Debug.Log("updateStats()");
		nVisits++;
		totValue += value;
	}
	
	public int arity() {
		//Debug.Log("arity()");
		return children == null ? 0 : children.Length; //return children == null ? 0 : children.length;
	}

	public TreeNode bestChild() {
		// if only one child choose it
		// check if a win for computer, if so make it best and stop  
		// look ahead one move and check of win for person, if so, pick best alternate  <== not yet implmented
		TreeNode bestChild = null;
		bool bestChildLoses = false;
		//TreeNode bestAlternate = null;
		//int canWin = 0;

		for (int i=0; i < children.Length; i++) {
			if(bestChild == null) {
				bestChild = children[i];
				if(bestChild.gameState.checkwin() == 1) {  // computer can win
					break;  // stop looking
				}
				bestChildLoses = PersonCanWin(bestChild);  // test best child for later
			}
			else {
				if(children[i].gameState.checkwin() == 1) {  // computer can win
					bestChild = children[i];
					break;  // stop looking
				}
				else {
					if (bestChildLoses) { // if best current move is losing, pick anything
						bestChild = children[i];
						bestChildLoses = PersonCanWin(bestChild);  // test best child for later
					}
					else { // check new move
						if (PersonCanWin(children[i]) == false) { // avoid trap, skip move if true
							if (children[i].totValue > bestChild.totValue) { //not a trap, is it better?
								bestChild = children[i];
							}
						}
					}
				}
			}
		}
		return bestChild;
	}

	public bool PersonCanWin(TreeNode currentPosition) {
		bool canWin = false;
		ArrayList am = currentPosition.gameState.availableMoves();

		foreach (int move in am)
		{
			TicTacToe lookAhead = new TicTacToe(currentPosition.gameState);
			lookAhead.makeMove(move);
			if (lookAhead.checkwin() == 1) {
				canWin = true;
				break;
			}
		}
		return canWin;
	}
}
