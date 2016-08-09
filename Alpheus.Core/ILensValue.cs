using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
namespace Alpheus
{
    public interface ILensValue<T> : IPositionAware<T>
    {
        string StringValue { get; set; }
        Position Position { get; set; }
        int Length { get; set; }
    }
}
