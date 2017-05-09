using System;
using System.Collections;
using System.Collections.Generic;

namespace Portierniaktosiedzi.Models
{
    public class NegativeArray<T> : IEnumerable<T>
    {
        private readonly T[] rightArray;
        private readonly T[] leftArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="NegativeArray{T}"/> class by cloning existing arrays.
        /// </summary>
        /// <param name="leftArray">Array that will be indexed with indices less than 1</param>
        /// <param name="rightArray">Array that will be accessed with indices greater or equal 1</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any array length is less than one</exception>
        public NegativeArray(T[] leftArray, T[] rightArray)
        {
            if (leftArray.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(leftArray), "Length should be greater than 0");
            }

            if (rightArray.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rightArray), "Length should be greater than 0");
            }

            this.rightArray = (T[])rightArray.Clone();
            this.leftArray = (T[])leftArray.Clone();
        }

        /// <summary>
        /// Returns the T, assuming that values greater than 0 belong to the rightArray array and values lower or equal 0 to the leftArray.
        /// </summary>
        /// <param name="index">Values from minus leftArray.Length-1 to rightArray.Length</param>
        public T this[int index]
        {
            get
            {
                if (index < (leftArray.Length - 1) * -1 || index > rightArray.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Day must betweeen leftArray.Length-1 and rightArray.Length.");
                }

                return index <= 0 ? leftArray[Math.Abs(index)] : rightArray[--index];
            }

            set
            {
                if (index < (leftArray.Length - 1) * -1 || index > rightArray.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), "Day must betweeen leftArray.Length-1 and rightArray.Length.");
                }

                if (index <= 0)
                {
                    leftArray[Math.Abs(index)] = value;
                }
                else
                {
                    rightArray[--index] = value;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = (leftArray.Length - 1) * -1; i <= rightArray.Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
