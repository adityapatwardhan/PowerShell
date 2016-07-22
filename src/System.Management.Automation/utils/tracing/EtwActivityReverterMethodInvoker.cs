#if !UNIX
//-----------------------------------------------------------------------
// <copyright file="EtwActivityReverterMethodInvoker.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace System.Management.Automation.Tracing
{
    using System;

    internal class EtwActivityReverterMethodInvoker :
        IMethodInvoker
    {
        #region Instance Data

        private readonly IEtwEventCorrelator _eventCorrelator;
        private readonly Func<Guid, Delegate, object[], object> _invoker;

        #endregion

        #region Creation/Cleanup

        public EtwActivityReverterMethodInvoker(IEtwEventCorrelator eventCorrelator)
        {
            if (eventCorrelator == null)
            {
                throw new ArgumentNullException("eventCorrelator");
            }

            _eventCorrelator = eventCorrelator;
            _invoker = DoInvoke;
        }

        #endregion

        #region Instsance Access

        public Delegate Invoker
        {
            get { return _invoker; }
        }

        public object[] CreateInvokerArgs(Delegate methodToInvoke, object[] methodToInvokeArgs)
        {
            // See DoInvoke method for what these args mean.
            var retInvokerArgs = new object[]
            {
                _eventCorrelator.CurrentActivityId,
                methodToInvoke,
                methodToInvokeArgs,
            };

            return retInvokerArgs;
        }

        #endregion

        #region Instance Utilities

        private object DoInvoke(Guid relatedActivityId, Delegate method, object[] methodArgs)
        {
            using (_eventCorrelator.StartActivity(relatedActivityId))
            {
                return method.DynamicInvoke(methodArgs);
            }
        }

        #endregion
    }
}


#endif
