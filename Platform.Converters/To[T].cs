﻿using System;
using Platform.Exceptions;
using Platform.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Converters
{
    [Obsolete]
    public static class To<T>
    {
        public static readonly Func<T, object> Signed;
        public static readonly Func<T, object> Unsigned;
        public static readonly Func<object, T> UnsignedAs;

        static To()
        {
            Signed = CompileSignedDelegate();
            Unsigned = CompileUnsignedDelegate();
            UnsignedAs = CompileUnsignedAsDelegate();
        }

        static private Func<T, object> CompileSignedDelegate()
        {
            return DelegateHelpers.Compile<Func<T, object>>(emiter =>
            {
                Ensure.Always.IsUnsignedInteger<T>();
                emiter.LoadArgument(0);
                var method = typeof(To).GetMethod("Signed", Types<T>.Array);
                emiter.Call(method);
                emiter.Box(method.ReturnType);
                emiter.Return();
            });
        }

        static private Func<T, object> CompileUnsignedDelegate()
        {
            return DelegateHelpers.Compile<Func<T, object>>(emiter =>
            {
                Ensure.Always.IsSignedInteger<T>();
                emiter.LoadArgument(0);
                var method = typeof(To).GetMethod("Unsigned", Types<T>.Array);
                emiter.Call(method);
                emiter.Box(method.ReturnType);
                emiter.Return();
            });
        }

        static private Func<object, T> CompileUnsignedAsDelegate()
        {
            return DelegateHelpers.Compile<Func<object, T>>(emiter =>
            {
                Ensure.Always.IsUnsignedInteger<T>();
                emiter.LoadArgument(0);
                var signedVersion = NumericType<T>.SignedVersion;
                emiter.UnboxValue(signedVersion);
                var method = typeof(To).GetMethod("Unsigned", new[] { signedVersion });
                emiter.Call(method);
                emiter.Return();
            });
        }
    }
}
