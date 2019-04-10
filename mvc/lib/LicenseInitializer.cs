using ESRI.ArcGIS.esriSystem;
using System;

namespace Web.lib
{
    public class LicenseInitializer
    {
        private IAoInitialize m_AoInitialize = new AoInitialize();

        public bool InitializeApplication()
        {
            bool bInitialized = true;

            if (m_AoInitialize == null)
            {
                //System.Windows.Forms.MessageBox.Show("Unable to initialize. This application cannot run!");
                //throw new Exception("Unable to initialize. This application cannot run!");
                bInitialized = false;
            }

            //初始化应用程序
            esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;

            licenseStatus = CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
            if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
            {
                licenseStatus = CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeEngine);
                if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                {
                    licenseStatus = CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeArcView);
                    if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                    {
                        licenseStatus = CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeArcEditor);
                        if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            licenseStatus = CheckOutLicenses(esriLicenseProductCode.esriLicenseProductCodeArcInfo);
                            if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
                            {
                                //throw new Exception(LicenseMessage(licenseStatus));
                                bInitialized = false;
                            }
                        }

                    }
                }
            }

            if (licenseStatus == esriLicenseStatus.esriLicenseCheckedOut)
            {
                licenseStatus = m_AoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            }
            if (licenseStatus == esriLicenseStatus.esriLicenseCheckedOut)
            {
                licenseStatus = m_AoInitialize.CheckOutExtension(esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst);
            }
            else
            {
                throw new Exception("不能使用空间分析扩展！");
            }

            if (licenseStatus != esriLicenseStatus.esriLicenseCheckedOut)
            {
                throw new Exception("不能使用3D分析扩展！");
            }

            return bInitialized;
        }

        public void ShutdownApplication()
        {
            if (m_AoInitialize == null) return;

            if (m_AoInitialize.IsExtensionCheckedOut(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst))
            {
                m_AoInitialize.CheckInExtension(esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst);
            }
            if (m_AoInitialize.IsExtensionCheckedOut(esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst))
            {
                m_AoInitialize.CheckInExtension(esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst);
            }
            //关闭 AoInitilaize对象
            m_AoInitialize.Shutdown();
            m_AoInitialize = null;
        }

        private esriLicenseStatus CheckOutLicenses(esriLicenseProductCode productCode)
        {
            esriLicenseStatus licenseStatus;

            //是否产品是可能的
            licenseStatus = m_AoInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == esriLicenseStatus.esriLicenseAvailable)
            {
                //用相应的许可文件进行初始化
                licenseStatus = m_AoInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }

        private string LicenseMessage(esriLicenseStatus licenseStatus)
        {
            string message = "";

            //没有许可
            if (licenseStatus == esriLicenseStatus.esriLicenseNotInitialized)
            {
                message = "You are not licensed to run this product!";
            }
            //许可正在使用
            else if (licenseStatus == esriLicenseStatus.esriLicenseUnavailable)
            {
                message = "There are insuffient licenses to run!";
            }
            //未知错误
            else if (licenseStatus == esriLicenseStatus.esriLicenseFailure)
            {
                message = "Unexpected license failure! Please contact your administrator.";
            }
            //已经初始化
            else if (licenseStatus == esriLicenseStatus.esriLicenseAlreadyInitialized)
            {
                message = "The license has already been initialized! Please check your implementation.";
            }
            return message;
        }
    }
}