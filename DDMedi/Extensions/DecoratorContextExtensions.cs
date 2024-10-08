﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DDMedi
{
    public static class DecoratorContextExtensions
    {
        public static Task<R> Next<T, R>(this IAsyncDecorator<T,R> currentDecorator, T inputs, IAsyncDecoratorContext<R> context, CancellationToken token = default)
             where T : class =>
            context.Next(inputs, token);
        public static Task Next<T>(this IAsyncDecorator<T> currentDecorator, T inputs, IAsyncDecoratorContext context, CancellationToken token = default)
             where T : class =>
            context.Next(inputs, token);
        public static R Next<T, R>(this IDecorator<T, R> currentDecorator, T inputs, IDecoratorContext<R> context)
             where T : class =>
            context.Next(inputs);
        public static void Next<T>(this IDecorator<T> currentDecorator, T inputs, IDecoratorContext context)
            where T : class =>
            context.Next(inputs);
        public static Task Next<T>(this IEDecorator<T> currentDecorator, T inputs, IEDecoratorContext context, CancellationToken token = default)
            where T : IEInputs =>
            context.Next(inputs, token);

        public static Task<R> Next<T, R>(this IAsyncDecorator<T, R> currentDecorator, T inputs, IAsyncDecoratorContext<R> context, ISupplierScope scope, CancellationToken token = default)
            where T : class =>
            context.Next(inputs, scope, token);
        public static Task Next<T>(this IAsyncDecorator<T> currentDecorator, T inputs, IAsyncDecoratorContext context, ISupplierScope scope, CancellationToken token = default)
            where T : class =>
            context.Next(inputs, scope, token);
        public static R Next<T, R>(this IDecorator<T, R> currentDecorator, T inputs, IDecoratorContext<R> context, ISupplierScope scope)
            where T : class =>
            context.Next(inputs, scope);
        public static void Next<T>(this IDecorator<T> currentDecorator, T inputs, IDecoratorContext context, ISupplierScope scope)
            where T : class =>
            context.Next(inputs, scope);
        public static Task Next<T>(this IEDecorator<T> currentDecorator, T inputs, IEDecoratorContext context, ISupplierScope scope, CancellationToken token = default)
            where T : IEInputs =>
            context.Next(inputs, scope, token);
    }
}
