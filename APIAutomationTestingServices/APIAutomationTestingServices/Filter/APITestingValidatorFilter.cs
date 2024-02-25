using APIAutomationTestingServices.Validator;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ModelsLibrary;

namespace APIAutomationTestingServices.Filter
{
    public class APITestingValidatorFilter : ActionFilterAttribute
    {
        public string PropertyName { get; set; } = null!;
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool IsValid = true;
            var requestObj = context.ActionArguments[PropertyName];

            if(requestObj is PostPutAPITestingModel)
            {
                IsValid = await ValidatePostPutAPITestingModel(context, IsValid, (PostPutAPITestingModel)requestObj);
            }
            else if (requestObj is CustomPostPutTestingModel)
            {
                IsValid = await ValidateCustomPostPutTestingModel(context, IsValid, (CustomPostPutTestingModel)requestObj);
            }
            else if(requestObj is GetDeleteTestingModel)
            {
                IsValid = await ValidateGetDeleteTestingModel(context, IsValid, (GetDeleteTestingModel)requestObj);
            }
            else if(requestObj is CustomGetDelTestModel)
            {
                IsValid = await ValidateCustomGetDelTestModel(context, IsValid, (CustomGetDelTestModel)requestObj);
            }

            //IsValid = await ValidateCreateModel(context, IsValid, (AliquotCreateModel)requestObj);

            if (!IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            await next();
        }
        private async Task<bool> ValidatePostPutAPITestingModel(ActionExecutingContext context, bool IsValid, PostPutAPITestingModel requestObj)
        {
            ValidationResult result = await new PostPutModelValidator().ValidateAsync(requestObj);

            if (!result.IsValid)
            {
                result.Errors.ForEach(x =>
                {
                    context.ModelState.Remove(x.PropertyName);
                    context.ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                });
                IsValid = false;
            }

            return IsValid;
        }
        private async Task<bool> ValidateCustomPostPutTestingModel(ActionExecutingContext context, bool IsValid, CustomPostPutTestingModel requestObj)
        {
            ValidationResult result = await new CustomPostPutModelValidator().ValidateAsync(requestObj);

            if (!result.IsValid)
            {
                result.Errors.ForEach(x =>
                {
                    context.ModelState.Remove(x.PropertyName);
                    context.ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                });
                IsValid = false;
            }

            return IsValid; throw new NotImplementedException();
        }
        private async Task<bool> ValidateGetDeleteTestingModel(ActionExecutingContext context, bool IsValid, GetDeleteTestingModel requestObj)
        {
            ValidationResult result = await new GetDelModelValidator().ValidateAsync(requestObj);

            if (!result.IsValid)
            {
                result.Errors.ForEach(x =>
                {
                    context.ModelState.Remove(x.PropertyName);
                    context.ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                });
                IsValid = false;
            }

            return IsValid;
        }
        private async Task<bool> ValidateCustomGetDelTestModel(ActionExecutingContext context, bool IsValid, CustomGetDelTestModel requestObj)
        {
            ValidationResult result = await new CustomGetDelModelValidator().ValidateAsync(requestObj);

            if (!result.IsValid)
            {
                result.Errors.ForEach(x =>
                {
                    context.ModelState.Remove(x.PropertyName);
                    context.ModelState.AddModelError(x.PropertyName, x.ErrorMessage);
                });
                IsValid = false;
            }

            return IsValid;
        }
  
    }
}
