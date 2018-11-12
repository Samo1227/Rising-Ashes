using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMakerButton : MonoBehaviour
{ 

    private Button theButton;

    private ColorBlock theColor;

    public int loc_x;
    public int loc_y;

    public int int_type;

    public MapMaker mM_map;

    // Use this for initialization
    void Start ()
    {
        theButton = gameObject.GetComponent<Button>();
        theColor = gameObject.GetComponent<Button>().colors;
    }
	
    public void GetMap(MapMaker mM)
    {
        mM_map = mM;
    }

	// Update is called once per frame
	void Update ()
    {
        if (int_type == 0)
        {
            theColor.highlightedColor = Color.white;
            theColor.normalColor = Color.white;
            theColor.pressedColor = Color.white;
            theButton.colors = theColor;
        }

        else if (int_type == 1)
        {
            theColor.highlightedColor = Color.red;
            theColor.normalColor = Color.red;
            theColor.pressedColor = Color.red;
            theButton.colors = theColor;
        }

        else if (int_type == 2)
        {
            theColor.highlightedColor = Color.blue;
            theColor.normalColor = Color.blue;
            theColor.pressedColor = Color.blue;
            theButton.colors = theColor;
        }
    
    }

    public void SetLocation(int x, int y)
    {
        loc_x = x;
        loc_y = y;
    }

    public void TypeChange()
    {
        if (int_type < 2)
        {
            int_type++;
        }
        else
        {
            int_type = 0;
        }

        mM_map.ChangeMap(loc_x, loc_y, int_type);
    }
}
