using Moq;
using Moq.AutoMock;
using Mover.Jobs;

namespace Mover.Engines;

public class JobEngineTests
{
    private readonly JobEngine jobEngine;

    private readonly AutoMocker mocks;

    public JobEngineTests()
    {
        mocks = new AutoMocker(MockBehavior.Loose, DefaultValue.Mock);

        jobEngine = mocks.CreateInstance<JobEngine>(true);
    }

    [Fact]
    public void Given_MoveJob_When_Run_Then_MoveJobEngineExecutes()
    {
        var job = new MoveJob();
        var moveResult = Task.CompletedTask;

        mocks
            .GetMock<MoveJobEngine>()
            .Setup(call => call.Run(job))
            .Returns(moveResult);

        var result = jobEngine.Run(job);

        Assert.Equal(moveResult, result);
    }
}