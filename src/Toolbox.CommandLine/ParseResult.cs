using System;

namespace Toolbox.CommandLine
{
    /// <summary>
    /// Represents the result of <see cref="Parser.Parse(string[])"/>
    /// </summary>
    public class ParseResult
    {
        /// <summary>
        /// Get the <see cref="Parser"/> that created the result
        /// </summary>
        public Parser Parser { get; internal set; }
        /// <summary>
        /// Gets the state of the parsing
        /// </summary>
        public State State { get; private set; } = State.Succeeded;
        /// <summary>
        /// Get the text of the parser, in case of an error.
        /// </summary>
        public string Text { get; private set; } = "";
        /// <summary>
        /// Gets the parsed option.
        /// </summary>
        /// <remarks>
        /// This might be a partly filled object if an error occured
        /// </remarks>
        /// <see cref="State"/>
        public object Option { get; internal set; }
        /// <summary>
        /// Gets the verb used.
        /// </summary>
        /// <see cref="VerbAttribute"/>
        public string Verb { get; internal set; }

        private int _return;
        /// <summary>
        /// The return value of the On-Methods.
        /// </summary>
        /// <remarks>
        /// It will only be valid after one of the On-Methods have been called.
        /// </remarks>
        public int Return 
        { 
            get
            {
                if (!ReturnSet)
                    throw new InvalidOperationException("Return not set");

                return _return;
            }
            private set
            {
                if (ReturnSet)
                    throw new InvalidOperationException("Return already set");

                _return = value;
                ReturnSet = true;
            }
        }
        private bool ReturnSet { get; set; }

        internal void SetState(State state, string text = "")
        {
            State = state;
            Text = text;
        }

        /// <summary>
        /// Handler for a given option type.
        /// </summary>
        /// <typeparam name="T">Type to handle</typeparam>
        /// <param name="handler">Function to call</param>
        /// <returns></returns>
        public ParseResult On<T>(Func<T, int> handler) where T : class
        {
            if (State==State.Succeeded && Option is T option)
            {
                Return = handler(option);
            }
            return this;
        }

        /// <summary>
        /// Handler for a given option type and a certain verb.
        /// </summary>
        /// <typeparam name="T">Type to handle</typeparam>
        /// <param name="verb">verb to match</param>
        /// <param name="handler">Function to call</param>
        /// <returns></returns>
        public ParseResult On<T>(string verb, Func<T, int> handler) where T : class
        {
            if (State == State.Succeeded && Verb==verb && Option is T option)
            {
                Return = handler(option);
            }
            return this;
        }

		/// <summary>
		/// Handler for a general option type.
		/// </summary>
		/// <param name="handler">Function to call</param>
		/// <returns></returns>
		public ParseResult On(Func<object, int> handler)
        {
			if (State == State.Succeeded)
			{
				Return = handler(Option);
			}
			return this;
		}

		/// <summary>
		/// Handler for a help request.
		/// </summary>
		/// <param name="handler">Function to call for doing the help</param>
		/// <returns></returns>
		/// <see cref="State.RequestHelp"/>
		public ParseResult OnHelp(Func<ParseResult, int> handler)
        {
            if (State == State.RequestHelp)
            {
                Return = handler(this);
            }
            return this;
        }

        /// <summary>
        /// Handler for errors.
        /// </summary>
        /// <param name="handler">Function to call for doing the help</param>
        /// <returns></returns>
        /// <see cref="State"/>
        public ParseResult OnError(Func<ParseResult, int> handler)
        {
            if (State.IsNoneOf(State.Succeeded, State.RequestHelp))
            {
                Return = handler(this);
            }
            return this;
        }

        /// <summary>
        /// Gets the help text for the result.
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public string GetHelpText(int width = 80)
        {
            return Parser.GetHelpText(Verb, width);
        }
    }

    /// <summary>
    /// State of the Parser
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Succesfully parsed all arguments
        /// </summary>
        Succeeded,
        /// <summary>
        /// Missing a verb on multiple options
        /// </summary>
        MissingVerb,
        /// <summary>
        /// Exspected an option as next argument
        /// </summary>
        MissingOption,
        /// <summary>
        /// Found an unknown option as next argument
        /// </summary>
        UnknownOption,
        /// <summary>
        /// Exspected an value for option as next argument
        /// </summary>
        MissingValue,
        /// <summary>
        /// Text could not be converted to the value of an option
        /// </summary>
        BadValue,
        /// <summary>
        /// One of the <see cref="Parser.HelpOptions"/> was found.
        /// </summary>
        RequestHelp,
        /// <summary>
        /// An mandatory option was not given
        /// </summary>
        MandatoryOption,
        /// <summary>
        /// An option was given more than once
        /// </summary>
        DuplicateOption
    }
}
