using UnityEngine;
using System.Collections;

public class Position {

    private int _x;
    private int _y;

    private int x {
        get { return _x; }
    }

    private int y {
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
        return this.IsContained(0, 0, board.rows, board.cols);
    }

    public override bool Equals(object other) {
        
        Position p = other as Position;
        
        if (p == null)
            return false;
        return this.x == p.x && this.y == p.y;
    }

    public int GetHashCode() {
        
        int hash = 23;
        hash = 17*hash + this.x;
        hash = 17*hash + this.y;
        
        return hash;
    }

    public static bool operator ==(Position p1, Position p2) {
        return p1.x == p2.x && p1.y == p2.y;
    }

    public static bool operator !=(Position p1, Position p2) {
        return p1.x != p2.x || p1.y != p2.y;
    }

    public static Position operator +(Position p1, Position p2) {
        return new Position(p1.x + p2.x, p1.y + p2.y);
    }

    public static Position operator -(Position p1, Position p2) {
        return new Position(p1.x - p2.x, p1.y - p2.y);
    }

}
