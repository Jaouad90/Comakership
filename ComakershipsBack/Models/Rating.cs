using Dahomey.Json.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    /// <summary>
    /// The rating class that represents the value of the review.
    /// </summary>

    public class Rating
    {
        /// <summary>
        /// The value of rating.
        /// </summary>
        /// <example>7</example>
        [Required]
        [JsonRequired(RequirementPolicy.Always)]
        [MinValue(1), MaxValue(10)]
        public int Value { get;set; }

    }

    internal class MinValueAttribute : Attribute
    {
        public int Min;

        public MinValueAttribute(int min)
        {
            Min = min;
            return;
        }
    }

    internal class MaxValueAttribute : Attribute
    {
        public int Max;

        public MaxValueAttribute(int max)
        {
            Max = max;
            return;
        }
    }
}
