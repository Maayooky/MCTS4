using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



/**
 * Created by Simon M. Lucas
 * sml@essex.ac.uk
 * Date: 03-Dec-2010
 * Time: 21:51:31
 */

/* converted to C# by Mike Silversides */

public /*static*/ class TreeNodeTest : MonoBehaviour {



	TicTacToe gameState; // = new TicTacToe();
	TreeNode tn = null;   // = new TreeNode(ttt);
	bool startPressed = false;
	bool showTree = false;
	bool treeDrawn = false;

	bool showNode = false;
	TreeNode curNode = null;

	int simulationCount = 500;
	int inset = 120;
	int minWidth = 300;
	int heightPerLevel = 40;

	int nodeWidth = 80;
	int nodeHeight = 20;

	TreeNode displayNode = null;
	int displayX = 0;
	int displayY = 0;

	TreeNode bestChild = null;
	Double bestChildValue = 0;
	int bestChildxx = 0;
	int bestChildyy = 0;

	int moveChosen = 0;
	int move = 0;
	bool stillPlaying = true;
	bool computersTurn;
	string msg = "";
	static System.Random r = new System.Random();

	public GameObject nodePrefab;
	public GameObject linePrefab;
	public GameObject treeCamera;

	DrawTree2 dt2;

	// Use this for initialization
	void Start () {
		dt2 = new DrawTree2(nodePrefab,linePrefab,treeCamera);
	}
	
	// Update is called once per frame
	void Update () {
		dt2.DTUpdate();
	}
	
	void OnGUI () {
		if(!startPressed) {
			GUI.Label(new Rect(10, 180, 400, 20), "Tic Tac Toe: computer plays X, random player starts");
            GUI.Label(new Rect(10, 200, 400, 20), "Game pauses for you to accept computer move.");
            GUI.Label(new Rect(10, 220, 400, 20), "You can display the search tree at this point.");
            GUI.Label(new Rect(10, 240, 400, 20), "You also can have it run more MCTS simulations.");
        }
		if(startPressed && stillPlaying) {
			if(gameState.whoseMove() == 'X' /*computersTurn*/ ) {
				move = gameState.drawBoard(10,200,msg,false);
				TreeNode BC = tn.bestChild();
				if(GUI.Button(new Rect(20,100,80,20), "accept:"+BC.lastMove.ToString())) {
					move = BC.lastMove;
				} else {
					move = 0;
				}
			}
			else {
				move = gameState.drawBoard(10,200,msg,stillPlaying);
			}
		}
		if(startPressed && (stillPlaying == false)) { //game over
			move = gameState.drawBoard(10,200,msg,false);
			move = 0;
			if(	GUI.Button(new Rect(20,100,80,20), "Play Again?")) {
				startPressed = false;
				stillPlaying = true;
				showTree = false;
			}
		}

		if(move > 0 && moveChosen == 0) {
			moveChosen = move;
			move = 0;
		}

		if(moveChosen > 0){
			//Debug.Log ("move chosen: "+moveChosen);
			gameState.makeMove(moveChosen);
			moveChosen = 0;
			switch(gameState.checkwin()){ 
			case 1 : // we have a winner
				if(gameState.whoseMove() == 'O') {
					msg = "Computer wins";
				}
				else {
					msg = "You Win!";
				}
				stillPlaying = false; 
				break;
			case 0 : // draw
				stillPlaying = false; 
				msg = "It's a draw";
				break;  
			default : /* keep playing */ 
				if(gameState.whoseMove() == 'X') {
					//computersTurn = true;
					//Debug.Log ("available moves: " + gameState.availableMoves().Count.ToString());
					TicTacToe ttt = new TicTacToe(gameState);
					tn = new TreeNode(ttt);
					test();
					msg = "Computer's turn";
				}
				else {
					msg = "Your turn";
					tn = null;
				}
				break;
			}
		}


		GUI.Box(new Rect(10,10,100,150), "MCTS Test");
		
		// Make the first button. 
		if(startPressed==false) {
			if(GUI.Button(new Rect(20,40,80,20), "Start Game")) {
				startPressed = true;
				gameState = new TicTacToe();

				if(gameState.whoseMove() == 'X') {
					//computersTurn = true;
					TicTacToe ttt = new TicTacToe(gameState);
					tn = new TreeNode(ttt);
					test();
					msg = "Computer's turn";
				}
				else {
					tn = null;
					msg = "Your turn";
				}
			}
		}
		else {
			if(gameState.whoseMove() == 'X' /*computersTurn*/ && stillPlaying && GUI.Button(new Rect(20,40,80,20), "more MCTS")) {
				test();
			}
		}
		// Make the second button.
		if(startPressed && stillPlaying) {
			if(gameState.whoseMove() == 'X') { /*computersTurn*/
				if(showTree) {
					if(GUI.Button(new Rect(20,70,80,20), "Hide Tree")) {
						showTree = false;
						dt2.destroyTree();  // remove onscreen tree objects
						treeDrawn = false;
					}
				}
				else {
					if(GUI.Button(new Rect(20,70,80,20), "Show Tree")) {
						showTree = true;
					}
				}
			}
		}

		if (showTree && tn != null && stillPlaying) {  // draw tree
            GUI.Label(new Rect(10, 300, 400, 20), "Use mouse wheel to zoom in/out. Right-click and drag to pan.");
            GUI.Label(new Rect(10, 320, 400, 20), "Node values: last move: current total / number of visits.");
            if (treeDrawn == false) {
				treeDrawn = true;   // only "draw" the tree once
				dt2.drawTree(tn); 
			}
			else {
				dt2.DTOnGUI(Event.current);  // handle mouse when tree is displayed
			}

			//draw (tn,(Screen.width+inset)/2,50,Screen.width-inset);
		}

		if(showNode) {
			string t;
			switch(curNode.gameState.checkwin()){ 
			case 1 : // we have a winner
				if(gameState.whoseMove() == 'O') {
					t = "Computer wins";
				}
				else {
					t = "Player wins";
				}
				break;
			case 0 : 
				t = "It's a draw";
				break;  
			default : /* keep playing */ 
				t = "whose move:"+curNode.gameState.whoseMove();
				break;
			} 
			curNode.gameState.drawBoard(curNode.x,curNode.y+25,t,false);
		}
	}

	void draw(TreeNode cur, int x, int y, int wFac) {
		// draw this one, then it's children

		int arity = cur.arity();
		for (int i = 0; i < arity; i++) {
			if (cur.children[i].nVisits > 0) {
				int xx = (int) ((i + 1.0) * wFac / (arity + 1) + (x - wFac / 2));
				int yy = y + heightPerLevel;

				//draw from centre bottom edge of upper node to centre top edge of lower node
				Drawing.DrawLine(new Vector2(x+(nodeWidth/2),y+nodeHeight),new Vector2(xx+(nodeWidth/2),yy),Color.black,1.0f,true);
				draw(cur.children[i], xx, yy, wFac / arity);

			}
		}

		cur.x = x;
		cur.y = y;
		String s = cur.lastMove + ": " + (int) cur.totValue + "/" + (int) cur.nVisits;
		if(GUI.Button(new Rect(x,y,nodeWidth,nodeHeight), s)) {
			//Debug.Log("inside node button logic");
			if(showNode) {
				showNode = false;
				curNode = null;
			}
			else {
				showNode = true;
				curNode = cur;
			}
		}
	}
	
	//public static void main(String[] args) {
	void test() {
		// not sure but it doesnt look like "list" is actually used
		//LinkedList<TreeNode> list = new LinkedList<TreeNode>();  

		//List<TreeNode> list = new LinkedList<TreeNode>();
		//list.add(null);
		//System.out.println("list: " + list);

		//TreeNode tn = new TreeNode();
		//list.AddLast (tn);  // try this
		ElapsedTimer t = new ElapsedTimer();
		//int n = 1000;
		for (int i=0; i<simulationCount; i++) {
			tn.selectAction();
		}
		Debug.Log("MCTS elapsed: " + t.toString()); //System.out.println(t);
		//TreeView tv = new TreeView(tn);
		//tv.showTree("After " + n + " play outs");
		//Debug.Log("Done.");   
	}
}
