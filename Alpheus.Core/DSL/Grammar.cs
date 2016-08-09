﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public partial class DSL
    {
        public class Grammar : Grammar<DSL>
        {
            public static Parser<AString> Property
            {
                get
                {
                    return
                        from ob in OpenSquareBracket
                        from an in AlphaNumericAString
                        from cb in CloseSquareBracket
                        select an;
                }
            }

            public static Parser<KeyValuePair<AString, AString>> Key
            {
                get
                {
                    return
                        from k in AlphaNumericAString.Positioned()
                        from e in Equal.Token()
                        from v in AlphaNumericAString.Positioned()
                        select new KeyValuePair<AString, AString>(k, v);
                }
            }

            public static Parser<KeyValuePair<AString, AString>> MultiValuedKey
            {
                get
                {
                    return
                        from k in AlphaNumericAString.Positioned()
                        from e in Equal.Token()
                        from v in AlphaNumericAString.Positioned().DelimitedBy(Comma)
                            .Select(value => new AString
                            {
                                Position = value.First().Position,
                                Length = value.Sum(l => l.Length),
                                StringValue = string.Join(",", value),
                            })
                        select new KeyValuePair<AString, AString>(k, v);
                }
            }

            public static Parser<AString> Comment
            {
                get
                {
                    return
                        from c in SemiColon.Or(Hash)
                        from a in AnyCharAString.Positioned()
                        from w in OptionalMixedWhiteSpace
                        from e in EOL
                        select a;
                }
            }

            public static Parser<KeyValueSection> Section
            {
                get
                {
                    return
                        from w1 in OptionalMixedWhiteSpace
                        from sn in SectionName.Token().Positioned()
                        from k in Key.Or(MultiValuedKey).Token().DelimitedBy(EOL)
                        select new KeyValueSection(sn, k);
                        
                }
            }
        }
    }

}
