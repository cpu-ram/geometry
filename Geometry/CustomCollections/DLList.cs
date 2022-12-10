using System;
using System.Collections;
using System.Collections.Generic;
namespace CustomCollections
{
    class DLList<T> :IEnumerable<T>
    {
        int count;
        private DLListNode<T> firstNode;
        private DLListNode<T> lastNode;

        public int Count { get => count; set => count = value; }

        public void AddFirst(T value)
        {
            if (firstNode == null)
            {
                DLListNode<T> newElement = new DLListNode<T>(value, null, lastNode);
                firstNode = lastNode = newElement;
            }
            else
            {
                DLListNode<T> newElement = new DLListNode<T>(value, null, firstNode);
                firstNode.Previous = newElement;
                firstNode = newElement;
            }
            Count++;
        }
        public void AddFirst(DLListNode<T> entryNode)
        {
            T value = entryNode.Value;
            AddFirst(value);
        }
        public void AddLast(T value)
        {
            if (lastNode == null)
            {
                DLListNode<T> newElement = new DLListNode<T>(value, null, null);
                this.lastNode = this.firstNode = newElement;
            }
            else
            {
                DLListNode<T> newElement = new DLListNode<T>(value, lastNode, null);
                lastNode.Next = newElement;
                lastNode = newElement;
            }
            Count++;
        }
        public void AddLast(DLListNode<T> entryNode)
        {
            T value = entryNode.Value;
            AddLast(value);
        }
        

        public DLListNode<T> First
        {
            get
            {
                return this.firstNode;
            }
            set
            {
                this.firstNode = value;
            }
            
        }
        public DLListNode<T> Last
        {
            get
            {
                return this.lastNode;
            }
            set
            {
                this.lastNode = value;
            }
        }
        public DLListNode<T> ElementAt(int index)
        {
            if (index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == 0)
            {
                return firstNode;
            }
            int counter = 0;
            DLListNode<T> currentElement = this.firstNode;
            while (counter < index)
            {
                currentElement = currentElement.Next;
                counter++;
            }
            if (index == counter)
            {
                return currentElement;
            }
            return null;

        }
        public void InsertAtPosition(T entry, int index)
        {
            if (index >= count) throw new IndexOutOfRangeException();
            DLListNode<T> currentIndexHolder = ElementAt(index);
            DLListNode<T> elementBeforeCurrent = currentIndexHolder.Previous;
            DLListNode<T> newElement = new DLListNode<T>(entry, elementBeforeCurrent, currentIndexHolder);
            currentIndexHolder.Previous = newElement;
            elementBeforeCurrent.Next = newElement;
            Count++;
        }
        public void RemoveAt(int index)
        {
            DLListNode<T> currentIndexHolder = ElementAt(index);
            DLListNode<T> elementBefore = currentIndexHolder.Previous;
            DLListNode<T> nextElement = currentIndexHolder.Next;
            elementBefore.Next = nextElement;
            nextElement.Previous = elementBefore;
            Count--;
        }
        public void RemoveFirst()
        {
            DLListNode<T> second = (this.First).Next;
            second.Previous = null;
        }

        public int[] Search(T query, bool printResults = true)
        {
            int currentIndex = 0;
            List<int> positionsFoundList = new List<int>();
            int[] positionsFoundIntArr;
            DLListNode<T> entryElement = this.firstNode;
            DLListNode<T> currentElement = entryElement;
            while (currentIndex < Count)
            {
                if (currentElement.Value.Equals(query)) positionsFoundList.Add(currentIndex);
                currentElement = currentElement.Next;
                currentIndex++;
            }
            positionsFoundIntArr = positionsFoundList.ToArray();
            return positionsFoundIntArr;
        }
        public string Print()
        {
            string resultString = "";
            int currentPosition = 0;
            DLListNode<T> currentElement = firstNode;
            Console.WriteLine(currentElement.Value);
            resultString += currentElement.Value;
            while (currentPosition < Count - 1)
            {
                currentElement = currentElement.Next;
                currentPosition++;
                Console.WriteLine(currentElement.Value);
                resultString += currentElement.Value;
            }
            Console.WriteLine();
            return resultString;
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            DLListNode<T> currentNode = this.First;
            while (currentNode != null)
            {
                T currentValue = currentNode.Value;
                yield return currentValue;
                currentNode = currentNode.Next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
    public class DLListNode<T>
    {
        T value;
        private DLListNode<T> prev;
        private DLListNode<T> next;

        public DLListNode<T> Previous { get => prev; set => prev = value; }
        public DLListNode<T> Next { get => next; set => next = value; }
        public T Value { get => value; set => this.value = value; }

        public DLListNode(T value, DLListNode<T> prev, DLListNode<T> next)
        {
            this.Value = value;
            this.Previous = prev;
            this.Next = next;
        }

    }
}
