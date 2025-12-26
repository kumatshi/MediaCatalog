using System;
using MediaCatalog.Models;
using Xunit;

namespace MediaCatalog.Tests
{
    public class MediaItemTests
    {
        [Fact]
        public void Book_GetMediaType_ReturnsCorrectString()
        {
            var book = new Book();
            Assert.Equal("Книга", book.GetMediaType());
        }

        [Fact]
        public void Book_ValidData_IsValidReturnsTrue()
        {
            var book = new Book
            {
                Title = "Test Book",
                Author = "Author",
                Year = 2023,
                PageCount = 100,
                Rating = 5
            };
            Assert.True(book.IsValid());
        }

        [Theory]
        [InlineData("Title", "Author", 2023, 100, 5, true)]
        [InlineData("", "Author", 2023, 100, 5, false)]
        [InlineData("Title", "", 2023, 100, 5, false)]
        public void Book_Validation_TheoryTest(string title, string author, int year, int pages, int rating, bool expected)
        {
            var book = new Book
            {
                Title = title,
                Author = author,
                Year = year,
                PageCount = pages,
                Rating = rating
            };
            Assert.Equal(expected, book.IsValid());
        }
    }
}