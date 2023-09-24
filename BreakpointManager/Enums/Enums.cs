namespace BreakpointManager.Enums
{
	public enum eOperationMode
	{
		Document = 0,
		Project = 1,
		Solution = 2,
	}

	public enum eSetBreakpointMode
	{
		Properties = 0,
    PropertiesGetter = 1,
    PropertiesSetter = 2,

		Methods = 5,
    MethodsPrivate = 6,
    MethodsPublic = 7,
	}

}
