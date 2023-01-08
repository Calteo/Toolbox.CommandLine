using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Toolbox.CommandLine
{
    /// <summary>
    /// Collection of options for a given type.
    /// </summary>
    public class OptionType
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="OptionType"/> class.
        /// </summary>
        /// <param name="parser">The parser creating these arguments.</param>
        /// <param name="type">The type of the options</param>
        internal OptionType(Parser parser, Type type, string verb = null)
        {
            Parser = parser;
            Type = type;
            Verb = verb ?? type.GetCustomAttribute<VerbAttribute>(true)?.Verb ?? "";

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(p => p.GetCustomAttribute<OptionAttribute>() != null && p.CanWrite);

            Options = properties.Select(p => new Option(p)).ToArray();
            if (Options.GroupBy(o => o.Name).Any(g => g.Count() > 1))
            {
                throw new ArgumentException($"Duplicate options on type {type.FullName}.", nameof(type));
            }

            PositionOptions = Options.Where(o => o.Position.HasValue).OrderBy(o => o.Position).ToArray();
            if (PositionOptions.Any(p => p.Position != Array.IndexOf(PositionOptions, p)))
            {
                throw new ArgumentException($"Bad positions for options on type {type.FullName}.", nameof(type));
            }
        }

        /// <summary>
        /// Gets the <see cref="Parser"/> object.
        /// </summary>
        public Parser Parser { get; }
        /// <summary>
        /// Get the <see cref="Type"/> of the options.
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// Gets the verb of these options.
        /// </summary>
        /// <remarks>
        /// Will be <c>null</c> if only a single opton class is used for parsing.
        /// </remarks>
        /// <see cref="VerbAttribute"/>
        public string Verb { get; }
        /// <summary>
        /// Get the list of options.
        /// </summary>
        public Option[] Options { get; }
        /// <summary>
        /// Get the list of options with fixed positions.
        /// </summary>
        /// <see cref="PositionAttribute"/>
        public Option[] PositionOptions { get; }

        internal void Parse(ParseResult result, Queue<string> queue)
        {
            var counts = Options.ToDictionary(o => o.Name, o => 0);

            result.Option = Activator.CreateInstance(Type);
            Options.ToList().ForEach(o => o.SetDefault(result.Option));

            var count = 0;
            while (queue.Count>0)
            {
                var arg = queue.Dequeue();
                
                if (arg == "")
                    result.SetState(State.MissingOption, $"option exspected at [{count}].");
                else if (arg[0]!=Parser.OptionChar && count>=PositionOptions.Length)
                    result.SetState(State.MissingOption, $"option exspected at [{count}] '{arg}'.");
                else
                {
                    if (Parser.IsHelp(arg))
                    {
                        result.SetState(State.RequestHelp);
                    }
                    else
                    {
                        Option option;
                        string name;
                        string value = null;

                        if (arg[0] != Parser.OptionChar)
                        {
                            option = PositionOptions[count];
                            name = option.Name;
                            value = arg;
                        }
                        else
                        {
                            name = arg.Substring(1);
                            option = Options.FirstOrDefault(o => o.Name == name);
                        }

                        if (option == null)
                            result.SetState(State.UnknownOption, $"option not known at [{count}] '{arg}'.");
                        else
                        {
                            counts[option.Name]++;

                            if (option.IsSwitch)
                            {
                                option.SetSwitch(result.Option);
                            }
                            else
                            {
                                if (value == null)
                                {
                                    if (queue.Count == 0)
                                        result.SetState(State.MissingValue, $"option needs value after [{count}] '{arg}'.");
                                    else
                                        value = queue.Dequeue();
                                }
                                
                                if (value != null)
                                {
                                    try
                                    {
                                        option.SetValue(result.Option, value);
                                    }
                                    catch (Exception exception)
                                    {
                                        result.SetState(State.BadValue, $"bad option value at [{count}] '{arg}'='{value}' ({exception.Message}).");
                                    }
                                }
                            }
                        }
                    }
                }
                count++;
            }

            if (result.State==State.Succeeded)
            {
                var mandatory = Options.Where(o => o.Mandatory && counts[o.Name] == 0);
                if (mandatory.Any())
                {
                    result.SetState(State.MandatoryOption, $"mandatory options: {string.Join(", ", mandatory.Select(o => o.Name))}");
                }
                else
                {
                    var duplicate = Options.Where(o => counts[o.Name]>1);
                    if (duplicate.Any())
                    {
                        result.SetState(State.DuplicateOption, $"duplicate options: {string.Join(", ", duplicate.Select(o => o.Name))}");
                    }
                }
            }
        }

        internal string Description => Type.GetCustomAttribute<DescriptionAttribute>(true)?.Description ?? "";

        internal string GetHelpText(StringCollector collector, string executable)
        {
            var options = Options.OrderBy(o => o.Position ?? int.MaxValue).ThenBy(o => o.Name).ToArray();
            
            if (Verb != "" && Description != "")
            {
                collector.Indent = 2;
                collector.Append(Description);
                collector.AppendLine("");
                collector.Indent = 0;
                collector.AppendLine("");
            }

            collector.AppendLine("SYNTAX");
            collector.Indent = 2;
            collector.Append(executable);

            if (Verb != "")
                collector.Append($"{Verb}");

            foreach (var option in options.Select(o => o.GetUsage(Parser.OptionChar)))
            {
                collector.Append(option);
            }
            collector.AppendLine("");
            collector.Indent = 0;

            collector.AppendLine("");
            collector.AppendLine("OPTIONS");
            collector.Indent = 2;
            foreach (var option in options)
            {
                collector.AppendLine(option.GetUsage(Parser.OptionChar));
                if (option.Description != "")
                {
                    collector.Indent += 2;
                    collector.AppendLine(option.Description);
                    collector.Indent -= 2;
                }
                if (option.Property.PropertyType.IsEnum)
                {
                    collector.Indent += 4;
                    foreach (var name in Enum.GetNames(option.Property.PropertyType))
                    {
                        var description = option.Property.PropertyType.GetField(name).GetCustomAttribute<DescriptionAttribute>()?.Description;
                        if (description != null)
                            collector.AppendLine($"{name} - {description}");
                    }
                    collector.Indent -= 4;
                }
                if (option.DefaultValue != null)
                {
                    collector.Indent += 2;
                    collector.AppendLine($"default: '{option.DefaultValue.Value}'");
                    collector.Indent -= 2;
                }
                collector.AppendLine("");
            }
            collector.Indent = 0;

            return collector.ToString();
        }
    }
}