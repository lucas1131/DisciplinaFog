using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovementDisplay : MonoBehaviour {

	public enum ArrowType {
		STUMP_UP,
		STUMP_DOWN,
		STUMP_LEFT,
		STUMP_RIGHT,

		UP_DOWN,
		UP_LEFT,
		UP_RIGHT,

		DOWN_UP,
		DOWN_LEFT,
		DOWN_RIGHT,

		LEFT_RIGHT,

		RIGHT_LEFT,

		ARROW_UP,
		ARROW_DOWN,
		ARROW_LEFT,
		ARROW_RIGHT
	}

	[HideInInspector]
	public Unit u;
	[HideInInspector]
	public List<GameObject> moveTiles, arrows;

	public GameObject moveTilePrefab, arrowPrefab;
	public Sprite[] arrowSprites;
	
	public void DestroyMovementDisplay(){
		foreach(GameObject go in moveTiles)
			GameObject.Destroy(go);

		if (arrows != null)
			foreach(GameObject go in arrows)
				GameObject.Destroy(go);
	}
	
	public void AddArrow(Position p, List<GameObject> arr, ArrowType type) {
	
		GameObject go = MonoBehaviour.Instantiate(
			arrowPrefab,
			new Vector3(p.x, p.y, 0f),
			Quaternion.identity
		) as GameObject;
	
		go.GetComponent<SpriteRenderer>().sprite = arrowSprites[(int) type];
		arr.Add(go);
	}

	public void AddArrow(Position p, List<GameObject> arr, Position p1, Position p2) {
		AddArrow(p, arr, GetArrowType(p1, p, p2));
	}

	public ArrowType GetArrowType(Position p1, Position p, Position p2) {
		p1 = p - p1;
		p2 = p2 - p;

		if (p1.x < 0) {
			if (p2.x < 0)
				return ArrowType.RIGHT_LEFT;
			else if (p2.y > 0)
				return ArrowType.UP_RIGHT;
			return ArrowType.DOWN_RIGHT;
		}

		if (p1.x > 0) {
			if (p2.x > 0)
				return ArrowType.LEFT_RIGHT;
			else if (p2.y > 0)
				return ArrowType.UP_LEFT;
			return ArrowType.DOWN_LEFT;
		}

		if (p1.y > 0) {
			if (p2.y > 0)
				return ArrowType.DOWN_UP;
			else if (p2.x < 0)
				return ArrowType.DOWN_LEFT;
			return ArrowType.DOWN_RIGHT;
		}

		// p1.y < 0
		if (p2.y < 0)
			return ArrowType.UP_DOWN;
		if (p2.x > 0)
			return ArrowType.UP_RIGHT;
		return ArrowType.UP_LEFT;
	}

	public void CreateArrows(List<GameObject> arr, List<Position> path) {
		Position d;
		int i = 1;

		d = path[0] - u.pos;
		if (d.x > 0)
			AddArrow(u.pos, arr, ArrowType.STUMP_RIGHT);
		else if (d.x < 0)
			AddArrow(u.pos, arr, ArrowType.STUMP_LEFT);
		else if (d.y > 0)
			AddArrow(u.pos, arr, ArrowType.STUMP_UP);
		else
			AddArrow(u.pos, arr, ArrowType.STUMP_DOWN);

		if (path.Count > 1) {
			AddArrow(path[0], arr, u.pos, path[1]);

			while (i < path.Count-1) {
				AddArrow(path[i], arr, path[i-1], path[i+1]);
				i++;
			}

			d = path[i] - path[i-1];
			if (d.x > 0)
				AddArrow(path[i], arr, ArrowType.ARROW_RIGHT);
			else if (d.x < 0)
				AddArrow(path[i], arr, ArrowType.ARROW_LEFT);
			else if (d.y > 0)
				AddArrow(path[i], arr, ArrowType.ARROW_UP);
			else
				AddArrow(path[i], arr, ArrowType.ARROW_DOWN);
		} else {
			if (d.x > 0)
				AddArrow(path[0], arr, ArrowType.ARROW_RIGHT);
			else if (d.x < 0)
				AddArrow(path[0], arr, ArrowType.ARROW_LEFT);
			else if (d.y > 0)
				AddArrow(path[0], arr, ArrowType.ARROW_UP);
			else
				AddArrow(path[0], arr, ArrowType.ARROW_DOWN);
		}
	}
}