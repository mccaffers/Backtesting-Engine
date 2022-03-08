using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using backtesting_engine;
using backtesting_engine_ingest;
using Moq;
using Utilities;
using Xunit;

namespace Tests;

[Collection("Sequential")]
public class DataInputTests
{

    private void SetEnvironmentVariables(){

        Environment.SetEnvironmentVariable("symbols", "TestEnvironmentSetup");
        Environment.SetEnvironmentVariable("TestEnvironmentSetup" + "_SF", "1000");
        Environment.SetEnvironmentVariable("folderPath", PathUtil.GetTestPath(""));
        Environment.SetEnvironmentVariable("strategy", "random");
        Environment.SetEnvironmentVariable("runID", "debug");
        Environment.SetEnvironmentVariable("elasticUser", "debug");
        Environment.SetEnvironmentVariable("elasticPassword", "debug");
        Environment.SetEnvironmentVariable("elasticCloudID", "debug");
        Environment.SetEnvironmentVariable("accountEquity", "debug");
        Environment.SetEnvironmentVariable("maximumDrawndownPercentage", "debug");
    }

    [Theory]
    [InlineData(1, "2018-01-01T01:00:00.594+00:00,1.35104,1.35065,1.5,0.75")]
    [InlineData(0, "UTC,AskPrice,BidPrice,AskVolume,BidVolume")]
    [InlineData(0, "2018-01-01T01:00:00.594+00:00,,,,")]
    [InlineData(0, ",,,,")]
    [InlineData(0, "")]
    public void TestPopulateLocalBuffer(int expectedResult, string line){

        SetEnvironmentVariables(); 

        var inputMock = new Mock<Ingest>(){
            CallBase = true
        };

        MethodInfo? populateLocalBuffer = inputMock?.Object.GetType().GetMethod("PopulateLocalBuffer", BindingFlags.NonPublic | BindingFlags.Instance);
        var fileName = "TestEnvironmentSetup";

        populateLocalBuffer?.Invoke(inputMock?.Object, new object[]{fileName, line});

        Assert.Equal(expectedResult, inputMock?.Object.localInputBuffer.Count);
    }
    
    [Fact]
    public async void TestFile()
    {

        var mySymbols = new List<string>();
        mySymbols.Add("testSymbol");

        var myFiles = new List<string>();
        myFiles.Add(PathUtil.GetTestPath("testSymbol.csv"));

        // Arrange
        var key = "symbols";
        var input = "TestEnvironmentSetup";
        Environment.SetEnvironmentVariable(key, input);

        key = input + "_SF";
        input = "1000";
        Environment.SetEnvironmentVariable(key, input);

        key = "folderPath";
        input = PathUtil.GetTestPath("");
        Environment.SetEnvironmentVariable(key, input);


        var programMock = new Mock<Main>(); // can't mock program
        var consumerMock = new Mock<IConsumer>();
        var inputMock = new Mock<Ingest>(){
            CallBase = true
        };

        inputMock.Setup(x=>x.EnvironmentSetup());

        consumerMock.Setup<Task>(x=>x.ConsumeAsync(It.IsAny<BufferBlock<PriceObj>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));
        // programMock.Setup(x=>x.EnvironmentSetup());
        // inputMock.Setup(x=>x.ReadLines(It.IsAny<BufferBlock<PriceObj>>())).Returns(Task.CompletedTask);

        var programObj = programMock.Object;
        // programObj.fileNames.Add(GetTestPath("testSymbol.csv"));

        await programObj.IngestAndConsume(consumerMock.Object, inputMock.Object);

        BufferBlock<PriceObj> buffer = programObj.GetFieldValue<BufferBlock<PriceObj>>("buffer");
        IList<PriceObj>? items;
        var output = buffer.TryReceiveAll(out items);

        Assert.True(items!=null && items.Count == 1);
    }
}