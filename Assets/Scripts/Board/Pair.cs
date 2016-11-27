using UnityEngine;
using System.Collections;

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

    public override bool Equals(object other) {
    
        Pair<U, V> p = other as Pair<U, V>;
    
        if (p == null) return false;
        return (p.u.Equals(this.u) && p.v.Equals(this.v));
    }

    public override int GetHashCode() {
        
        int hash = 23;
        hash = 17*hash + this.u.GetHashCode();
        hash = 17*hash + this.v.GetHashCode();
        
        return hash;
    }

    public static bool operator == (Pair<U, V> p1, Pair<U, V> p2) {
        return (Object.Equals(p1.u, p2.u) && Object.Equals(p1.v, p2.v));
    }

    public static bool operator != (Pair<U, V> p1, Pair<U, V> p2) {
        return !(p1 == p2);
    }
}
