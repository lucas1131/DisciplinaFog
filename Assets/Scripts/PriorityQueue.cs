using System;

public class PriorityQueue<E> {

    private class Node {
        public E elm;
        public int priority;
        public Node lchild;
        public Node rchild;

        public Node(E e, int p) {
            elm = e;
            priority = p;
        }

        public Node SetChild(Node n, bool dir) {
            if (dir)
                rchild = n;
            else
                lchild = n;

            return n;
        }

        public Node GetChild(bool dir) {
            if (dir)
                return rchild;
            else
                return lchild;
        }

        public void Swap(Node n) {
            E e = elm;
            int p = priority;
            elm = n.elm;
            priority = n.priority;
            n.elm = e;
            n.priority = p;
        }

        public void Fix() {
            Node smallest = this;

            if (lchild != null && lchild.priority < smallest.priority)
                smallest = lchild;
            if (rchild != null && rchild.priority < smallest.priority)
                smallest = rchild;

            if (smallest != this) {
                Swap(smallest);
                smallest.Fix();
            }
        }
    }


    private static readonly byte UINT_BITS = 8*sizeof(uint);

    public uint Count {
        get;
        private set;
    }
    private Node root;

    public PriorityQueue() {
        Count = 0;
    }

    private byte MostSignificantBitSet(uint n) {
        byte b = 0;
        while (n >> (UINT_BITS - 1 - b) == 0)
            b++;
        return b;
    }

    private Node Add(Node node, byte step, E elm, int priority) {
        if (step == UINT_BITS)
            return new Node(elm, priority);

        bool childDir = (((Count >> (UINT_BITS - 1 - step)) & 1) == 1);
        Node child = node.GetChild(childDir);
        node.SetChild(Add(child, (byte) (step+1), elm, priority), childDir);
        child = node.GetChild(childDir);

        if (child.priority < node.priority)
            node.Swap(child);

        return node;
    }

    public void Add(E elm, int priority) {
        Count++;
        byte step = MostSignificantBitSet(Count);
        root = Add(root, (byte) (step+1), elm, priority);
    }

    private bool Pop(Node node, byte step) {
        if (step == UINT_BITS)
            return false;

        bool childDir = (((Count >> (UINT_BITS - 1 - step)) & 1) == 1);
        Node child = node.GetChild(childDir);
        node.Swap(child);

        if (!Pop(child, (byte) (step+1)))
            node.SetChild(child.lchild, childDir);

        node.Fix();
        return true;
    }

    public E Pop() {
        if (Count == 0)
            return default(E);
        byte step = MostSignificantBitSet(Count);
        E e = root.elm;
        Pop(root, (byte) (step+1));
        Count--;
        return e;
    }

    private bool Update(Node n, Predicate<E> check, Func<int, int> update) {
        if (n != null) {
            bool success = false;
            if (check(n.elm)) {
                n.priority = update(n.priority);
                success = true;
            }

            success = Update(n.lchild, check, update) || success;
            success = Update(n.rchild, check, update) || success;

            if (success)
                n.Fix();

            return success;
        }

        return false;
    }

    public void Update(Predicate<E> check, Func<int, int> update) {
        Update(root, check, update);
    }

}
