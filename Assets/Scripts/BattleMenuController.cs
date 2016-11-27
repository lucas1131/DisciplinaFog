using UnityEngine;
using System.Collections;

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

    private ArrayList entries;

    //panel size = 20 + entries.length*50;
	void Start () {

        //default value, no need to change
        PANEL_WIDTH = 165;

        //sets up a list with all options
        entries = new ArrayList(8);
        entries.Add(attack);
        entries.Add(rescue);
        entries.Add(item);
        entries.Add(trade);
        entries.Add(wait);
        entries.Add(unit);
        entries.Add(status);
        entries.Add(end);

        //updates the background panel size
        updatePanelSize();

        //sets corrects positions for all menu entries
        setPositions();

        //sets menu cursor position
        setMenuCursorPosition();

    }
	
	void Update () {
        updatePanelSize();
        setPositions();
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
                Debug.Log(e.GetComponent<RectTransform>().anchoredPosition);
                e.GetComponent<RectTransform>().anchoredPosition = new Vector2(INITIAL_POSX,INITIAL_POSY + (SPACING*count));
                count++;
            }
        }
    }

    void setMenuCursorPosition() {

    }
}
