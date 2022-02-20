using System;
using System.Collections.Generic;
using OK.Siemens.Models;
using Xunit;

namespace OK.Siemens.DataProviders.Tests;

public class PlcDbParserTests
{
    [Fact]
    public void Parse_ShouldReturnRightDataList()
    {
        var expected = new PlcTag[]
        {
            new PlcTag {DataType = DataType.Real, DbAddress = new DbAddress {Byte = 0, Bit = 0}, Description = "ВО1.1 - Температура воздуха", TagName = "VO0101_TE_AIR"},
            new PlcTag {DataType = DataType.Real, DbAddress = new DbAddress {Byte = 4, Bit = 0}, Description = "ВО1.1 - Температура ребра", TagName = "VO0101_TE_SIDE"},
            new PlcTag {DataType = DataType.UInt, DbAddress = new DbAddress {Byte = 8, Bit = 0}, Description = "ВО1.1 - Состояние", TagName = "VO0101_STATE"},
            new PlcTag {DataType = DataType.Bool, DbAddress = new DbAddress {Byte = 10, Bit = 0}, Description = "ВО1.1 - Клапан подачи жидкости открыт", TagName = "VO0101_LIQ_VALVE_OPENED"},
            new PlcTag {DataType = DataType.Bool, DbAddress = new DbAddress {Byte = 10, Bit = 1}, Description = "ВО1.1 - Клапан отвода паро-жидкости открыт", TagName = "VO0101_SUC_VALVE_OPENED"},
            new PlcTag {DataType = DataType.Real, DbAddress = new DbAddress {Byte = 12, Bit = 0}, Description = "ВО2.1 - Температура воздуха", TagName = "VO0201_TE_AIR"}
        };
        
        var parser = new PlcDbParser();
        var tagsList = parser.ParseCsvFile("tags.csv");
        
        Assert.Collection(tagsList, 
            item => ComparePlcTag(item, expected[0]),
            item => ComparePlcTag(item, expected[1]),
            item => ComparePlcTag(item, expected[2]),
            item => ComparePlcTag(item, expected[3]),
            item => ComparePlcTag(item, expected[4]),
            item => ComparePlcTag(item, expected[5])
        );
    }

    private void ComparePlcTag(PlcTag item, PlcTag expected)
    {
        Assert.Equal(expected.TagName, item.TagName);
        Assert.Equal(expected.Description, item.Description);
        Assert.Equal(expected.DataType, item.DataType);
        Assert.Equal(expected.DbAddress.Bit, item.DbAddress.Bit);
        Assert.Equal(expected.DbAddress.Byte, item.DbAddress.Byte);
    }
}