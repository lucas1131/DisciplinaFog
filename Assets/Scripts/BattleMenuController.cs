using UnityEngine;
using System.Collections.Generic;

public class BattleMenuController : MonoBehaviour {

    private float PANEL_WIDTH;
    private float PANEL_HEIGHT;

    public float INITIAL_POSY;
    public float INITIAL_POSX;
    public float SPACING;

    public GameObject cursor;
    public GameObject unit;
    public GameObject trade;
    public GameObject wait;
    public GameObject status;
    public GameObject end;
    public GameObject rescue;
    public GameObject attack;
    public GameObject item;

    public GameObject currentEntry;

    private List<GameObject> entries;
    private int currentEntryIndex;
    private int arraySize;

    private int c = 0;

    //panel size = 20 + entries.length*50;
	void Start () {

        //default value, no need to change
        PANEL_WIDTH = 165;

        arraySize = 8;

        //sets up a list with all options
        entries = new List<GameObject>(arraySize);
        entries.Add(attack);
        entries.Add(rescue);
        entries.Add(item);
        entries.Add(trade);
        entries.Add(wait);
        entries.Add(unit);
        entries.Add(status);
        entries.Add(end);

        currentEntry = null;

        //updates the background panel size
        updatePanelSize();

        //sets corrects positions for all menu entries
        setPositions();

        //sets menu cursor position
        setCursorPosition();

    }
	
	void Update () {
        updatePanelSize();
        setPositions();
        setCursorPosition();
	}

    void updatePanelSize() {

        PANEL_HEIGHT = (calculateEntriesNo()*50) + 20;

        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PANEL_WIDTH);
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PANEL_HEIGHT);
    }

    int calculateEntriesNo() {
        int count = 0;
        //counts array elements set as true
        foreach(GameObject e in entries) if(e.activeSelf) count++;
        return count;
    }

    void setPositions() {
        int count = 0;

        foreach (GameObject e in entries) {
            if (e.activeSelf) {
                e.GetComponent<RectTransform>().anchoredPosition = 
                    new Vector2(INITIAL_POSX,INITIAL_POSY + (SPACING*count));
                count++;
            }
        }
    }

    public void next()
    {
        int i, j;
        GameObject cur = null;

        i = currentEntryIndex;

        if (calculateEntriesNo() == 0) return;

        //iterates through all array itens
        for (j = 0; j < arraySize; j++)
        {
            //checks if is in array end
            if (i + 1 >= arraySize) i = 0;
            else i++;

            //fetches entry in index i
            cur = entries[i];

            //if this entry is active, returns it
            if (cur.activeSelf)
            {
                currentEntryIndex = i;
                return;
            }
        }
    }

    public void prev()
    {
        int i, j;
        GameObject cur = null;

        i = currentEntryIndex;

        if (calculateEntriesNo() == 0) return;

        //iterates through all array itens
        for (j = 0; j < arraySize; j++)
        {
            //checks if is in array end
            if (i - 1 < 0) i = arraySize-1;
            else i--;

            //fetches entry in index i
            cur = entries[i];

            //if this entry is active, returns it
            if (cur.activeSelf)
            {
                currentEntryIndex = i;
                return;
            }
        }
    }

    public GameObject getCurrentEntry()
    {
        return entries[currentEntryIndex];
    }

    float getCurrentEntryY()
    {
        return entries[currentEntryIndex].GetComponent<RectTransform>().anchoredPosition.y;
    }

    void setCursorPosition() {
        if(calculateEntriesNo() <= 0)
        {
            this.cursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(INITIAL_POSX, -10);
        } else {
            this.cursor.GetComponent<RectTransform>().anchoredPosition = new Vector2(INITIAL_POSX, getCurrentEntryY());
        }

    }
}
