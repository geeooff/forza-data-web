namespace ForzaData.Console;

static class Program
{
	[DynamicDependency(
		DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicNestedTypes,
		typeof(DefaultCommand)
	)]
	static int Main(string[] args)
	{
		// enforcing UTF-8 output
		System.Console.OutputEncoding = System.Text.Encoding.UTF8;

		return new CommandApp<DefaultCommand>().Run(args);
	}
}