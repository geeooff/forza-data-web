using System.Net;

namespace ForzaData.SampleRecorder;

static class Program
{
	[DynamicDependency(
		DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicNestedTypes,
		typeof(DefaultCommand)
	)]
	static int Main(string[] args) => new CommandApp<DefaultCommand>().Run(args);
}