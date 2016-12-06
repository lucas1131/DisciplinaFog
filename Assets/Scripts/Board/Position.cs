using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Position {

    [SerializeField]
    private int _x, _y;

    public int x {
        set { _x = value; }
        get { return _x; }
    }

    public int y {
        set { _y = value; }
        get { return _y; }
    }


    public Position(int x, int y) {
        this._x = x;
        this._y = y;
    }

    public Position(Position p) {
        this._x = p.x;
        this._y = p.y;
    }

    public bool IsContained(int xMax, int yMax) {
        return this.x < xMax && this.y < yMax;
    }

    public bool IsContained(int xMin, int yMin, int xMax, int yMax) {
        return this.x >= xMin && this.y >= yMin && this.IsContained(xMax, yMax);
    }

    public bool IsValid(BoardManager board) {
        return this.IsContained(0, 0, board.cols, board.rows);
    }

    public List<Position> ValidNeighbors(BoardManager board) {

        Position[] deltas = new Position[] {
            new Position(1, 0),
            new Position(-1, 0),
            new Position(0, 1),
            new Position(0, -1),
        };

        List<Position> l = new List<Position>();

        foreach (Position delta in deltas) {
            Position p = this + delta;
            if (p.IsValid(board))
                l.Add(p);
        }

        return l;
    }

    public Vector2 ToVector2() {
        return new Vector2(this.x, this.y);
    }

    public override string ToString() {
        return "(" + this.x + ", " + this.y + ")";
    }

    public override bool Equals(object other) {

        Position p = other as Position;

        if (p == null)
            return false;
        return this.x == p.x && this.y == p.y;
    }

    public override int GetHashCode() {

        int hash = 23;
        hash = 17 * hash + this.x;
        hash = 17 * hash + this.y;

        return hash;
    }

    public int ManhattanDistance(Position p) {
        p -= this;
        if (p.x < 0)
            p.x *= -1;
        if (p.y < 0)
            p.y *= -1;
        return p.x + p.y;
    }

    public static bool operator == (Position p1, Position p2) {
        if (Object.ReferenceEquals(p1, null))
            return Object.ReferenceEquals(p2, null);
        else if (Object.ReferenceEquals(p2, null))
            return false;

        return p1.x == p2.x && p1.y == p2.y;
    }

    public static bool operator != (Position p1, Position p2) {
        return p1.x != p2.x || p1.y != p2.y;
    }

    public static Position operator + (Position p1, Position p2) {
        return new Position(p1.x + p2.x, p1.y + p2.y);
    }

    public static Position operator - (Position p1, Position p2) {
        return new Position(p1.x - p2.x, p1.y - p2.y);
    }
}
