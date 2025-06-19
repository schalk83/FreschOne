using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreschOne.Models
{
    public class FieldDefinition
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string InputType { get; set; } = "text";
        public string ControlType { get; set; } = "textbox";

        public string CssClass { get; set; } = "form-control";
        public string RowCssClass { get; set; }

        public bool Visible { get; set; } = true;
        public bool ReadOnly { get; set; } = false;
        public bool Disabled { get; set; } = false;
        public bool DisplayOnly { get; set; } = false;

        public bool Required { get; set; } = false;
        public int? MaxLength { get; set; }
        public int? MinLength { get; set; }
        public string RegexPattern { get; set; }
        public string ValidationMessage { get; set; }

        public bool RefreshOnChange { get; set; } = false;
        public List<string> DependsOn { get; set; } = new();

        public List<SelectListItem> Options { get; set; } = new();
        public bool IsCalculated { get; set; } = false;
        public string Formula { get; set; }

        public string HelpText { get; set; }
        public string TabIndex { get; set; }
        public bool HideLabel { get; set; } = false;
        public string Group { get; set; }

        public List<(DateTime changedAt, string user, object value)> AuditTrail { get; set; } = new();
    }

}
