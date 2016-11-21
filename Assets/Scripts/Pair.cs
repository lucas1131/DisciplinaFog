using System;

public class Pair<U, V> {
    private U u;
    private V v;

    public U first {
        get { return this.u; }
    }
    public V second {
        get { return this.v; }
    }

    public Pair(U first, V second) {
        this.u = first;
        this.v = second;
    }

    public bool Equals(Object other) {
        Pair<U, V> p = other as Pair<U, V>;
        if (p == null)
            return false;
        return p.u == this.u && p.v == this.v;
    }

    public static bool operator ==(Pair<U, V> p1, Pair<U, V> p2) {
        return Object.Equals(p1.u, p2.u) && Object.Equals(p1.v, p2.v);
    }

    public static bool operator !=(Pair<U, V> p1, Pair<U, V> p2) {
        return !(this == p2);
    }
}
