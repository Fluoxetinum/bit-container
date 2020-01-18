using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Scripts
{
    public interface IT4DbInitScript
    {
        String DbName { get; }
        string TransformText();
    }
}
