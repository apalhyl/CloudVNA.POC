using Hyland.SOAP.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading;

namespace Hyland.SOAP.Service
{
    public class XUAOperationContext : IExtension<OperationContext>
    {
        #region IExtension<OperationContext>

        void IExtension<OperationContext>.Attach(OperationContext owner) { }

        void IExtension<OperationContext>.Detach(OperationContext owner) { }

        #endregion

        #region Static singleton object

        public static XUAOperationContext Current
        {
            get
            {
                XUAOperationContext context = null;

                if (OperationContext.Current != null && OperationContext.Current.Extensions != null)
                {
                    context = OperationContext.Current.Extensions.Find<XUAOperationContext>();
                }

                if (context == null)
                {
                    context = new XUAOperationContext();
                    if (OperationContext.Current != null && OperationContext.Current.Extensions != null)
                    {
                        OperationContext.Current.Extensions.Add(context);
                    }
                    else
                    {
                        object threadContext = Thread.GetData(Thread.GetNamedDataSlot("XUAOperationContext"));
                        if (threadContext != null && threadContext is XUAOperationContext)
                        {
                            context = threadContext as XUAOperationContext;
                        }
                        else
                        {
                            Thread.SetData(Thread.GetNamedDataSlot("XUAOperationContext"), context);
                        }
                    }
                }

                return context;
            }
        }

        private XUAOperationContext() { }

        #endregion

        public XUADetails XUADetails { get; private set; }

        public string UserName
        {
            get { return string.IsNullOrWhiteSpace(XUADetails.User) ? string.Empty : $"{XUADetails.Alias}<{XUADetails.User}@{XUADetails.Issuer}>"; }
        }

        public bool IsInValidUser
        {
            get { return XUADetails == null; }
        }

        public void SetXUADetails(XUADetails details)
        {
            XUADetails = details;
        }
    }
}
