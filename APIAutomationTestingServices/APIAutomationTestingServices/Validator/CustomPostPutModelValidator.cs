using FluentValidation;
using ModelsLibrary;
using Newtonsoft.Json.Linq;

namespace APIAutomationTestingServices.Validator
{
    public class CustomPostPutModelValidator : AbstractValidator<CustomPostPutTestingModel>
    {
        public CustomPostPutModelValidator()
        {
            RuleFor(model => model.APIUrl)
               .NotEmpty().WithMessage("APIUrl cannot be null or empty.");

            RuleFor(model => model.MethodType)
                .NotEmpty().WithMessage("MethodType cannot be null or empty.");

            RuleFor(model => model.JsonSchemas)
                .Must(jsonSchemas => jsonSchemas != null && jsonSchemas.Count > 0)
                .WithMessage("At least one JsonSchema must be provided.");

            RuleForEach(model => model.JsonSchemas)
                .Must(BeJsonType).WithMessage("All JsonSchemas values must be of JSON type.");
        }
        private bool BeJsonType(KeyValue keyValue)
        {
            if (!string.IsNullOrWhiteSpace(keyValue?.Value))
            {
                try
                {
                    // Attempt to parse the JsonSchema value to check if it's a valid JSON
                    var jObject = JObject.Parse(keyValue.Value);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}
