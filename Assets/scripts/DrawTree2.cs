using UnityEngine;
using System.Collections;

public class DrawTree2 {

	public class Tree {
		public TreeNode tNode;   	// the game's TreeNode
		public GameObject dtNode;  // the node to draw
		public GameObject parentLine = null;  // line to parent
		public Tree[] children;    
		//public GameObject[] childLines;
		public int arity;				// number of children
		public int totalWidth = 10;		// width of child nodes or current node if no children.
		public int x,y;

		public Tree (TreeNode tn, /*GameObject t,*/ int n) {
			tNode = tn;
			//dtNode = t;

			arity = n;
			if (arity > 0) {
				children = new Tree[n];
				//childLines = new GameObject[n];
			}
		}

		public void addParent(GameObject t) {
			dtNode = t;
		}

		public void addParentLine(GameObject t) {
			parentLine = t;
		}

		public void addChild(Tree t, int n) {
			children[n] = t;
		}

//		public void addLineGameObject(GameObject l, int n) {
//			childLines[n] = l;
//		}

	}

	GameObject nodePrefab;
	GameObject linePrefab;
	GameObject treeCamera;
	Tree saveTree;

	int maxX;

	public DrawTree2 (GameObject n, GameObject p, GameObject c) {
		nodePrefab = n;
		linePrefab = p;
		treeCamera = c;
		saveTree = null;
		maxX = 0;
	}

	GameObject  drawLine(int x, int y, int xx, int yy) {
		// utility function
		// use line prefab to "draw" a line from (x,y) to (xx,yy)

		float xLine = (x + xx) / 2;  //calc half way
		float yLine = (y + yy) / 2;

		float adjacent = Mathf.Abs(yy) - Mathf.Abs(y);
		float opposite = Mathf.Abs(xx) - Mathf.Abs(x);
		float angle;
		if (adjacent == 0) {
			angle = 0f;
		}
		else {
			angle = Mathf.Rad2Deg *
				Mathf.Atan(opposite / adjacent);
		}
		float h = Mathf.Sqrt(opposite * opposite  + adjacent * adjacent); // Pythagoras' Theorem
		
		var l = GameObject.Instantiate (linePrefab, new Vector3(xLine,yLine,0), Quaternion.Euler(0,0,angle)) as GameObject;
		l.transform.localScale = new Vector3(0.15f, h, 0.15f);

		return l;
	}

	public void destroyTree() {
		destroy (saveTree);
	}

	void destroy(Tree t) {
		// remove the onscreen GameObjects for this tree
		// removes the children, then the line to the parent, then the node itself
		if(t != null) {
			if(t.arity > 0) {
//				foreach(GameObject line in t.childLines) {
//					Object.Destroy (line);
//				}
				foreach(Tree child in t.children) {
					destroy(child);
				}
			}
			if (t.parentLine != null) {
				Object.Destroy (t.parentLine);
			}
			Object.Destroy(t.dtNode); 
		}
	}

	public void drawTree(TreeNode tn) {
		maxX = 0;
		saveTree = draw (tn, null, 0, 0);


	}

	Tree draw(TreeNode cur, Tree parent, int x, int y) {
		// draw children, then draw the current node centred.
		// draw line from current to parent
		// save the drawTree as we 'draw' it.

		var newNode = new Tree(cur,/*dNode,*/ cur.arity ());  // create tree for gameObjects to be drawn

		int treeWidth = 0;
		newNode.x = x;
		newNode.y = y;

		if(parent != null) {  // draw line to parent
			GameObject line = drawLine (parent.x,parent.y,newNode.x,newNode.y);
			newNode.addParentLine(line);
		}

		// create child drawtree nodes first, adding up width
		int yy = y - 20;
		//int childWidths = 0;

		int xx = x;
		for (int i = 0; i < cur.arity(); i++) {
			newNode.addChild(draw(cur.children[i], newNode, xx, yy),i);
				//childWidths = childWidths + newNode.children[i].totalWidth;
				//xx = xx + 20;

				//if(i>0 ) {
			xx = xx + 12;
				//}
			if (maxX > xx) {
				xx = maxX;
			}
		}

		if (xx > maxX) {
			maxX = xx;
		}
		// make current gameObject
		newNode.dtNode = GameObject.Instantiate (nodePrefab, new Vector3(newNode.x,newNode.y,0), Quaternion.identity) as GameObject;
		newNode.dtNode.transform.Rotate (0.0f,90.0f,90.0f);

		//newNode.totalChildWidth = childWidths;
		
		string s = cur.lastMove + ": " + (int) cur.totValue + "/" + (int) cur.nVisits;
		var tLabel =  newNode.dtNode.transform.GetChild(0).gameObject.GetComponent("TextMesh") as TextMesh;
		tLabel.text = s;  // set node label

		return newNode;
	}


	// Click and Drag to move the screen view of the tree
	// Move the scroll wheel to zoom in/out
	public void DTOnGUI(Event e) {
		if (e.type == EventType.MouseDrag) {
			treeCamera.transform.Translate(-e.delta.x, e.delta.y, 0);
		}
		if (e.type == EventType.ScrollWheel) {
			treeCamera.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel")*100);
			
		}
	}

	// handle input for things like node mouse clicks
	public void DTUpdate() {

		RaycastHit nodeHit;
		GameObject nodeClicked;

		if (Input.GetButtonDown("Fire1")) {
			var ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray1, out nodeHit, Mathf.Infinity)) {
				// hit a node
				//Debug.Log("Node Hit ");
				nodeClicked = nodeHit.transform.gameObject;
				nodeClicked.gameObject.GetComponent<Renderer>().material.color = Color.red;
				Debug.Log(nodeClicked.name);
				Debug.Log(nodeClicked.transform.GetChild(0).gameObject.name);
				var testLabel =  nodeClicked.transform.GetChild(0).gameObject.GetComponent("TextMesh") as TextMesh;
				Debug.Log(testLabel.text);

				//display game board for this node
			}
			else {
				//Debug.Log("Node Miss ");
			}
		}
	}


}
