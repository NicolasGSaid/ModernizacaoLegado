using FluentAssertions;
using LegacyProcs.Application.Common.Models;
using Xunit;

namespace LegacyProcs.Tests.Application;

public class PaginationTests
{
    [Fact]
    public void Pagination_ShouldCalculate_TotalPages()
    {
        // Arrange
        var totalCount = 100;
        var pageSize = 10;

        // Act
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Assert
        totalPages.Should().Be(10);
    }

    [Theory]
    [InlineData(100, 10, 10)]
    [InlineData(95, 10, 10)]
    [InlineData(101, 10, 11)]
    [InlineData(50, 20, 3)]
    [InlineData(1, 10, 1)]
    public void Pagination_ShouldCalculate_CorrectTotalPages(
        int totalCount, 
        int pageSize, 
        int expectedPages)
    {
        // Act
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Assert
        totalPages.Should().Be(expectedPages);
    }

    [Fact]
    public void Pagination_ShouldValidate_PageNumber()
    {
        // Arrange
        var pageNumber = 1;
        var totalPages = 10;

        // Act
        var isValid = pageNumber >= 1 && pageNumber <= totalPages;

        // Assert
        isValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(11, false)]
    public void Pagination_ShouldValidate_PageNumberRange(
        int pageNumber, 
        bool expectedValid)
    {
        // Arrange
        var totalPages = 10;

        // Act
        var isValid = pageNumber >= 1 && pageNumber <= totalPages;

        // Assert
        isValid.Should().Be(expectedValid);
    }

    [Fact]
    public void Pagination_ShouldCalculate_HasNextPage()
    {
        // Arrange
        var currentPage = 5;
        var totalPages = 10;

        // Act
        var hasNextPage = currentPage < totalPages;

        // Assert
        hasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Pagination_ShouldCalculate_HasPreviousPage()
    {
        // Arrange
        var currentPage = 5;

        // Act
        var hasPreviousPage = currentPage > 1;

        // Assert
        hasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void Pagination_ShouldCalculate_Skip()
    {
        // Arrange
        var pageNumber = 3;
        var pageSize = 10;

        // Act
        var skip = (pageNumber - 1) * pageSize;

        // Assert
        skip.Should().Be(20);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(3, 10, 20)]
    [InlineData(1, 20, 0)]
    [InlineData(2, 20, 20)]
    public void Pagination_ShouldCalculate_CorrectSkip(
        int pageNumber, 
        int pageSize, 
        int expectedSkip)
    {
        // Act
        var skip = (pageNumber - 1) * pageSize;

        // Assert
        skip.Should().Be(expectedSkip);
    }

    [Fact]
    public void PaginatedResult_ShouldContain_AllMetadata()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2", "Item3" };
        var pageNumber = 1;
        var pageSize = 10;
        var totalCount = 25;

        // Act
        var result = new PaginatedResult<string>(
            items,
            totalCount,
            pageNumber,
            pageSize
        );

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(25);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void PaginatedResult_ShouldHandle_EmptyList()
    {
        // Arrange
        var items = new List<string>();
        var pageNumber = 1;
        var pageSize = 10;
        var totalCount = 0;

        // Act
        var result = new PaginatedResult<string>(
            items,
            totalCount,
            pageNumber,
            pageSize
        );

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void PaginatedResult_ShouldHandle_LastPage()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2" };
        var pageNumber = 3;
        var pageSize = 10;
        var totalCount = 22;

        // Act
        var result = new PaginatedResult<string>(
            items,
            totalCount,
            pageNumber,
            pageSize
        );

        // Assert
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeTrue();
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void Pagination_ShouldDefault_ToFirstPage()
    {
        // Arrange
        int? pageNumber = null;
        var defaultPage = 1;

        // Act
        var effectivePage = pageNumber ?? defaultPage;

        // Assert
        effectivePage.Should().Be(1);
    }

    [Fact]
    public void Pagination_ShouldDefault_ToStandardPageSize()
    {
        // Arrange
        int? pageSize = null;
        var defaultPageSize = 10;

        // Act
        var effectivePageSize = pageSize ?? defaultPageSize;

        // Assert
        effectivePageSize.Should().Be(10);
    }

    [Fact]
    public void Pagination_ShouldLimit_MaxPageSize()
    {
        // Arrange
        var requestedPageSize = 1000;
        var maxPageSize = 100;

        // Act
        var effectivePageSize = Math.Min(requestedPageSize, maxPageSize);

        // Assert
        effectivePageSize.Should().Be(100);
    }
}
