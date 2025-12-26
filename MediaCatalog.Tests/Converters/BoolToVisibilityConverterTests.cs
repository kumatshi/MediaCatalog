using System;
using System.Globalization;
using System.Windows;
using MediaCatalog.Converters;
using Xunit;

namespace MediaCatalog.Tests
{
    public class BoolToVisibilityConverterTests
    {
        [Fact]
        public void Convert_True_ReturnsVisible()
        {
            var converter = new BoolToVisibilityConverter();
            var result = converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);
            Assert.Equal(Visibility.Visible, result);
        }

        [Theory]
        [InlineData(true, null, Visibility.Visible)]
        [InlineData(false, null, Visibility.Collapsed)]
        [InlineData(true, "inverse", Visibility.Collapsed)]
        [InlineData(false, "inverse", Visibility.Visible)]
        public void Convert_VariousInputs_ReturnsExpected(bool input, string parameter, Visibility expected)
        {
            var converter = new BoolToVisibilityConverter();
            var result = converter.Convert(input, typeof(Visibility), parameter, CultureInfo.InvariantCulture);
            Assert.Equal(expected, result);
        }
    }
}