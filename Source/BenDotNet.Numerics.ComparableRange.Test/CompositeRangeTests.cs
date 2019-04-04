using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BenDotNet.Numerics.Range
{
    public class CompositeRangeTests
    {
        static Range<float> range1 = new Range<float>(2.35f, 800);
        static Range<float> range2 = new LinearDiscreteRange(900, 1000, 1);

        [Fact]
        public void FindClosestValueTest()
        {
            CompositeRange compositeRange = new CompositeRange(new List<Range<float>>() { range1, range2 });
            float closestValue = compositeRange.FindClosestValue(905.5f);
        }
    }
}
