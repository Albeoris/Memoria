/*
 * Created by SharpDevelop.
 * User: LYCJ
 * Date: 27/10/2007
 * Time: 21:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace QuickZip.MiniHtml2
{
	
	
	/// <summary>
	/// Description of DependencyProterty.
	/// </summary>
	public class DependencyProperty<AnyType>
	{
		private static List<DependencyProperty<AnyType>> dependencyPropertyList = new List<DependencyProperty<AnyType>>();				
		private string name;
		private object propertyValue;
		private Type propertyType;
		private Type ownerType;
		private PropertyMetadata typeMetadata;
		private ValidateValueCallback validateValueCallback;
		
		public Type PropertyType { get { return propertyType; } }		
		internal object PropertyValue
		{
			get { return propertyValue; }
			set { propertyValue = value; CallValidateValueCallback(); }
		}
			
		
		
		
		private DependencyProperty(string aName, Type aPropertyType, 
		                           Type anOwnerType, PropertyMetadata aTypeMetadata,
		                           ValidateValueCallback aValidateValueCallback)
		{
			name = aName;
			propertyType = aPropertyType;
			ownerType = anOwnerType;
			typeMetadata = aTypeMetadata;
			validateValueCallback = aValidateValueCallback;
		}
			
		public static DependencyProperty<AnyType> Register(string name, Type propertyType, 
		                                          Type ownerType, PropertyMetadata typeMetadata,
		                                          ValidateValueCallback validateValueCallback)
		{
			DependencyProperty<AnyType> retVal = new DependencyProperty<AnyType>(name, propertyType, ownerType, 
			                                                   typeMetadata, validateValueCallback);
			
			
			
			
			dependencyPropertyList.Add(retVal);
			return retVal;
		}
		
		public static void SetValue(DependencyProperty<AnyType> property, object propertyValue)
		{			
			property.PropertyValue = propertyValue;
		}
		
		public static object GetValue(DependencyProperty<AnyType> property)
		{
			return property.PropertyValue;
		}
		
		
		private void CallValidateValueCallback()
		{
			if (validateValueCallback != null)
				validateValueCallback(this, new ValidateValueEventArgs());
		}
	}
	
	
	internal class DependencyPropertyList
	{
		
	}
	
	public class PropertyMetadata
	{
		
	}
	
	public class ValidateValueEventArgs : EventArgs
	{
		
	}
	
	public delegate void ValidateValueCallback(Object sender, ValidateValueEventArgs e);	
}

		
