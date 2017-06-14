using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Portierniaktosiedzi.Models
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Reviewed.")]
    public class NegativeArray<T> : IEnumerable<T>
    {
        private readonly T[] rightArray;
        private readonly T[] leftArray;

        /// <summary>
        /// Initializes a new instance of <see cref="NegativeArray{T}"/> class from IEnumerables.
        /// </summary>
        /// <param name="left">IEnumerable that will be indexed with indices less than 1</param>
        /// <param name="right">IEnumerable that will be accessed with indices greater or equal 1</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any IEnumerable length is less than one</exception>
        public NegativeArray(IEnumerable<T> left, IEnumerable<T> right)
        {
            var leftArray = left.ToArray();
            var rightArray = right.ToArray();

            if (leftArray.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(left), "Length should be greater than 0");
            }

            if (rightArray.Length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(right), "Length should be greater than 0");
            }

            this.rightArray = rightArray;
            this.leftArray = leftArray;
        }

        public int Length => rightArray.Length + leftArray.Length;

        public int RightArrayLength => rightArray.Length;

        public int LeftArrayLength => leftArray.Length;

        /// <summary>
        /// Returns the T, assuming that values greater than 0 belong to the rightArray array and values lower or equal 0 to the leftArray.
        /// leftArray indexing uses absolute value. negativeArray[-1] -> leftArr[1]
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
