// <copyright file="IdGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// A generator which generates id which should be unique within an instance of a generator.
    /// Ids which are no longer used can be given back to the generator, e.g. when an object joins a game map,
    /// it gets an id, later when it leaves the map, it gives the id back, so another player can use it.
    /// </summary>
    /// <remarks>
    /// The namespace for this class is probably not the right one - we're missing something like a "Utility" namespace for things like that.
    /// </remarks>
    public class IdGenerator
    {
        private readonly int maxValue;
        private readonly ConcurrentBag<int> givenBack = new ConcurrentBag<int>();
        private int currentValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdGenerator"/> class.
        /// </summary>
        /// <param name="firstValue">The first value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public IdGenerator(int firstValue, int maxValue)
        {
            this.maxValue = maxValue;
            this.currentValue = firstValue - 1; // will be increased by 1 by GetId
        }

        /// <summary>
        /// Gets an identifier which is unique within this generator instance.
        /// </summary>
        /// <returns>An identifier which is unique within this generator instance.</returns>
        /// <exception cref="InvalidOperationException">Maximum object id exceeded.</exception>
        public int GetId()
        {
            if (this.givenBack.TryTake(out int next))
            {
                return next;
            }

            if (this.currentValue == this.maxValue)
            {
                throw new InvalidOperationException("Maximum object id exceeded");
            }

            return Interlocked.Increment(ref this.currentValue);
        }

        /// <summary>
        /// Gives the id back for further usage by the next object.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void GiveBack(int id)
        {
            if (id <= this.maxValue)
            {
                this.givenBack.Add(id);
            }
        }
    }
}