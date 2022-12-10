using System;
namespace CustomCollections
{
    class DStack<T>
    {
        int count;
        StackNode lastNode;
        public int Count { get => count; set => count = value; }
        public class StackNode
        {
            T value;
            private StackNode prev;

            internal StackNode Prev { get => prev; set => prev = value; }
            public T Value { get => value; set => this.value = value; }

            public StackNode(T value, StackNode prev)
            {
                this.Value = value;
                this.Prev = prev;
            }

        }
        public void Push(T entry)
        {
            StackNode newNode = new StackNode(entry, lastNode);
            lastNode = newNode;
            count++;
        }
        public T Pop()
        {
            T resultValue = lastNode.Value;
            StackNode deeperNode = lastNode.Prev;
            lastNode = deeperNode;
            return resultValue;
        }
        public T Peek()
        {
            T result = lastNode.Value;
            return result;
        }
        public void Clear()
        {
            lastNode = null;
        }
    }
}
