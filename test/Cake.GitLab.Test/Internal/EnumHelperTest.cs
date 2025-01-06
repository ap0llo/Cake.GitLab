using System;
using System.Runtime.Serialization;
using Cake.GitLab.Internal;
using Xunit;

namespace Cake.GitLab.Test.Internal;

/// <summary>
/// Tests for <see cref="EnumHelper"/>
/// </summary>
public class EnumHelperTest
{
    public enum SimpleEnum
    {
        Value1,
        Value2,
        someOtherValue
    }

    public enum EnumWithValueNames
    {
        Value1,
        [EnumMember(Value = "some_other_name")]
        Value2,

        [EnumMember(Value = "value3")]
        Value3
    }

    public class ConvertToString
    {
        [Theory]
        [InlineData(SimpleEnum.Value1, "Value1")]
        [InlineData(SimpleEnum.Value2, "Value2")]
        [InlineData(SimpleEnum.someOtherValue, "someOtherValue")]
        public void Returns_enum_name_if_no_value_name_is_defined(SimpleEnum value, string expected)
        {
            // ARRANGE

            // ACT
            var actual = value.ConvertToString();

            // ASSERT
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(EnumWithValueNames.Value1, "Value1")]
        [InlineData(EnumWithValueNames.Value2, "some_other_name")]
        [InlineData(EnumWithValueNames.Value3, "value3")]
        public void Returns_value_name_if_defined(EnumWithValueNames value, string expected)
        {
            // ARRANGE

            // ACT
            var actual = value.ConvertToString();

            // ASSERT
            Assert.Equal(expected, actual);
        }
    }


    public class Parse
    {
        [Theory]
        [InlineData("Value1", SimpleEnum.Value1)]
        [InlineData("Value2", SimpleEnum.Value2)]
        [InlineData("someOtherValue", SimpleEnum.someOtherValue)]
        // Parsing should be case-insensitive
        [InlineData("value1", SimpleEnum.Value1)]
        public void Returns_expected_enum_name_if_no_value_name_is_defined(string value, SimpleEnum expected)
        {
            // ARRANGE

            // ACT
            var actual = EnumHelper.Parse<SimpleEnum>(value);

            // ASSERT
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Value1", EnumWithValueNames.Value1)]
        [InlineData("some_other_name", EnumWithValueNames.Value2)]
        [InlineData("value3", EnumWithValueNames.Value3)]
        // Parsing should be case-insensitive
        [InlineData("VALUE3", EnumWithValueNames.Value3)]
        [InlineData("SOME_OTHER_NAME", EnumWithValueNames.Value2)]
        public void Returns_expected_enum_name_if_no_value_names_are_defined(string value, EnumWithValueNames expected)
        {
            // ARRANGE

            // ACT
            var actual = EnumHelper.Parse<EnumWithValueNames>(value);

            // ASSERT
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Throws_ArgumentException_if_value_cannot_be_parsed_for_enum_without_value_names()
        {
            // ARRANGE

            // ACT
            var ex = Record.Exception(() => EnumHelper.Parse<SimpleEnum>("unknown_value"));

            // ASSERT
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal<object>("Cannot convert value 'unknown_value' to enum SimpleEnum", ex.Message);
        }

        [Fact]
        public void Throws_ArgumentException_if_value_cannot_be_parsed_for_enum_with_value_names()
        {
            // ARRANGE

            // ACT
            var ex = Record.Exception(() => EnumHelper.Parse<EnumWithValueNames>("unknown_value"));

            // ASSERT
            Assert.IsType<ArgumentException>(ex);
            Assert.Equal<object>("Cannot convert value 'unknown_value' to enum EnumWithValueNames", ex.Message);
        }

    }
}
