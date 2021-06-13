using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace CsGenericVisitors.AspNetCore
{
    /// <summary>
    /// <see cref="IModelBinder"/> implementation for binding complex types.
    /// </summary>
    public class ValidatedObjectModelBinder : ComplexTypeModelBinder
    {
        private Func<object> _modelCreator2;

        public ValidatedObjectModelBinder(IDictionary<ModelMetadata, IModelBinder> propertyBinders) : base(propertyBinders)
        {
        }

        public ValidatedObjectModelBinder(IDictionary<ModelMetadata, IModelBinder> propertyBinders, ILoggerFactory loggerFactory) : base(propertyBinders, loggerFactory)
        {
        }

        public ValidatedObjectModelBinder(IDictionary<ModelMetadata, IModelBinder> propertyBinders, ILoggerFactory loggerFactory, bool allowValidatingTopLevelNodes) : base(propertyBinders, loggerFactory, allowValidatingTopLevelNodes)
        {
        }


        protected override object CreateModel(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // If model creator throws an exception, we want to propagate it back up the call stack, since the
            // application developer should know that this was an invalid type to try to bind to.
            if (_modelCreator2 == null)
            {
                // The following check causes the ComplexTypeModelBinder to NOT participate in binding structs as
                // reflection does not provide information about the implicit parameterless constructor for a struct.
                // This binder would eventually fail to construct an instance of the struct as the Linq's NewExpression
                // compile fails to construct it.
                var modelTypeInfo = bindingContext.ModelType.GetTypeInfo();
                if (modelTypeInfo.IsAbstract || modelTypeInfo.GetConstructor(Type.EmptyTypes) == null)
                {
                    var metadata = bindingContext.ModelMetadata;
                    switch (metadata.MetadataKind)
                    {
                        case ModelMetadataKind.Parameter:
                            throw new InvalidOperationException(
                                Resources.FormatComplexTypeModelBinder_NoParameterlessConstructor_ForParameter(
                                    modelTypeInfo.FullName,
                                    metadata.ParameterName));
                        case ModelMetadataKind.Property:
                            throw new InvalidOperationException(
                                Resources.FormatComplexTypeModelBinder_NoParameterlessConstructor_ForProperty(
                                    modelTypeInfo.FullName,
                                    metadata.PropertyName,
                                    bindingContext.ModelMetadata.ContainerType.FullName));
                        case ModelMetadataKind.Type:
                            throw new InvalidOperationException(
                                Resources.FormatComplexTypeModelBinder_NoParameterlessConstructor_ForType(
                                    modelTypeInfo.FullName));
                    }
                }

                _modelCreator2 = Expression
                    .Lambda<Func<object>>(Expression.New(bindingContext.ModelType))
                    .Compile();
            }

            return _modelCreator2();
        }

        
    }
}