/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Klocman.Extensions;
using Klocman.Native;
using Klocman.Tools;

namespace Klocman.IO
{
    public static class MsiTools
    {
        private static readonly int[] GuidRegistryFormatPattern = { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 };

        public static IEnumerable<Guid> MsiEnumProducts()
        {
            var sbProductCode = new StringBuilder(39);
            var iIdx = 0;

            while (MsiWrapper.MsiEnumProducts(iIdx++, sbProductCode) == 0)
            {
                var guidString = sbProductCode.ToString();
                if (GuidTools.GuidTryParse(guidString, out var guid))
                    yield return guid;
                else
                    Console.WriteLine($@"Invalid MSI guid in MsiEnumProducts: {guidString}");
            }
        }

        public static string MsiGetProductInfo(Guid productCode, MsiWrapper.INSTALLPROPERTY property)
        {
            var propertyLen = 512;
            var sbProperty = new StringBuilder(propertyLen);

            var code = MsiWrapper.MsiGetProductInfo(productCode.ToString("B"), property.PropertyName, sbProperty,
                ref propertyLen);

            //if (code != 0)
            //    throw new System.IO.IOException("MsiGetProductInfo returned error code " + code);

            //If code is 0 prevent returning junk
            return code != 0 ? null : sbProperty.ToString();
        }

        public static Guid ConvertBetweenUpgradeAndProductCode(Guid from)
        {
            return new Guid(from.ToString("N").Reverse(GuidRegistryFormatPattern));
        }

        public static X509Certificate2 GetCertificate(Guid productCode)
        {
            var localPackage = MsiGetProductInfo(productCode, MsiWrapper.INSTALLPROPERTY.LOCALPACKAGE);

            if (localPackage == null || !Path.IsPathRooted(localPackage) || !File.Exists(localPackage))
                return null;

            IntPtr certData;
            uint pcb = 0;
            var result = MsiWrapper.MsiGetFileSignatureInformation(localPackage, 0, out certData, null, ref pcb);

            if (result == 0)
                return new X509Certificate2(certData);

            return null;
        }
    }
}