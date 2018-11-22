using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public int x;
    public int z;
    public int distanceFromStart = 0;

    public List<Tile> neighboursList = new List<Tile>();
    public bool walkingSelection = false;

    public Renderer colour;

    public int movecost = 1;
	// Use this for initialization
	void Start () {
        colour = gameObject.GetComponent<Renderer>();

    }
	
	// Update is called once per frame
	void Update () {
        if (walkingSelection)
        {
           // gameObject.GetComponent<Renderer>().material.color = Color.cyan;
            colour.material.color = Color.cyan;
        }
        else
        {
            colour.material.color = Color.white;
        }
	}

    public void FindNeighbours(Tile startTile)
    {
        int x = this.x;
        int z = this.z;
        CheckTiles(x, z + 1);
        CheckTiles(x, z - 1);
        CheckTiles(x + 1, z);
        CheckTiles(x - 1, z);

    }

    public void CheckTiles(int cX, int cZ)
    {
        Tile tT = null;
        if (cX >= 0 && cX <= 9 && cZ >= 0 && cZ <= 9)
        {
            tT = CSGameManager.gameManager.map[cX, cZ];
            neighboursList.Add(tT);
        }
    }

    private void OnMouseUp()
    {
        if (walkingSelection)
        {
            Robot rob = CSGameManager.gameManager.currentRobot;
            rob.transform.position = new Vector3(x, rob.transform.position.y, z);
            rob.x = x;
            rob.z = z;
            rob.ClearWalkSelection();

        }
    }
}
