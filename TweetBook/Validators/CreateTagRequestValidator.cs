using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Requests;

namespace TweetBook.Validators
{
    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(x => x.TagName)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}
