using FluentValidation;
using ModelsLibrary;

namespace APIAutomationTestingServices.Validator
{
    public class CustomGetDelModelValidator : AbstractValidator<CustomGetDelTestModel>
    {
        public CustomGetDelModelValidator()
        {
            RuleFor(model => model.APIUrl)
               .NotEmpty().WithMessage("APIUrl cannot be null or empty.");

            RuleFor(model => model.MethodType)
                .NotEmpty().WithMessage("MethodType cannot be null or empty.");

            RuleFor(model => model.Params)
                .Must(paramsList => ValidateParams(paramsList))
                .WithMessage("If Key has a value in Params, it must have at least one Values.");
        }
        private bool ValidateParams(List<ParamKeyValues> paramsList)
        {
            foreach (var param in paramsList)
            {
                if (!string.IsNullOrEmpty(param.Key) && (param.Values == null || param.Values.Count == 0))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
