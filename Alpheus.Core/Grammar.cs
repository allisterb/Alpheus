﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public abstract class Grammar<T> where T: new()
    {
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

        public static Parser<char> CloseSquareBracket
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

        public static Parser<char> Equal
        {
            get
            {
                return Parse.Char('=');
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
                    Parse.WhiteSpace.AtLeastOnce().Text()
                    .Or(Parse.LineTerminator).Or(Parse.LineEnd).AtLeastOnce().Select(w => string.Join(string.Empty, w)).Optional()
                    .Select(o => o.GetOrElse(string.Empty));
            }
        }

        public static Parser<string> EOL
        {
            get
            {
                return
                    from wp in OptionalMixedWhiteSpace
                    from e in Parse.LineTerminator.Or(Parse.LineEnd).Many().Select(l => string.Join(string.Empty, l))
                    from wa in OptionalMixedWhiteSpace
                    select wp + e + wa;
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
                return AlphaNumericIdentifier.Select(an => new AString(an));
            }
        }

        public static Parser<AString> AnyCharAString
        {
            get
            {
                return Parse.AnyChar.Many().Text().Select(aC => new AString(aC));
            }
        }

    }
}
