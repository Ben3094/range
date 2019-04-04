using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BenDotNet.Numerics
{
    public class Range<T> : INotifyPropertyChanged where T : IComparable<T>
    {
        protected Range() { }
        public Range(T minValue, T maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
        private T minValue;
        public virtual T MinValue
        {
            get => this.minValue;
        }
        private T maxValue;
        public virtual T MaxValue
        {
            get => this.maxValue;
        }

        public virtual bool IsInRange(T value)
        {
            return (minValue.CompareTo(value) <= 0) & (value.CompareTo(maxValue) <= 0);
        }
        public virtual T FindClosestValue(T value)
        {
            if (value.CompareTo(this.minValue) < 0)
                return this.minValue;
            else if (value.CompareTo(this.maxValue) > 0)
                return this.maxValue;
            else
                return value;
        }
        public bool IsCorrect(T value)
        {
            return this.IsInRange(value) & (this.FindClosestValue(value).Equals(value));
        }

        #region Properties changed event handler
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class DiscreteRange<T> : Range<T> where T : IComparable<T>
    {
        public DiscreteRange(ref IOrderedEnumerable<T> discreteValue) : base()
        {
            this.discreteValues = discreteValue;
        }
        private IOrderedEnumerable<T> discreteValues;
        public IOrderedEnumerable<T> DiscreteValues
        {
            get => this.discreteValues;
            set
            {
                this.discreteValues = value;
                OnPropertyChanged();
            }
        }
        
        public override T MinValue
        {
            get => this.discreteValues.First();
        }
        public override T MaxValue
        {
            get => this.discreteValues.Last();
        }

        public override T FindClosestValue(T value)
        {
            return this.discreteValues.OrderBy(discreteValue => Math.Abs(value.CompareTo(discreteValue))).First();
        }
    }

    public class LinearDiscreteRange : Range<float>
    {
        public LinearDiscreteRange(float minValue, float maxValue, float valueStep) : base(minValue, maxValue)
        {
            this.valueStep = valueStep;
        }
        private float valueStep;
        public float ValueStep
        {
            get => this.valueStep;
            set
            {
                this.valueStep = value;
                OnPropertyChanged();
            }
        }

        public override float FindClosestValue(float value)
        {
            if (value.CompareTo(this.MinValue) < 0)
                return this.MinValue;
            else if (value.CompareTo(this.MaxValue) > 0)
                return this.MaxValue;
            else
            {
                float toDelete = value % this.ValueStep;
                value -= toDelete;
                if (toDelete >= this.ValueStep / 2)
                    return value + this.ValueStep;
                else
                    return value;
            }
        }
    }

    public class CompositeRange : Range<float>
    {
        public readonly IEnumerable<Range<float>> Ranges;

        public CompositeRange(IEnumerable<Range<float>> ranges) : base()
        {
            this.Ranges = ranges;
        }

        public override float MinValue => this.Ranges.Min(range => range.MinValue);
        public override float MaxValue => this.Ranges.Max(range => range.MaxValue);

        public override float FindClosestValue(float value)
        {
            return FindClosestValues(value).OrderBy(maybeClosestValue => Math.Abs(maybeClosestValue - value)).First();

            IEnumerable<float> FindClosestValues(float valueQuery)
            {
                foreach (Range<float> range in this.Ranges)
                    yield return range.FindClosestValue(valueQuery);
            }
        }

        public override bool IsInRange(float value)
        {
            foreach (Range<float> range in this.Ranges)
                if (range.IsInRange(value))
                    return true;

            return false;
        }
    }
}

