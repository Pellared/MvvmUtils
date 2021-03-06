﻿using AutoMapper;
using Pellared.Common;
using Pellared.MvvmUtils.Validation;
using Pellared.SalaryBook.Common;
using Pellared.SalaryBook.Entities;
using Pellared.SalaryBook.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Pellared.SalaryBook.ViewModels
{
    public class EditableSalaryViewModel : ValidatableViewModel, IEditableObject, ISalary
    {
        private Memento<EditableSalaryViewModel> memento;
        private ValidationError[] secondPhaseErrors;

#if DEBUG

        /// <summary>
        /// Design-time constructor
        /// </summary>
        public EditableSalaryViewModel()
        {
            if (IsInDesignMode)
            {
                FirstName = "Jan";
                LastName = "Kowalski";
                BirthDate = DateTime.Today.AddYears(-20);
                SalaryValue = 1000;
            }
        }

#endif

        public EditableSalaryViewModel(IValidator<ISalary> salaryValidator)
        {
            SalaryValidator = salaryValidator;
            secondPhaseErrors = new ValidationError[0];
        }

        public IValidator<ISalary> SalaryValidator { get; private set; }

        private string firstName;

        public string FirstName
        {
            get { return firstName; }
            set
            {
                if (value != firstName)
                {
                    firstName = value;
                    RaisePropertyChanged(() => FirstName);
                }
            }
        }

        private string lastName;

        public string LastName
        {
            get { return lastName; }
            set
            {
                if (value != lastName)
                {
                    lastName = value;
                    RaisePropertyChanged(() => LastName);
                }
            }
        }

        private DateTime? birthDate;

        public DateTime? BirthDate
        {
            get { return birthDate; }
            set
            {
                if (value != birthDate)
                {
                    birthDate = value;

                    if (birthDate > DateTime.Today)
                        birthDate = DateTime.Today;

                    RaisePropertyChanged(() => BirthDate);
                }
            }
        }

        private double salaryValue;

        public double SalaryValue
        {
            get { return salaryValue; }
            set
            {
                if (Math.Abs(value - salaryValue) > double.Epsilon)
                {
                    salaryValue = Math.Round(value, 2);
                    RaisePropertyChanged(() => SalaryValue);
                }
            }
        }

        public void BeginEdit()
        {
            memento = new Memento<EditableSalaryViewModel>(this);
        }

        public void CancelEdit()
        {
            memento.Restore(this);
            memento = null;
        }

        public void EndEdit()
        {
            memento = null;
        }

        public void Clear()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthDate = null;
            SalaryValue = 0;
        }

        public void LoadEntity(Salary salary)
        {
            Mapper.DynamicMap(salary, this);
        }

        public Salary CreateEntity()
        {
            Salary result = Mapper.DynamicMap<Salary>(this);
            return result;
        }

        public void ValidateAll()
        {
            var errors = SecondPhaseValidation();
            if (errors != null)
            {
                secondPhaseErrors = errors.ToArray();
            }
            else
            {
                secondPhaseErrors = new ValidationError[0];
            }

            Validate();
        }

        public Task ValidateAllAsync()
        {
            return Task.Run(() => ValidateAll());
        }

        protected override IEnumerable<ValidationError> Validation()
        {
            var result = Enumerable.Empty<ValidationError>();

            var validationErrors = SalaryValidator.Validate(this);
            if (!validationErrors.IsNullOrEmpty())
            {
                result = result.Concat(validationErrors);
            }

            if (!secondPhaseErrors.IsNullOrEmpty())
            {
                result = result.Concat(secondPhaseErrors);
            }

            return result;
        }

        private IEnumerable<ValidationError> SecondPhaseValidation()
        {
            SalaryComplexValidator complexValidator = new SalaryComplexValidator();
            return complexValidator.Validate(this);
        }
    }
}