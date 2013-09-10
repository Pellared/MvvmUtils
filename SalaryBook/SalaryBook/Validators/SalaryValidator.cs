﻿using System;

using FluentValidation;

using Pellared.SalaryBook.Entities;
using Pellared.SalaryBook.Properties;
using Pellared.Utils.Mvvm.Validation;

namespace Pellared.SalaryBook.Validators
{
    public class SalaryValidator : InlineFluentValidator<ISalary>
    {
        public SalaryValidator()
        {
            Func<ISalary, bool> nameValidation = x => !string.IsNullOrEmpty(x.FirstName) || !string.IsNullOrEmpty(x.LastName);

            FluentValidator.RuleFor(x => x.FirstName)
                           .Must((instance, _) => nameValidation(instance))
                           .WithMessage(Resources.NameValidationErrorText);
            FluentValidator.RuleFor(x => x.LastName)
                           .Must((instance, _) => nameValidation(instance))
                           .WithMessage(Resources.NameValidationErrorText);
            FluentValidator.ObjectRule(x => nameValidation(x) ? null : Resources.NameValidationErrorText);

            FluentValidator.RuleFor(x => x.BirthDate).NotEmpty().WithName(Resources.BirthDateText);
        }
    }
}