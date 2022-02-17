using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using OK.Siemens.Models;
using Xunit;

namespace OK.Siemens.DataProviders.Tests;

#region Fixture

// ReSharper disable once ClassNeverInstantiated.Global
public class DbContextFixture
{
    public PgsqlDataRecordsRepository Repository { get; }
    private IQueryable<DataRecord> _data;

    public DbContextFixture()
    {
        // test data
        _data = new List<DataRecord>
        {
            new DataRecord{TagName = "Test", Value = 1.1, TimeStamp = DateTime.Parse("01.01.1970 07:00:00")},
            new DataRecord{TagName = "Test", Value = 2.2, TimeStamp = DateTime.Parse("01.01.1970 07:01:00")},
            new DataRecord{TagName = "Test", Value = 3.3, TimeStamp = DateTime.Parse("01.01.1970 07:03:00")},
            new DataRecord{TagName = "Test", Value = 4.4, TimeStamp = DateTime.Parse("01.01.1970 07:04:00")}
        }.AsQueryable();
        
        // mocking DbContext then
        var mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        mockDbContext.Setup(c => c.DataRecords).Returns(CreateMockDbSet);
        mockDbContext.Setup(c => c.AddRangeAsync(It.IsAny<IEnumerable<DataRecord>>(), It.IsAny<CancellationToken>()))
            .Returns((IEnumerable<DataRecord> x, CancellationToken _) => AddData(x, _));

        // finally mocking IDbContextFactory
        var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
        mockDbFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockDbContext.Object);
        
        Repository = new PgsqlDataRecordsRepository(mockDbFactory.Object);
    }

    private Task AddData(IEnumerable<DataRecord> x, CancellationToken _)
    {
        var list = _data.ToList();
        list.AddRange(x);
        _data = list.AsQueryable();
        return Task.FromResult(true);
    }

    private DbSet<DataRecord> CreateMockDbSet()
    {
        // mocking DbSet first 
        var mockDbSet = new Mock<DbSet<DataRecord>>();
        mockDbSet.As<IQueryable<DataRecord>>().Setup(m => m.Provider).Returns(_data.Provider);
        mockDbSet.As<IQueryable<DataRecord>>().Setup(m => m.Expression).Returns(_data.Expression);
        mockDbSet.As<IQueryable<DataRecord>>().Setup(m => m.ElementType).Returns(_data.ElementType);
        mockDbSet.As<IQueryable<DataRecord>>().Setup(m => m.GetEnumerator()).Returns(_data.GetEnumerator());
        return mockDbSet.Object;
    }
}

#endregion

public class PgsqlDataRecordsRepositoryTests: IClassFixture<DbContextFixture>
{
    private readonly DbContextFixture _fixture;

    public PgsqlDataRecordsRepositoryTests(DbContextFixture fixture)
    {
        _fixture = fixture;
    }
    
    /// <summary>
    /// Get only those data records which are between after and before dates 
    /// </summary>
    [Fact]
    public async Task GetRecordsBetweenTime_ShouldReturnExactRows()
    {
        var receivedData = await _fixture.Repository
            .GetRecordsBetweenTime(DateTime.Parse("01.01.1970 07:00:00"), DateTime.Parse("01.01.1970 07:02:00"));
        Assert.Equal(2, receivedData.Count());
    }

    /// <summary>
    /// Add two data records to repository and check it's count after
    /// </summary>
    [Fact]
    public async Task AddRecords_ShouldPresentInRepository()
    {
        var data = new List<DataRecord>
        {
            new DataRecord{TagName = "Test", Value = 6.6, TimeStamp = DateTime.Parse("01.01.1970 07:05:00")},
            new DataRecord{TagName = "Test", Value = 7.7, TimeStamp = DateTime.Parse("01.01.1970 07:06:00")}
        };
        var countPrev = await _fixture.Repository.GetRecordsBetweenTime(DateTime.Parse("01.01.1970 07:00:00"), DateTime.Now);
        
        await _fixture.Repository.AddDataRecordsAsync(data);
        var countCurrent = await _fixture.Repository.GetRecordsBetweenTime(DateTime.Parse("01.01.1970 07:00:00"), DateTime.Now);

        Assert.Equal(2, countCurrent.Count() - countPrev.Count());
    }
}