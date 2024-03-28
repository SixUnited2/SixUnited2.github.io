using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace yz.gaming.accessoryapp.Utils.Com
{
    [ComImport]
    [Guid("37c994e7-432b-4834-a2f7-dce1f13b834b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ITipInvocation
    {
        void Toggle(IntPtr hwnd);
    }

    public static class ComTypes
    {
        internal static readonly Guid ImmersiveShellBrokerGuid;
        internal static readonly Type ImmersiveShellBrokerType;

        internal static readonly Guid TipInvocationGuid;
        internal static readonly Type TipInvocationType;

        static ComTypes()
        {
            TipInvocationGuid = Guid.Parse("4ce576fa-83dc-4F88-951c-9d0782b4e376");
            TipInvocationType = Type.GetTypeFromCLSID(TipInvocationGuid);

            ImmersiveShellBrokerGuid = new Guid("228826af-02e1-4226-a9e0-99a855e455a6");
            ImmersiveShellBrokerType = Type.GetTypeFromCLSID(ImmersiveShellBrokerGuid);
        }
    }
}
