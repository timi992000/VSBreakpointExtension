using System;

namespace BreakpointManager.BaseClasses
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	public class DependsUpon : Attribute
	{
		public string MemberName;
		public DependsUpon(string memberName)
		{
			MemberName = memberName;
		}
	}
}
