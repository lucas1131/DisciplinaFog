
public class Pair<U, V> {
    private U u;
    private V v;

    public U first {
        get { return this.u; }
    };
    public V second {
        get { return this.v; }
    }

    public Pair(U first, V second) {
        this.u = first;
        this.v = second;
    }

    public static bool operator ==(Pair<U, V> p1, Pair<U, V> p2) {
        return p1.u == p2.u && p1.v == p2.v;
    }

    public static bool operator !=(Pair<U, V> p1, Pair<U, V> p2) {
        return p1.u != p2.u || p1.v != p2.v;
    }
}
