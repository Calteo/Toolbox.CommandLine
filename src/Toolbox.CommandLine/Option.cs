﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Toolbox.CommandLine
{
    /// <summary>
    /// Describes a single argument of the options
    /// </summary>
    [DebuggerDisplay("{Name,nq} -> {Property.PropertyName,nq}")]
    public class Option
    {
        internal Option(PropertyInfo property)
        {
            var optionAttribute = property.GetCustomAttribute<OptionAttribute>();

            Name = optionAttribute.Name;
            Property = property;
            DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>();
            Mandatory = property.GetCustomAttribute<MandatoryAttribute>() != null;
            Position = property.GetCustomAttribute<PositionAttribute>()?.Position;
            Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
        }

        /// <summary>
        /// Gets the name of the option
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Get the property associated with this option
        /// </summary>
        public PropertyInfo Property { get; }
        /// <summary>
        /// Get the default value if one is present.
        /// </summary>
        public DefaultValueAttribute DefaultValue { get; }
        /// <summary>
        /// Get the description of the option
        /// </summary>
        /// <see cref="DescriptionAttribute"/>
        public string Description { get; }
        /// <summary>
        /// Get the postion if one is present.
        /// </summary>
        /// <see cref="PositionAttribute"/>
        public int? Position { get; }
        /// <summary>
        /// Get if the option is mandatory
        /// </summary>
        /// <see cref="MandatoryAttribute"/>
        public bool Mandatory { get; }
        /// <summary>
        /// Is this option a boolean switch that takes no values.
        /// </summary>        
        public bool IsSwitch => Property.PropertyType == typeof(bool);

        internal void SetValue(object option, string value)
        {
            object optionValue;
            if (Property.PropertyType.IsArray)
            {
                var elementType = Property.PropertyType.GetElementType();
                var parts = value.Split(',');

                var array = Array.CreateInstance(elementType, parts.Length);
                for (var i = 0; i < parts.Length; i++)
                {
                    array.SetValue(ConvertTo(elementType, parts[i]), i);
                }
                optionValue = array;
            }
            else 
            {
                optionValue = ConvertTo(Property.PropertyType, value);
            }   
            
            Property.SetValue(option, optionValue);
        }

        private object ConvertTo(Type type, string value)
        {           
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(Nullable<>))
                {
                    return ConvertToCore(type.GetGenericArguments()[0], value);
                }
            }
            return ConvertToCore(type, value);
        }

        private object ConvertToCore(Type type, string value)
        {
            if (type.IsAssignableFrom(typeof(string))) return value;
            if (type.IsAssignableFrom(typeof(TimeSpan))) return TimeSpan.Parse(value);

            return Convert.ChangeType(value, type);
        }

        internal void SetDefault(object option)
        {
            if (DefaultValue != null)
                Property.SetValue(option, DefaultValue.Value);
        }

        internal void SetSwitch(object option)
        {
            Property.SetValue(option, true);
        }

        internal string GetUsage(char optionChar)
        {
            var typeText = $"<{Property.PropertyType.Name}>";
            var nameText = $"{optionChar}{Name}";
            if (Position.HasValue)
                nameText = $"[{nameText}]";

            var text = $"{nameText} {typeText}";

            if (IsSwitch)
                text = nameText;

            if (!Mandatory)
                text = $"[{text}]";

            return text;
        }
    }
}