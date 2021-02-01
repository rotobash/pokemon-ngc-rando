using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.Utility;

namespace Randomizer
{
    public interface IGameExtractor
    {
        TrainerPool[] ExtractPools();
    }
}
