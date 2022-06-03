using System;
using System.IO.Ports;
using AutoBogus;
using AutoMapper;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace M5x.AutoMapper.Tests;

public class AutoMapperTests : IoCTestsBase
{

    public record A
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
    }
    
    public record B
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
    }


    private IMapper _mapper;

    

    [Fact]
    public void Should_BeAbleToMapAtoB()
    {
        var a = TestHelper.Create<A>();
        var b = _mapper.Map<A, B>(a);
        Assert.Null(b);
    }


    public AutoMapperTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected override void Initialize()
    {
        _mapper = Container.GetRequiredService<IMapper>();
    }

    protected override void SetTestEnvironment()
    {
        
    }

    protected override void InjectDependencies(IServiceCollection services)
    {
        services.AddAutoMapper();
    }
}