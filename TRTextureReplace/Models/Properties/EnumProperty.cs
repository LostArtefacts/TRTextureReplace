using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRTextureReplace.Models;

public class EnumProperty : BaseProperty
{
    public override DataType DataType => DataType.Enum;

    private EnumOption _value;

    public EnumOption Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }
    }

    public List<EnumOption> Options { get; set; }
}

public class EnumOption
{
    public int ID { get; set; }
    public string Title { get; set; }
}
