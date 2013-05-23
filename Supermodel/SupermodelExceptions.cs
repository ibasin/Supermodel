using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReflectionMapper;
using Supermodel.DDD.Models.Validation;

namespace Supermodel 
{
	public class SupermodelException : Exception 
    {
		public SupermodelException() : this("N/A") {} 
        public SupermodelException(string msg) : base(ReflectionHelper.GetThrowingContext() + ": " + msg) {}
	}

    public class SettingParentIdOnRootException : SupermodelException {}

    public class ChangingParentIdOnChildtoInvalidValueException : SupermodelException {}

    public class ModelStateInvalidException : SupermodelException
    {
        public ModelStateInvalidException(object model)
        {
            Model = model;
        }

        public object Model { get; protected set; }
    }

    public class ValidationResultException : SupermodelException
    {
        public ValidationResultException(string errorMessage, IEnumerable<string> memberNames)
        {
            ValidationResultList = new ValidationResultList { new ValidationResult(errorMessage, memberNames) };
        }

        public ValidationResultException(ValidationResultList validationResultList)
        {
            ValidationResultList = validationResultList;
        }

        public ValidationResultList ValidationResultList { get; protected set; }
    }

    public class UnableToDeleteException : Exception
    {
        public UnableToDeleteException(string errorMessageToDisplay) : base(errorMessageToDisplay) {}
    }
}
