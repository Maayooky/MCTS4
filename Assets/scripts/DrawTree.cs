using UnityEngine;
using System.Collections;

public class DrawTree : MonoBehaviour {

	TreeNode tn = null;
	 public GameObject nodeTestPrefab;
	 public GameObject linePrefab;
	 public GameObject treeCamera;



	// Use this for initialization
	void Start () {
		TicTacToe gameState = new TicTacToe();
		gameState.makeMove (5);
		gameState.makeMove (1);
		gameState.makeMove (4);

		tn = new TreeNode(gameState);
		for (int i=0; i<50; i++) {
			tn.selectAction();
		}
		draw (tn,100,0,200);
	}

	void  drawLine(int x, int y, int xx, int yy) {
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
		
		//					Debug.Log("x=" + x);
		//					Debug.Log("y=" + y);
		//					Debug.Log("xx=" + xx);
		//					Debug.Log("yy=" + yy);
		//					Debug.Log ("h=" +h);
		//					Debug.Log("angle=" + angle);
		
		var l = Instantiate (linePrefab, new Vector3(xLine,yLine,0), Quaternion.Euler(0,0,angle)) as GameObject;
		l.transform.localScale = new Vector3(0.15f, h, 0.15f);

	}

	public  void draw(/*Graphics2D g,*/ TreeNode cur, int x, int y, int wFac) {
		// draw children first, then the node

		int arity = cur.arity();
		Vector3 xy = new Vector3(x,y,0);

		for (int i = 0; i < arity; i++) {
			if (cur.children[i].nVisits > 0) {
				int xx = (int) ((i + 1.0) * wFac / (arity + 1) + (x - wFac / 2));
				int yy = y - 20;

				drawLine (x,y,xx,yy);

				draw(cur.children[i], xx, yy, wFac / arity);
			}
		}
		var nn = Instantiate (nodeTestPrefab, new Vector3(x,y,0), Quaternion.identity) as GameObject;
		nn.transform.Rotate (0.0f,90.0f,90.0f);

		cur.x = x;
		cur.y = y;
		string s = cur.lastMove + ": " + (int) cur.totValue + "/" + (int) cur.nVisits;
		var tLabel =  nn.transform.GetChild(0).gameObject.GetComponent("TextMesh") as TextMesh;
		tLabel.text = s;
	}

	// Update is called once per frame
	void Update () {

	}

	// Move the scroll wheel to determine
	// the X & Y scrolling amount.
	void OnGUI() {
		Event e = Event.current;
		if (e.type == EventType.MouseDrag) {
			treeCamera.transform.Translate(-e.delta.x, e.delta.y, 0);
		}
		if (e.type == EventType.ScrollWheel) {
			treeCamera.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel")*100);

		}
	}
}
