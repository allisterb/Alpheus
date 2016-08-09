using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace Alpheus
{
    public class Lens<T> where T: ILensValue<T>
    {
        public Parser<T> Parser { get; set; }
        IResult<T> Result;
        public T Value;
        public string Template { get; set; }

        public bool Get(string t)
        {
            Result = this.Parser.TryParse(t); 
            Value = Result.WasSuccessful ? Result.Value : default(T);
            return Result.WasSuccessful;
        }

        public string Set(string t)
        {
            string o = string.Format(this.Template, Value.StringValue);
            return t.Insert(this.Value.Position.Pos, o);
        }
    }
}
