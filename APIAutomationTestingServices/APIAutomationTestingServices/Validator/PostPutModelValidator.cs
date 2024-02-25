using FluentValidation;
using ModelsLibrary;
using Newtonsoft.Json.Linq;

namespace APIAutomationTestingServices.Validator
{
    public class PostPutModelValidator : AbstractValidator<PostPutAPITestingModel>
    {
        public PostPutModelValidator()
        {
            RuleFor(model => model.APIUrl)
               .NotEmpty().WithMessage("APIUrl cannot be null or empty.");

            RuleFor(model => model.MethodType)
                .NotEmpty().WithMessage("MethodType cannot be null or empty.");

            RuleFor(model => model.JsonSchema)
                .NotEmpty().WithMessage("JsonSchema cannot be null or empty.")
                .Must(BeJsonObject).WithMessage("JsonSchema should be a valid JSON object.");
        }
        private bool BeJsonObject(string jsonSchema)
        {
            try
            {
                // Attempt to parse the JsonSchema to check if it's a valid JSON object
                var jObject = JObject.Parse(jsonSchema);
                return jObject.Type == JTokenType.Object;
            }
            catch
            {
                return false;
            }
        }
    }
}
