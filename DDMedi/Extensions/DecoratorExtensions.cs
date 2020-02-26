using System;

namespace DDMedi
{
    public static class DecoratorExtensions
    {
        public static DDMediCollection AddAsyncDecorator<TInputs, TResult, TDecorator>(this DDMediCollection ddCollection, Func<IDecoratorInfo, bool> condition = null)
            where TInputs : IInputs<TResult>
            where TDecorator : IAsyncDecorator<TInputs, TResult>
        {
            ddCollection.DecoratorCollection.RegisterAsyncDecorator<TInputs, TResult, TDecorator>(condition);
            return ddCollection;
        }
        public static DDMediCollection AddAsyncDecorator<TInputs, TDecorator>(this DDMediCollection ddCollection, Func<IDecoratorInfo, bool> condition = null)
            where TInputs : IInputs
            where TDecorator : IAsyncDecorator<TInputs>
        {
            ddCollection.DecoratorCollection.RegisterAsyncDecorator<TInputs, TDecorator>(condition);
            return ddCollection;
        }
        public static DDMediCollection AddDecorator<TInputs, TResult, TDecorator>(this DDMediCollection ddCollection, Func<IDecoratorInfo, bool> condition = null)
                where TInputs : IInputs<TResult>
                where TDecorator : IDecorator<TInputs, TResult>
        {
            ddCollection.DecoratorCollection.RegisterDecorator<TInputs, TResult, TDecorator>(condition);
            return ddCollection;
        }
        public static DDMediCollection AddDecorator<TInputs, TDecorator>(this DDMediCollection ddCollection, Func<IDecoratorInfo, bool> condition = null)
                    where TInputs : IInputs
                    where TDecorator : IDecorator<TInputs>
        {
            ddCollection.DecoratorCollection.RegisterDecorator<TInputs, TDecorator>(condition);
            return ddCollection;
        }
        public static DDMediCollection AddEDecorator<TEInputs, TEDecorator>(this DDMediCollection ddCollection, Func<IDecoratorInfo, bool> condition = null)
            where TEInputs : IEInputs
            where TEDecorator : IEDecorator<TEInputs>
        {
            ddCollection.DecoratorCollection.RegisterEDecorator<TEInputs, TEDecorator>(condition);
            return ddCollection;
        }
        public static DDMediCollection AddGenericDecorator(this DDMediCollection ddCollection, Type iGenericSupplierType, Type genericImplementType,
            Func<IDecoratorInfo, bool> condition = null)
        {
            ddCollection.DecoratorCollection.RegisterGenericDecorator(iGenericSupplierType, genericImplementType, condition);
            return ddCollection;
        }
    }
}
