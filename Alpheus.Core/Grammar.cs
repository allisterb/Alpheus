using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public abstract class Grammar<T, S, K> where T: ConfigurationFile<S, K>, new() where S: IConfigurationNode where K: IConfigurationNode
    {
        public static T F = new T();

        #region Public methods
        public static Parser<AString> AStringFromIdentifierChar(Parser<char> c)
        {
            return  c.AtLeastOnce().Text().Select(s => new AString(s)).Positioned();
        }

        public static Parser<AString> AnyCharAString(string except)
        {
            Parser<char> r = Parse.CharExcept(except);
            return r.Many().Text().Select(s => new AString(s)).Positioned();
        }

        public static Parser<AString> DoubleQuoted(Parser<AString> S)
        {
            return
                from o in DoubleQuote
                from s in S
                from c in DoubleQuote
                select s;
        }
        #endregion

        public static Parser<char> Dot
        {
            get
            {
                return Parse.Char('.');
            }
        }

        public static Parser<char> Colon
        {
            get
            {
                return Parse.Char(':');
            }
        }

        public static Parser<char> SemiColon
        {
            get
            {
                return Parse.Char(';');
            }
        }

        public static Parser<char> Hash
        {
            get
            {
                return Parse.Char('#');
            }
        }

        public static Parser<char> Dash
        {
            get
            {
                return Parse.Char('-');
            }
        }

        public static Parser<char> Underscore
        {
            get
            {
                return Parse.Char('_');
            }
        }

        public static Parser<char> Caret
        {
            get
            {
                return Parse.Char('^');
            }
        }

        public static Parser<char> Comma
        {
            get
            {
                return Parse.Char(',');
            }
        }

        public static Parser<char> Ampersand
        {
            get
            {
                return Parse.Char('&');
            }
        }

        public static Parser<char> OpenBracket
        {
            get
            {
                return Parse.Char('(');
            }
        }

        public static Parser<char> ClosedBracket
        {
            get
            {
                return Parse.Char(')');
            }
        }

        public static Parser<char> OpenSquareBracket
        {
            get
            {
                return Parse.Char('[');
            }
        }

        public static Parser<char> ClosedSquareBracket
        {
            get
            {
                return Parse.Char(']');
            }
        }

        public static Parser<char> OpenAngledBracket
        {
            get
            {
                return Parse.Char('<');
            }
        }

        public static Parser<char> ClosedAngleBracket
        {
            get
            {
                return Parse.Char('>');
            }
        }

        public static Parser<char> OpenCurlyBracket
        {
            get
            {
                return Parse.Char('{');
            }
        }

        public static Parser<char> ClosedCurlyBracket
        {
            get
            {
                return Parse.Char('}');
            }
        }

        public static Parser<char> SingleQuote
        {
            get
            {
                return Parse.Char('\'');
            }
        }

        public static Parser<char> DoubleQuote
        {
            get
            {
                return Parse.Char('"');
            }
        }

        public static Parser<char> Equal
        {
            get
            {
                return Parse.Char('=');
            }
        }

        public static Parser<char> ForwardSlash
        {
            get
            {
                return Parse.Char('/');
            }
        }

        public static Parser<string> Digits
        {
            get
            {
                return Parse.Digit.AtLeastOnce().Text();
            }
        }

        public static Parser<char> NonDigit
        {
            get
            {
                return Parse.Letter;
            }
        }

        public static Parser<char> PositiveDigit
        {
            get
            {
                return Parse.Digit.Except(Parse.Char('0'));
            }
        }

        public static Parser<char> BeginEOL
        {
            get
            {
                return Parse.Char('\r').Or(Parse.Char('\n'));
            }
        }
        public static Parser<string> LineTerminator
        {
            get
            {
                return Parse.Char('\n').Select(c => new string(c, 1)).Or(Parse.String("\r\n")).Text();
            }
        }

        public static Parser<string> NumericIdentifier
        {
            get
            {
                return Digits;
            }
        }

        public static Parser<string> OptionalMixedWhiteSpace
        {
            get
            {
                return
                    Parse.WhiteSpace.AtLeastOnce().Text().Or(LineTerminator).Many()
                    .Select(w => string.Join(string.Empty, w)).Optional()
                    .Select(o => o.GetOrElse(string.Empty));
            }
        }

        public static Parser<string> NumericOnlyIdentifier
        {
            get
            {
                return
                    from chars in Parse.Digit.Or(Dot).AtLeastOnce()
                    let x = chars.Where(c => char.IsDigit(c)).ToArray()
                    select new string(x);
            }
        }

        public static Parser<char> AlphaNumericIdentifierChar
        {
            get
            {
                return Parse.Digit.Or(Parse.Letter);
            }
        }

        public static Parser<string> AlphaNumericIdentifier
        {
            get
            {
                return AlphaNumericIdentifierChar.AtLeastOnce().Text();
            }
        }

        public static Parser<AString> AlphaNumericAString
        {
            get
            {
                return AlphaNumericIdentifier.Select(an => new AString(an)).Positioned();
            }
        }


        //AStringFromIdentifierChar(Parse.AnyChar.Except(SingleQuote.Or(DoubleQuote).Or(Parse.Char('\n'))
    }
}
