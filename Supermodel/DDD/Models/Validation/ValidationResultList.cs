using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Supermodel.DDD.Models.Validation
{
    public class ValidationResultList : List<ValidationResult>
    {
        public ValidationResult AddValidationResult<ModelT, ValueT>(ModelT model, Expression<Func<ModelT, ValueT>> expression, string message)
        {
            var validationResult = CreateValidationResult(model, expression, message);
            Add(validationResult);
            return validationResult;
        }
        public ValidationResult AddValidationResult<ModelT, Value1T, Value2T>(ModelT model, Expression<Func<ModelT, Value1T>> expression1, Expression<Func<ModelT, Value2T>> expression2, string message)
        {
            var validationResult = CreateValidationResult(model, expression1, expression2, message);
            Add(validationResult);
            return validationResult;
        }
        public ValidationResult AddValidationResult<ModelT, Value1T, Value2T, Value3T>(ModelT model, Expression<Func<ModelT, Value1T>> expression1, Expression<Func<ModelT, Value2T>> expression2, Expression<Func<ModelT, Value3T>> expression3, string message)
        {
            var validationResult = CreateValidationResult(model, expression1, expression2, expression2, message);
            Add(validationResult);
            return validationResult;
        }
        public ValidationResultList AddValidationResultList(ValidationResultList vrl)
        {
            foreach (var vr in vrl) Add(vr);
            return this;
        }

        public static ValidationResult CreateValidationResult<ModelT, ValueT>(ModelT model, Expression<Func<ModelT, ValueT>> expression, string message)
        {
            return new ValidationResult(message, new [] { GetPropertyName(model, expression) });
        }
        public static ValidationResult CreateValidationResult<ModelT, Value1T, Value2T>(ModelT model, Expression<Func<ModelT, Value1T>> expression1, Expression<Func<ModelT, Value2T>> expression2, string message)
        {
            return new ValidationResult(message, new[] { GetPropertyName(model, expression1), GetPropertyName(model, expression2) });
        }
        public static ValidationResult CreateValidationResult<ModelT, Value1T, Value2T, Value3T>(ModelT model, Expression<Func<ModelT, Value1T>> expression1, Expression<Func<ModelT, Value2T>> expression2, Expression<Func<ModelT, Value3T>> expression3, string message)
        {
            return new ValidationResult(message, new[] { GetPropertyName(model, expression1), GetPropertyName(model, expression2), GetPropertyName(model, expression3) });
        }

        public static string GetPropertyName<ModelT, ValueT>(ModelT model, Expression<Func<ModelT, ValueT>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess) throw new SupermodelException("Expression must describe a property");
            var memberExpression = (MemberExpression)expression.Body;
            var propertyName = memberExpression.Member is PropertyInfo ? memberExpression.Member.Name : null;
            return GetExpressionName(memberExpression.Expression) + propertyName;
        }

        public static string GetExpressionName(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter) return "";
            
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpression = (MemberExpression) expression;
                return GetExpressionName(memberExpression.Expression) + memberExpression.Member.Name + ".";
            }

            throw new Exception("Invalid Expression '" + expression + "'");
        }
    }
}
