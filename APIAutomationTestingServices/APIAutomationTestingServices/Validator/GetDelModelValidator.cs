using FluentValidation;
using ModelsLibrary;

namespace APIAutomationTestingServices.Validator
{
    public class GetDelModelValidator : AbstractValidator<GetDeleteTestingModel>
    {
        public GetDelModelValidator()
        {
            RuleFor(model => model.APIUrl)
               .NotEmpty().WithMessage("APIUrl cannot be null or empty.");

            RuleFor(model => model.MethodType)
                .NotEmpty().WithMessage("MethodType cannot be null or empty.");
        }
        
    }
}
