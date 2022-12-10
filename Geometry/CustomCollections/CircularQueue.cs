using System;
namespace CustomCollections
{
    public class CircularQueue<T>
    {
        T[] array;
        bool[] positionSet;
        bool isNull;
        int frontPointer;
        int rearPointer;

        public CircularQueue(int capacity)
        {
            array = new T[capacity];
            positionSet = new bool[capacity];
        }
        public void Add(T entry)
        {
            if (this.IsNull())
            {
                array[frontPointer] = entry;
                positionSet[frontPointer] = true;
                return;
            }
            if (!this.IsNull())
            {
                frontPointer = NextPosition(frontPointer);
                array[frontPointer] = entry;
                positionSet[frontPointer] = true;
                if (frontPointer == rearPointer)
                {
                    rearPointer = NextNonEmptyPosition(rearPointer);
                }
            }
            
        }
        public T Pop()
        {
            if (this.IsNull()) throw new Exception("Array " +
                "is empty, cannot retrieve or remove an element.");

            T result = array[rearPointer];
            array[rearPointer] = default(T);
            positionSet[rearPointer] = false;
            rearPointer = NextPosition(rearPointer);
            return result;
        }
        public int NextPosition(int startPosition)
        {
            int resultPosition;
            if (startPosition < array.Length - 1)
            {
                resultPosition = startPosition + 1;
            }
            else
            {
                resultPosition = 0;
            }
            return resultPosition;
        }
        public int NextNonEmptyPosition(int startPosition)
        {
            if (this.IsNull()) throw new Exception("Circular queue is null, " +
                "can not find next available position");
            int result=-1;
            if (startPosition < array.Length - 1)
            {
                for (int i = startPosition + 1; i < array.Length; i++)
                {
                    if (positionSet[i] == true)
                    {
                        result = i;

                        break;
                    }
                    if (i == array.Length - 1)
                    {
                        result = FindFromZero();
                        break;
                    }
                }
            }
            else result = FindFromZero();

            int FindFromZero()
            {
                for(int j=0; j<=startPosition; j++)
                {
                    if (positionSet[j] == true) return j;
                }
                throw new Exception();
            }
            return result;
        }
        public bool IsNull()
        {
            for(int i=0; i<positionSet.Length; i++)
            {
                if (positionSet[i] == true) return false;
            }
            frontPointer = 0;
            rearPointer = 0;
            return true;
        }
        public void Print()
        {
            if (this.IsNull())
            {
                throw new NullReferenceException("Linear queue is empty, " +
                    "can not print it."); 
            }
            if (frontPointer < rearPointer)
            {
                for(int i=rearPointer; i<array.Length; i++)
                {
                    Console.WriteLine(array[i]);
                }
                for(int i=0; i<=frontPointer; i++)
                {
                    Console.WriteLine(array[i]);
                }
            }
            if (frontPointer>=rearPointer)
            {
                for(int i=rearPointer; i<=frontPointer; i++)
                {
                    Console.WriteLine(array[i]);
                }
            }
            Console.WriteLine();
        }
    }
}
