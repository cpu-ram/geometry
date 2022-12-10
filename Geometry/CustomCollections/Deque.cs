using System;
namespace CustomCollections
{
    class Deque<T>
    {
        int count;
        DequeNode<T> firstNode;
        DequeNode<T> lastNode;

        public int Count { get => count; set => count = value; }

        public class DequeNode<T>
        {
            T value;
            private DequeNode<T> prev;
            private DequeNode<T> next;

            internal DequeNode<T> Prev { get => prev; set => prev = value; }
            internal DequeNode<T> Next { get => next; set => next = value; }
            public T Value { get => value; set => this.value = value; }

            public DequeNode(T value, DequeNode<T> prev, DequeNode<T> next)
            {
                this.Value = value;
                this.Prev = prev;
                this.Next = next;
            }

        }
        public void AddFirst(T value)
        {
            if (firstNode == null)
            {
                DequeNode<T> newElement = new DequeNode<T>(value, null, null);
                firstNode = lastNode = newElement;
            }
            else
            {
                DequeNode<T> newElement = new DequeNode<T>(value, null, firstNode);
                firstNode.Prev = newElement;
                firstNode = newElement;
            }
            Count++;
        }
        public void AddLast(T value)
        {
            if (lastNode == null)
            {
                DequeNode<T> newElement = new DequeNode<T>(value, null, null);
                this.lastNode = this.firstNode = newElement;
            }
            else
            {
                DequeNode<T> newElement = new DequeNode<T>(value, lastNode, null);
                lastNode.Next = newElement;
                lastNode = newElement;
            }
            Count++;
        }
        public void Clear()
        {
            firstNode = null;
            lastNode = null;
        }
        public T FirstElement()
        {
            T resultValue = firstNode.Value;
            return resultValue;
        }
        public T LastElement()
        {
            T resultValue = lastNode.Value;
            return resultValue;
        }
        public void Print()
        {
            DequeNode<T> currentNode = firstNode;
            while (currentNode != null)
            {
                Console.WriteLine(currentNode.Value);
                currentNode = currentNode.Next;
            }
        }
        public void RemoveFirst()
        {
            firstNode = firstNode.Next;
            firstNode.Prev = null;
        }
        public void RemoveLast()
        {
            lastNode = lastNode.Prev;
            lastNode.Next = null;
        }
    }
}
