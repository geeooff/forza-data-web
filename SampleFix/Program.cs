namespace ForzaData.SampleFix;

static class Program
{
	[DynamicDependency(
		DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicNestedTypes,
		typeof(DefaultCommand)
	)]
	static int Main(string[] args) => new CommandApp<DefaultCommand>().Run(args);
}
