using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BreakpointManager.BaseClasses
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		private const string EXECUTE_PREFIX = "Execute_";
		private const string CANEXECUTE_PREFIX = "CanExecute_";

		public event PropertyChangedEventHandler PropertyChanged;
		private readonly ConcurrentDictionary<string, object> _properties;
		private readonly ConcurrentDictionary<string, object> _values;

		private readonly List<string> _commandNames;
		private IDictionary<string, MethodInfo> _Methods;
		private IDictionary<string, DependsUponObject> _DependsUponDict;

		public ViewModelBase()
		{
			_properties = new ConcurrentDictionary<string, object>();
			_DependsUponDict = new ConcurrentDictionary<string, DependsUponObject>();
			_commandNames = new List<string>();

			Type MyType = GetType();
			_values = new ConcurrentDictionary<string, object>();
			__GetMembersAndGenerateCommands(MyType);
		}

		public object this[string key]
		{
			get
			{
				if (_values.ContainsKey(key))
					return _values[key];
				return null;
			}
			set
			{
				_values[key] = value;
				OnPropertyChanged(key);
			}
		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#region Get
		protected T Get<T>(Expression<Func<T>> expression)
		{
			return Get<T>(__GetPropertyName(expression));
		}
		protected T Get<T>(Expression<Func<T>> expression, T defaultValue)
		{
			return Get(__GetPropertyName(expression), defaultValue);
		}

		protected T Get<T>(T defaultValue, [CallerMemberName] string propertyName = null)
		{
			return Get(propertyName, defaultValue);
		}
		protected T Get<T>([CallerMemberName] string name = null)
		{
			return Get(name, default(T));
		}

		protected T Get<T>(string name, T defaultValue)
		{
			return GetValueByName<T>(name, defaultValue);
		}

		protected T GetValueByName<T>(String name, T defaultValue)
		{

			if (_properties.TryGetValue(name, out var val))
				return (T)val;

			return defaultValue;
		}
		#endregion

		#region [Set]

		protected void Set<T>(Expression<Func<T>> expression, T value)
		{
			Set(__GetPropertyName(expression), value);
		}

		protected void Set<T>(T value, [CallerMemberName] string propertyName = "")
		{
			Set(propertyName, value);
		}

		public void Set<T>(string name, T value)
		{
			if (_properties.TryGetValue(name, out var val))
			{
				if (val == null && value == null)
					return;

				if (val != null && val.Equals(value))
					return;
			}
			_properties[name] = value;
			this[name] = value;
			OnPropertyChanged(name);
			__RefreshDependendObjects(name);
		}
		#endregion

		private string __GetPropertyName<T>(Expression<Func<T>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new ArgumentException($"{nameof(expression)} must be a property {nameof(expression)}");

			return memberExpression.Member.Name;
		}

		private void __GetMembersAndGenerateCommands(Type myType)
		{
			var MethodInfos = new Dictionary<String, MethodInfo>(StringComparer.InvariantCultureIgnoreCase);
			foreach (var method in myType.GetMethods())
			{
				if (method.Name.StartsWith(EXECUTE_PREFIX))
					_commandNames.Add(method.Name.Substring(EXECUTE_PREFIX.Length));
				__ProcessMethodAttributes(method);
				MethodInfos[method.Name] = method;
			}
			foreach (var property in myType.GetProperties())
			{
				this[property.Name] = property;
				__ProcessPropertyAttributes(property);
			}
			_commandNames.ForEach(n => Set(n, new RelayCommand(p => __ExecuteCommand(n, p), p => __CanExecuteCommand(n, p))));
			_Methods = MethodInfos;
		}

		private void __ProcessPropertyAttributes(PropertyInfo property)
		{
			var attributes = property.GetCustomAttributes<DependsUpon>();
			if (attributes.Any())
				_DependsUponDict[property.Name] = new DependsUponObject { DependendObjects = attributes.Where(a => a.MemberName.IsNotNullOrEmpty()).Select(m => m.MemberName).ToList() };
		}

		private void __ProcessMethodAttributes(MethodInfo method)
		{
			var attributes = method.GetCustomAttributes<DependsUpon>();
			if (attributes.Any())
				_DependsUponDict[method.Name] = new DependsUponObject { DependendObjects = attributes.Where(a => a.MemberName.IsNotNullOrEmpty()).Select(m => m.MemberName).ToList() };
		}

		private void __ExecuteCommand(string name, object parameter)
		{
			MethodInfo methodInfo;
			_Methods.TryGetValue(EXECUTE_PREFIX + name, out methodInfo);
			if (methodInfo == null) return;

			methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] { parameter } : null);
		}

		private bool __CanExecuteCommand(string name, object parameter)
		{
			MethodInfo methodInfo;
			_Methods.TryGetValue(CANEXECUTE_PREFIX + name, out methodInfo);
			if (methodInfo == null) return true;

			return (bool)methodInfo.Invoke(this, methodInfo.GetParameters().Length == 1 ? new[] { parameter } : null);
		}

		private void __RefreshDependendObjects(string memberName)
		{
			if (_DependsUponDict != null)
			{
				var dependendObjects = _DependsUponDict.Where(d => d.Value != null && d.Value.DependendObjects != null && d.Value.DependendObjects.Contains(memberName));
				if (dependendObjects != null)
				{
					foreach (var dependsUponObj in dependendObjects)
					{
						if (dependsUponObj.Value.DependendObjects != null && dependsUponObj.Value.DependendObjects.Any())
						{
							if (_properties.ContainsKey(dependsUponObj.Key))
								OnPropertyChanged(dependsUponObj.Key);
							else if (_Methods.ContainsKey(dependsUponObj.Key))
							{
								MethodInfo methodInfo;
								_Methods.TryGetValue(dependsUponObj.Key, out methodInfo);
								if (methodInfo == null) return;
								if (methodInfo.GetParameters().Length == 0)
									methodInfo.Invoke(this, null);
							}
						}
					}
				}
			}
		}

	}

	public class DependsUponObject
	{
		public DependsUponObject()
		{
			DependendObjects = new List<string>();
		}
		public List<string> DependendObjects { get; set; }
		//public List<MethodInfo> Methods { get; private set; }
		//public List<MethodInfo> Properties { get; private set; }

	}

}
