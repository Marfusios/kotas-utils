using System;
using System.Linq;
using Kotas.Utils.Common.Utils;
using Xunit;

namespace Kotas.Utils.Common.Tests
{
    public class PrimitiveTypeUtilsTests
    {
        private readonly Type[] _primitiveTypes = {
            typeof(int),
            typeof(long),
            typeof(double),
            typeof(decimal),
            typeof(float),
            typeof(string),
            typeof(DateTime),
            typeof(byte),
            typeof(char),
            typeof(bool),
            typeof(sbyte),
            typeof(short),
            typeof(TestEnum),
        };

        private readonly Type[] _primitiveGenericTypes = {
            typeof(int?),
            typeof(long?),
            typeof(double?),
            typeof(decimal?),
            typeof(float?),
            typeof(DateTime?),
            typeof(byte?),
            typeof(char?),
            typeof(bool?),
            typeof(sbyte?),
            typeof(short?),
            typeof(TestEnum?),
        };

        private readonly Type[] _referenceTypes = {
            typeof(object),
            typeof(TestClass),
            typeof(NotImplementedException),
        };

        [Fact]
        public void IsPrimitive_ForPrimitiveTypesShouldReturnTrue()
        {
            foreach (var type in _primitiveTypes)
            {
                Assert.True(PrimitiveTypeUtils.IsPrimitive(type));
            }
        }

        [Fact]
        public void IsPrimitiveWithGeneric_ForPrimitiveTypesShouldReturnTrue()
        {
            var combined = _primitiveGenericTypes.Concat(_primitiveTypes);
            foreach (var type in combined)
            {
                Assert.True(PrimitiveTypeUtils.IsPrimitiveWithGeneric(type));
            }
        }

        [Fact]
        public void IsPrimitive_ForReferenceTypesShouldReturnFalse()
        {
            foreach (var type in _referenceTypes)
            {
                Assert.False(PrimitiveTypeUtils.IsPrimitive(type));
            }
        }

        [Fact]
        public void IsPrimitiveWithGeneric_ForReferenceTypesShouldReturnFalse()
        {
            foreach (var type in _referenceTypes)
            {
                Assert.False(PrimitiveTypeUtils.IsPrimitiveWithGeneric(type));
            }
        }

        private enum TestEnum
        {
            First, 
            Second
        }

        private class TestClass
        {
            public string Prop1 { get; set; }
            public double Prop2 { get; set; }
        }
    }
}
