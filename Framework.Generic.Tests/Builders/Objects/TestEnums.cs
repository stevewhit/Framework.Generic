using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Framework.Generic.Tests.Builders
{
    public enum TestEnum
    {
        [Description("Fake description for testing purposes.")]
        ValueWithDescription,

        [Display(Name = "Value With Display")]
        ValueWithDisplay,
        
        ValueWithoutDescriptionOrDisplay
    }

    public enum EmptyEnum
    {

    }
}
