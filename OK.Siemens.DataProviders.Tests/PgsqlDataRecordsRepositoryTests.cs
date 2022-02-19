using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using OK.Siemens.Models;
using Xunit;

namespace OK.Siemens.DataProviders.Tests;

#region Fixture

// ReSharper disable once ClassNeverInstantiated.Global
public class DbContextFixture
{
    public PgsqlDataRecordsRepository Repository { get; }
    private IQueryable<DataRecord> _dataRecords;

    public DbContextFixture()
    {
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var mockDbContext = Create.MockedDbContextFor<AppDbContext>(dbContextOptions);
        
        #region DataRecords db context

        // test data
        _dataRecords = new List<DataRecord>
        {
            new DataRecord
            {
                TagName = new PlcTag
                    {Tagname = "Tag1", DbAddress = new DbAddress {Bit = 0, Byte = 0}, DataType = DataType.Real, Description = "Desc1"},
                Value = 1.1f, TimeStamp = DateTime.Parse("01.01.1970 07:00:00")
            },
            new DataRecord
            {
                TagName = new PlcTag
                    {Tagname = "Tag2", DbAddress = new DbAddress {Bit = 2, Byte = 0}, DataType = DataType.UInt, Description = "Desc2"},
                Value = 2.2f, TimeStamp = DateTime.Parse("01.01.1970 07:01:00")
            },
            new DataRecord
            {
                TagName = new PlcTag
                    {Tagname = "Tag3", DbAddress = new DbAddress {Bit = 4, Byte = 0}, DataType = DataType.Real, Description = "Desc3"},
                Value = 3.3f, TimeStamp = DateTime.Parse("01.01.1970 07:03:00")
            },
            new DataRecord
            {
                TagName = new PlcTag
                    {Tagname = "Tag4", DbAddress = new DbAddress {Bit = 6, Byte = 0}, DataType = DataType.UInt, Description = "Desc4"},
                Value = 4.4f, TimeStamp = DateTime.Parse("01.01.1970 07:04:00")
            }
        }.AsQueryable().AsNoTracking();
        
        mockDbContext.Set<DataRecord>().AddRange(_dataRecords);

        #endregion

        mockDbContext.SaveChanges();
        mockDbContext.ChangeTracker.Clear();

        // finally mocking IDbContextFactory
        var mockDbFactory = new Mock<IDbContextFactory<AppDbContext>>();
        mockDbFactory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockDbContext);
        
        Repository = new PgsqlDataRecordsRepository(mockDbFactory.Object);
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
            new DataRecord{TagName = new PlcTag{Id = 1}, Value = 6.6f, TimeStamp = DateTime.Parse("01.01.1970 07:05:00")},
            new DataRecord{TagName = new PlcTag{Id = 2}, Value = 7.7f, TimeStamp = DateTime.Parse("01.01.1970 07:06:00")}
        }.AsEnumerable();
        var countPrev = await _fixture.Repository.GetRecordsBetweenTime(DateTime.Parse("01.01.1970 07:00:00"), DateTime.Now);
        var countPrevCount = countPrev.ToList().Count();
        await _fixture.Repository.AddDataRecordsAsync(data);
        
        var countCurrent = await _fixture.Repository.GetRecordsBetweenTime(DateTime.Parse("01.01.1970 07:00:00"), DateTime.Now);
        var countCurrentCount = countCurrent.ToList().Count();

        Assert.Equal(2, countCurrentCount - countPrevCount);
    }

    [Fact]
    public async Task AddTagsAsync_ShouldPresentInRepository()
    {
        var data = new List<PlcTag>
        {
            new PlcTag
            {
                DataType = DataType.Bool, 
                DbAddress = new DbAddress {Bit = 8, Byte = 0}, 
                Description = "Desc5",
                Tagname = "Tag5"
            },
            new PlcTag
            {
                DataType = DataType.Real, 
                DbAddress = new DbAddress {Bit = 10, Byte = 0}, 
                Description = "Desc6",
                Tagname = "Tag6"
            }
        };
        var expected = await _fixture.Repository.GetTagsAsync();
        // expected.ToList().AddRange(data);

        await _fixture.Repository.AddTagsAsync(data);
        
        Assert.Collection(await _fixture.Repository.GetTagsAsync(), 
            item => ComparePlcTag(item, expected.ToArray()[0]),
            item => ComparePlcTag(item, expected.ToArray()[1]),
            item => ComparePlcTag(item, expected.ToArray()[2]),
            item => ComparePlcTag(item, expected.ToArray()[3]),
            item => ComparePlcTag(item, expected.ToArray()[4]),
            item => ComparePlcTag(item, expected.ToArray()[5]));
    }
    
    private void ComparePlcTag(PlcTag item, PlcTag expected)
    {
        Assert.Equal(expected.Tagname, item.Tagname);
        Assert.Equal(expected.Description, item.Description);
        Assert.Equal(expected.DataType, item.DataType);
        Assert.Equal(expected.DbAddress.Bit, item.DbAddress.Bit);
        Assert.Equal(expected.DbAddress.Byte, item.DbAddress.Byte);
    }
}