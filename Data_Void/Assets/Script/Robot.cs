using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {
    public int moveRange = 5;
    public int x;
    public int z;

    public List<Tile> selectableTiles = new List<Tile>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnMouseUp()
    {
        FindMoveTiles();
        CSGameManager.gameManager.SetCurrentRobot(this);
    }

    public void FindMoveTiles()
    {
        Tile currentTile = CSGameManager.gameManager.map[this.x, this.z];
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);
        while (process.Count > 0)
        {
            Tile tempTile = process.Dequeue();
            selectableTiles.Add(tempTile);
            tempTile.walkingSelection = true;
            if(tempTile.distanceFromStart < moveRange)
            {
                foreach(Tile neighbourTile in tempTile.neighboursList)
                {
                    neighbourTile.distanceFromStart = neighbourTile.movecost + tempTile.distanceFromStart;
                    if(neighbourTile.distanceFromStart <= moveRange)
                    {
                        process.Enqueue(neighbourTile);
                    }
                }
            }

        }

    }
    public void ClearWalkSelection()
    {
        foreach(Tile tempTile in selectableTiles)
        {
            tempTile.walkingSelection = false;
            tempTile.distanceFromStart = 0;
        }
        selectableTiles.Clear();
    }
    //public void MoveToSelection(Tile targetTile)
    //{

    //}


}
